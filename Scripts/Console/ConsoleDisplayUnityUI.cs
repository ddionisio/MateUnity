using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M8 {
    public class ConsoleDisplayUnityUI : MonoBehaviour {
        [System.Serializable]
        public class LogTypeData {
            public LogType logType;
            public Sprite icon;
            public Color color = Color.white;
        }

        [Header("Console")]
        public Console console;

        [Header("Config")]
        [SerializeField]
        bool _showLogError = true;
        [SerializeField]
        bool _showLogWarning = true;
        [SerializeField]
        bool _showLogInfo = true;

        [Header("Templates")]
        public ConsoleDisplayUnityUILogWidget template;
        public bool templateIsPrefab;

        [Header("Log Widget Config")]
        public LogTypeData[] logTypes;

        [Header("Display")]
        public Transform logRoot;
        public InputField inputField;

        public GameObject toggleErrorGO;
        public GameObject toggleWarningGO;
        public GameObject toggleInfoGO;

        private CacheList<ConsoleDisplayUnityUILogWidget> mLogWidgets;
        private CacheList<ConsoleDisplayUnityUILogWidget> mLogCacheWidgets;

        public void ToggleInfo() {
            _showLogInfo = !_showLogInfo;

            RefreshLogs();
            RefreshInfoToggle();
        }

        public void ToggleWarning() {
            _showLogWarning = !_showLogWarning;

            RefreshLogs();
            RefreshWarningToggle();
        }

        public void ToggleError() {
            _showLogError = !_showLogError;

            RefreshLogs();
            RefreshErrorToggle();
        }

        public void SubmitInput(string text) {
            console.Execute(text);

            //reactivate input
            if(inputField) {
                inputField.text = "";
                inputField.ActivateInputField();
            }
        }

        public void Clear() {
            console.Clear();
        }

        void OnDestroy() {
            if(console)
                console.refreshCallback -= RefreshLogs;
        }

        void Awake() {
            if(!templateIsPrefab)
                template.gameObject.SetActive(false);

            mLogWidgets = new CacheList<ConsoleDisplayUnityUILogWidget>(console.maxLog);
            mLogCacheWidgets = new CacheList<ConsoleDisplayUnityUILogWidget>(console.maxLog);

            //fill in logs
            RefreshLogs();

            RefreshErrorToggle();
            RefreshWarningToggle();
            RefreshInfoToggle();

            console.refreshCallback += RefreshLogs;
        }

        private LogTypeData GetLogTypeData(LogType type) {
            for(int i = 0; i < logTypes.Length; i++) {
                if(logTypes[i].logType == type)
                    return logTypes[i];
            }
            return null;
        }

        void RefreshLogs() {
            //refresh activate logs, add new log, remove leftover logs
            var consoleLogs = console.logs;

            int addInd = 0;

            for(int consoleInd = 0; consoleInd < consoleLogs.Count; consoleInd++) {
                var consoleLog = consoleLogs[consoleInd];

                bool canAdd;

                switch(consoleLog.type) {
                    case LogType.Assert:
                    case LogType.Error:
                    case LogType.Exception:
                        canAdd = _showLogError;
                        break;
                    case LogType.Log:
                        canAdd = _showLogInfo;
                        break;
                    case LogType.Warning:
                        canAdd = _showLogWarning;
                        break;
                    default:
                        canAdd = false;
                        break;
                }

                if(canAdd) {
                    if(addInd == mLogWidgets.Count) {
                        ConsoleDisplayUnityUILogWidget itm;

                        //add new log
                        if(mLogCacheWidgets.Count > 0) //add from cache
                            itm = mLogCacheWidgets.RemoveLast();
                        else //create new log
                            itm = Instantiate(template, logRoot);

                        itm.gameObject.SetActive(true);
                        itm.transform.SetAsLastSibling();
                        mLogWidgets.Add(itm);
                    }

                    ApplyLog(mLogWidgets[addInd], consoleLog);

                    addInd++;
                }
            }

            int deltaCount = mLogWidgets.Count - addInd;

            //remove excess
            if(deltaCount > 0) {
                for(int i = 0; i < deltaCount; i++) {
                    var itm = mLogWidgets.RemoveLast();
                    itm.gameObject.SetActive(false);
                    mLogCacheWidgets.Add(itm);
                }
            }

            //preserve scroll position
        }

        void RefreshErrorToggle() {
            if(toggleErrorGO) {
                toggleErrorGO.SetActive(_showLogError);
            }
        }

        void RefreshWarningToggle() {
            if(toggleWarningGO) {
                toggleWarningGO.SetActive(_showLogWarning);
            }
        }

        void RefreshInfoToggle() {
            if(toggleInfoGO) {
                toggleInfoGO.SetActive(_showLogInfo);
            }
        }

        void ApplyLog(ConsoleDisplayUnityUILogWidget widget, Console.LogItem log) {
            var logType = GetLogTypeData(log.type);
            if(logType != null)
                widget.Init(logType.icon, log.text, logType.color);
            else
                widget.Init(null, log.text, Color.white);
        }
    }
}