using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace M8 {
    [CreateAssetMenu(fileName = "console", menuName = "Console")]
    public class Console : ScriptableObject, ILogger {
        public const string tagCommand = "command";

        public struct LogItem {
            public LogType type;
            public string tag;
            public string text;
        }

        [Header("Config")]        
        public string[] tags; //add tags to allow Console to only include specific commands
        public int maxLine = 512;

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

        public bool CompareTag(string tag) {
            if(tags == null || tags.Length == 0)
                return true;

            for(int i = 0; i < tags.Length; i++) {
                if(tags[i] == tag)
                    return true;
            }

            return false;
        }

        public void Refresh() {

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
                Log(LogType.Log, tagCommand, "Unknown Command: " + command);
        }

        public bool IsLogTypeAllowed(LogType logType) {
            return true;
        }

        public void Log(LogType logType, string tag, object message) {
            if(!mIsLogEnabled)
                return;

            //check if we need to remove last item
            if(mLogs.Count == maxLine) {
                mLogs.RemoveAt(0);

                //callback
            }

            mLogs.Add(new LogItem() { type = logType, tag = tag, text = message != null ? message.ToString() : "" });

            //callback
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
            if(mIsUnityLogEnabled)
                Application.logMessageReceived += OnUnityLog;

            //fill in commands from ConsoleClassRegisterAttribute
            RegisterCommandsFromClasses();
        }

        void OnDisable() {
            if(mIsUnityLogEnabled)
                Application.logMessageReceived -= OnUnityLog;

            mCommands.Clear();
            mLogs.Clear();
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

        private void RegisterCommandsFromClasses() {
            foreach(var assembly in System.AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(var type in assembly.GetTypes()) {
                    var registerAttr = System.Attribute.GetCustomAttribute(type, typeof(ConsoleClassRegisterAttribute), true) as ConsoleClassRegisterAttribute;
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