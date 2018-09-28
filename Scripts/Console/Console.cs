using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace M8 {
    [CreateAssetMenu(fileName = "console", menuName = "M8/Console")]
    public class Console : ScriptableObject, ILogger {
        public const string tagCommand = "command";

        public struct LogItem {
            public LogType type;
            public string tag;
            public string text;
        }

        [Header("Config")]
        public string[] tagCommandFilters; //add tags to allow Console to only include specific commands
        public int maxLog = 256;
        public bool defaultUnityLogEnabled = true;
        public bool defaultLogEnabled = true;
        public LogType[] defaultLogTypeFilters = new LogType[] { LogType.Assert, LogType.Error, LogType.Exception, LogType.Log, LogType.Warning };

        [Header("Signals")]
        public SignalConsole signalRefresh;

        public bool unityLogEnabled {
            get { return mIsUnityLogEnabled; }
            set {
                if(mIsUnityLogEnabled != value) {
                    mIsUnityLogEnabled = value;
                    Refresh();

                    if(mIsUnityLogEnabled)
                        Application.logMessageReceived += OnUnityLog;
                    else
                        Application.logMessageReceived -= OnUnityLog;
                }
            }
        }

        public bool logEnabled {
            get { return mIsLogEnabled; }
            set {
                if(mIsLogEnabled != value) {
                    mIsLogEnabled = value;
                    Refresh();
                }
            }
        }

        public List<LogItem> logs {
            get { return mLogs; }
        }

        public event System.Action refreshCallback;

        LogType ILogger.filterLogType { get; set; }
        ILogHandler ILogger.logHandler { get; set; }

        private class Command {
            public string name;
            public string hint;
            public ConsoleParamType[] parms;

            public MethodInfo method;

            public string logHint {
                get {
                    var sb = new System.Text.StringBuilder();

                    sb.Append(name);

                    for(int i = 0; i < parms.Length; i++) {
                        sb.Append(' ');

                        sb.Append(parms[i].ToString());

                        if(i < parms.Length - 1)
                            sb.Append(',');
                    }

                    sb.Append('\n');

                    if(!string.IsNullOrEmpty(hint))
                        sb.Append(hint);

                    return sb.ToString();
                }
            }

            public Command(MethodInfo aMethod) {
                method = aMethod;

                var commandAttr = System.Attribute.GetCustomAttribute(method, typeof(ConsoleCommandAttribute), true) as ConsoleCommandAttribute;
                if(commandAttr != null) {
                    name = !string.IsNullOrEmpty(commandAttr.alias) ? commandAttr.alias : method.Name;
                    hint = commandAttr.hint;
                }
                else {
                    name = method.Name;
                    hint = "";
                }

                parms = ConsoleParam.GenerateParams(method);
            }

            public Command(MethodInfo aMethod, string alias, string hint) {
                method = aMethod;

                name = !string.IsNullOrEmpty(alias) ? alias : method.Name;
                this.hint = hint;

                parms = ConsoleParam.GenerateParams(method);
            }
            
            public void Execute(object[] objParms) {
                method.Invoke(null, objParms);
            }
        }

        private bool mIsUnityLogEnabled = true;
        private bool mIsLogEnabled = true;

        private Dictionary<string, Command> mCommands = new Dictionary<string, Command>();
        private List<LogItem> mLogs = new List<LogItem>();
        private CacheList<LogType> mLogTypeFilters;

        public bool CompareTag(string tag) {
            if(tagCommandFilters == null || tagCommandFilters.Length == 0)
                return true;

            for(int i = 0; i < tagCommandFilters.Length; i++) {
                if(tagCommandFilters[i] == tag)
                    return true;
            }

            return false;
        }

        public void SetLogFilter(LogType logType, bool active) {
            bool isChanged = false;

            if(active) {
                bool isFound = false;
                for(int i = 0; i < mLogTypeFilters.Count; i++) {
                    if(mLogTypeFilters[i] == logType) {
                        isFound = true;
                        break;
                    }
                }
                if(!isFound) {
                    mLogTypeFilters.Add(logType);
                    isChanged = true;
                }
            }
            else
                isChanged = mLogTypeFilters.Remove(logType);

            if(isChanged)
                Refresh();
        }

        public void Refresh() {
            //clean up logs based on filters
            var newLogs = new List<LogItem>();

            for(int i = 0; i < mLogs.Count; i++) {
                var log = mLogs[i];

                if(log.tag != tagCommand) {
                    if(!mIsLogEnabled)
                        continue;

                    if(!IsLogTypeAllowed(log.type))
                        continue;
                }

                newLogs.Add(log);
            }

            mLogs = newLogs;

            RefreshInvoke();
        }

        public void AddCommand(MethodInfo method) {
            var newCommand = new Command(method);
            if(mCommands.ContainsKey(newCommand.name))
                mCommands[newCommand.name] = newCommand; //overwrite existing command
            else
                mCommands.Add(newCommand.name, newCommand);
        }

        public void AddCommand(MethodInfo method, string alias, string hint) {
            var newCommand = new Command(method, alias, hint);
            if(mCommands.ContainsKey(newCommand.name))
                mCommands[newCommand.name] = newCommand; //overwrite existing command
            else
                mCommands.Add(newCommand.name, newCommand);
        }

        public void Execute(string line) {
            if(string.IsNullOrEmpty(line))
                return;

            string commandName;
            string parmLine = "";

            //grab first word
            int spaceInd = line.IndexOf(' ');
            if(spaceInd != -1) {
                commandName = line.Substring(0, spaceInd);

                if(spaceInd < line.Length - 1)
                    parmLine = line.Substring(spaceInd + 1, line.Length - spaceInd - 1);
            }
            else {
                commandName = line;
            }

            var command = GetCommand(commandName);
            if(command != null) {
                if(string.IsNullOrEmpty(parmLine) && command.parms.Length > 0) {
                    Log(command.logHint);
                }
                else {
                    object[] objParms;
                    if(ConsoleParam.Parse(command.parms, parmLine, out objParms))
                        command.Execute(objParms);
                    else
                        Log(LogType.Log, tagCommand, command.logHint);
                }
            }
            else
                Log(LogType.Log, tagCommand, "Unknown Command: " + commandName);
        }

        public bool IsLogTypeAllowed(LogType logType) {
            return mLogTypeFilters.Exists(logType);
        }

        public void Clear() {
            mLogs.Clear();
            RefreshInvoke();
        }

        public void Log(LogType logType, string tag, object message) {
            if(tag != tagCommand) {
                if(!mIsLogEnabled)
                    return;

                if(!IsLogTypeAllowed(logType))
                    return;
            }

            //check if we need to remove last item
            if(mLogs.Count == maxLog)
                mLogs.RemoveAt(0);

            mLogs.Add(new LogItem() { type = logType, tag = tag, text = message != null ? message.ToString() : "" });

            RefreshInvoke();
        }

        public void Log(LogType logType, string tag, object message, Object context) {
            var sb = new System.Text.StringBuilder();
            sb.Append(context.name).Append(": ");
            if(message != null)
                sb.Append(message.ToString());

            Log(logType, tag, sb.ToString());
        }

        public void Log(object message) {
            Log(LogType.Log, message);
        }

        public void Log(string tag, object message) {
            Log(LogType.Log, tag, message);
        }

        public void Log(string tag, object message, Object context) {
            Log(LogType.Log, tag, message, context);
        }

        public void Log(LogType logType, object message, Object context) {
            Log(logType, "", message, context);
        }

        public void Log(LogType logType, object message) {
            Log(logType, "", message);
        }
                
        public void LogError(string tag, object message) {
            Log(LogType.Log, tag, message);
        }

        public void LogError(string tag, object message, Object context) {
            Log(LogType.Error, tag, message, context);
        }

        public void LogException(System.Exception exception) {
            Log(LogType.Exception, "", exception);
        }

        public void LogFormat(LogType logType, string format, params object[] args) {
            Log(logType, "", string.Format(format, args));
        }

        public void LogWarning(string tag, object message, Object context) {
            Log(LogType.Warning, tag, message, context);
        }

        public void LogWarning(string tag, object message) {
            Log(LogType.Warning, tag, message);
        }

        public void LogException(System.Exception exception, Object context) {
            Log(LogType.Exception, "", exception, context);
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args) {
            Log(logType, "", string.Format(format, args), context);
        }

        void OnEnable() {
            mIsLogEnabled = defaultLogEnabled;

            mIsUnityLogEnabled = defaultUnityLogEnabled;
            if(mIsUnityLogEnabled)
                Application.logMessageReceived += OnUnityLog;

            mLogTypeFilters = new CacheList<LogType>(System.Enum.GetValues(typeof(LogType)).Length);
            for(int i = 0; i < defaultLogTypeFilters.Length; i++)
                SetLogFilter(defaultLogTypeFilters[i], true);

            //fill in commands from ConsoleClassRegisterAttribute
            RegisterCommandsFromClasses();
        }

        void OnDisable() {
            if(mIsUnityLogEnabled)
                Application.logMessageReceived -= OnUnityLog;

            mCommands.Clear();
            mLogs.Clear();

            refreshCallback = null;
        }

        void OnUnityLog(string logString, string stackTrace, LogType type) {
            var sb = new System.Text.StringBuilder();
            sb.Append(logString).Append('\n').Append(stackTrace);
            Log(type, sb.ToString());
        }

        private Command GetCommand(string name) {
            Command comm;
            mCommands.TryGetValue(name, out comm);
            return comm;
        }

        private void RefreshInvoke() {
            if(signalRefresh)
                signalRefresh.Invoke(this);

            if(refreshCallback != null)
                refreshCallback();
        }

        private void RegisterCommandsFromClasses() {
            foreach(var assembly in System.AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(var type in assembly.GetTypes()) {
                    var registerAttr = System.Attribute.GetCustomAttribute(type, typeof(ConsoleCommandClassAttribute), true) as ConsoleCommandClassAttribute;
                    if(registerAttr != null) {
                        //check tag filter
                        if(!string.IsNullOrEmpty(registerAttr.tag) && !CompareTag(registerAttr.tag))
                            continue;

                        //grab "public static funcs"
                        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                        foreach(var method in methods) {
                            var commandAttr = System.Attribute.GetCustomAttribute(method, typeof(ConsoleCommandAttribute), true) as ConsoleCommandAttribute;
                            if(commandAttr != null) {
                                //check tag filter
                                if(!string.IsNullOrEmpty(commandAttr.tag) && !CompareTag(commandAttr.tag))
                                    continue;

                                AddCommand(method, commandAttr.alias, commandAttr.hint);
                            }
                            else
                                AddCommand(method, "", "");
                        }
                    }
                }
            }
        }
    }
}