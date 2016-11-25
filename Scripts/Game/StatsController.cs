using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [System.Serializable]
    public struct StatTemplateData {
        public string name;
        public int id;
    };

    [System.Serializable]
    public struct StatTemplateList {
        [SerializeField]
        List<StatTemplateData> items;

        public static string ToJSON(List<StatTemplateData> items, bool prettyPrint) {
            return JsonUtility.ToJson(new StatTemplateList() { items=items }, prettyPrint);
        }

        public static List<StatTemplateData> FromJSON(string json) {
            return !string.IsNullOrEmpty(json) ? JsonUtility.FromJson<StatTemplateList>(json).items : new List<StatTemplateData>();
        }
    }

    [System.Serializable]
    public class StatItem {
        public const int InvalidID = 0;
        
        [SerializeField]
        int _id;

        [SerializeField]
        float _value;

        [SerializeField]
        bool _clamp;

        public int id { get { return _id; } }

        public float value { get { return _value; } }
        public int valueI { get { return Mathf.RoundToInt(value); } }

        public float currentValue {
            get { return mCurVal; }
            set {
                if(mCurVal != value) {
                    var prevVal = mCurVal;
                    mCurVal = _clamp ? Mathf.Clamp(value, 0f, _value) : value;

                    if(changeCallback != null)
                        changeCallback(this, mCurVal - prevVal);
                }
            }
        }

        public int currentValueI {
            get { return Mathf.RoundToInt(currentValue); }
            set { currentValue = value; }
        }
        
        public event System.Action<StatItem, float> changeCallback; //float delta
        public event System.Action<StatItem> resetCallback;

        private float mCurVal;
                
        public StatItem(int aId) {
            _id = aId;
            _value = 0f;
            _clamp = true;
        }

        public StatItem(int aId, float aValue, bool aClamp) {
            _id = aId;
            _value = aValue;
            _clamp = aClamp;
        }

        public void Reset() {
            mCurVal = _value;

            if(resetCallback != null)
                resetCallback(this);
        }
    }

    [System.Serializable]
    public class Stats : IEnumerable<StatItem> {
        [SerializeField]
        StatItem[] _statItems;

        public Stats(StatItem[] items) {
            _statItems = items;
        }
        
        public StatItem GetStatItem(int id) {
            for(int i = 0; i < _statItems.Length; i++) {
                if(_statItems[i].id == id)
                    return _statItems[i];
            }
            return null;
        }

        public void Reset() {
            for(int i = 0; i < _statItems.Length; i++)
                _statItems[i].Reset();
        }
                
        public IEnumerator<StatItem> GetEnumerator() {
            for(int i = 0; i < _statItems.Length; i++)
                yield return _statItems[i];
        }
                
        IEnumerator IEnumerable.GetEnumerator() {
            yield return GetEnumerator();
        }

        //TODO: database of sort
    }

    /// <summary>
    /// Simple way for a dynamic stats
    /// </summary>
    [AddComponentMenu("M8/Game/Stats")]
    public class StatsController : MonoBehaviour, IEnumerable<StatItem> {
        [SerializeField]
        Stats _stats;

        public StatItem this[int id] {
            get {
                if(_stats == null)
                    return null;

                return _stats.GetStatItem(id);
            }
        }

        public IEnumerator<StatItem> GetEnumerator() {
            return ((IEnumerable<StatItem>)_stats).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<StatItem>)_stats).GetEnumerator();
        }

        public void Reset() {
            if(_stats == null)
                _stats = new Stats(new StatItem[0]);

            _stats.Reset();
        }

        public void Override(StatItem[] items) {
            _stats = new Stats(items);
        }
        
        void Awake() {
            Reset();
        }
    }
}