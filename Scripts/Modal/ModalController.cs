using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this for modals, name is used as id.
    /// </summary>
    [AddComponentMenu("M8/Modal/Controller")]
    public class ModalController : MonoBehaviour, IModalController {
        [Tooltip("Hide modals behind if this is the top.")]
        [SerializeField]
        bool _exclusive = true;

        public string id { get { return name; } }
        public bool isExclusive { get { return _exclusive; } }
        public bool isActive { get; private set; }
        public ModalManager owner { get; private set; }

        public event System.Action<bool> activeCallback;

        //Interfaces grabbed from within gameObject
        private IModalPush[] mIPushes;
        private IModalPop[] mIPops;
        private IModalOpen[] mIOpens;
        private IModalOpening[] mIOpenings;
        private IModalClose[] mICloses;
        private IModalClosing[] mIClosings;
        private IModalActive[] mIActives;

        public void Close() {
            if(owner)
                owner.CloseUpTo(id, true);
        }

        public void CloseIfTop() {
            if(owner && owner.GetTopBehaviour() == this)
                owner.CloseTop();
        }

        //IModalController Implements
        MonoBehaviour IModalController.behaviour { get { return this; } }

        void IModalController.Init() {
            //grab interfaces
            var comps = gameObject.GetComponentsInChildren<MonoBehaviour>(true);

            var IPushes = new List<IModalPush>();
            var IPops = new List<IModalPop>();
            var IOpens = new List<IModalOpen>();
            var IOpenings = new List<IModalOpening>();
            var ICloses = new List<IModalClose>();
            var IClosings = new List<IModalClosing>();
            var IActives = new List<IModalActive>();

            for(int i = 0; i < comps.Length; i++) {
                var comp = comps[i];

                var push = comp as IModalPush;
                if(push != null) IPushes.Add(push);

                var pop = comp as IModalPop;
                if(pop != null) IPops.Add(pop);

                var open = comp as IModalOpen;
                if(open != null) IOpens.Add(open);

                var opening = comp as IModalOpening;
                if(opening != null) IOpenings.Add(opening);

                var close = comp as IModalClose;
                if(close != null) ICloses.Add(close);

                var closing = comp as IModalClosing;
                if(closing != null) IClosings.Add(closing);

                var active = comp as IModalActive;
                if(active != null) IActives.Add(active);
            }

            mIPushes = IPushes.ToArray();
            mIPops = IPops.ToArray();
            mIOpens = IOpens.ToArray();
            mIOpenings = IOpenings.ToArray();
            mICloses = ICloses.ToArray();
            mIClosings = IClosings.ToArray();
            mIActives = IActives.ToArray();

            isActive = false;

            //default hide
            gameObject.SetActive(false);
        }

        void IModalController.SetOwner(ModalManager mgr) {
            owner = mgr;
        }

        void IModalController.Push(GenericParams parms) {
            for(int i = 0; i < mIPushes.Length; i++)
                mIPushes[i].Push(parms);
        }

        void IModalController.Pop() {
            for(int i = 0; i < mIPops.Length; i++)
                mIPops[i].Pop();
        }

        void IModalController.Open() {
            for(int i = 0; i < mIOpens.Length; i++)
                mIOpens[i].Open();
        }

        IEnumerator IModalController.Opening() {
            for(int i = 0; i < mIOpenings.Length; i++)
                yield return mIOpenings[i].Opening();
        }

        void IModalController.Close() {
            for(int i = 0; i < mICloses.Length; i++)
                mICloses[i].Close();
        }

        IEnumerator IModalController.Closing() {
            for(int i = 0; i < mIClosings.Length; i++)
                yield return mIClosings[i].Closing();
        }

        void IModalController.SetShow(bool aShow) {
            gameObject.SetActive(aShow);
        }

        void IModalController.SetActive(bool aActive) {
            isActive = aActive;

            for(int i = 0; i < mIActives.Length; i++)
                mIActives[i].SetActive(aActive);

            if(activeCallback != null)
                activeCallback(isActive);
        }
    }
}