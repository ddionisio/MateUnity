using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public enum ConsoleParamType {
        Unsupported = -1,

        Integer,
        UnsignedInteger,
        Long,
        UnsignedLong,
        Float,
        Double,
        Vector2,
        Vector3,
        Vector4,
        Color,
        String,
        Char,
        Boolean,
    }

    public struct ConsoleParam {
        public static ConsoleParamType[] GenerateParams(System.Reflection.MethodInfo method) {
            var parmInfs = method.GetParameters();

            var consoleParms = new ConsoleParamType[parmInfs.Length];

            for(int i = 0; i < parmInfs.Length; i++) {
                var parmType = parmInfs[i].ParameterType;

                var consoleParmType = ConsoleParamType.Unsupported;

                if(parmType == typeof(int))
                    consoleParmType = ConsoleParamType.Integer;
                else if(parmType == typeof(uint))
                    consoleParmType = ConsoleParamType.UnsignedInteger;
                else if(parmType == typeof(long))
                    consoleParmType = ConsoleParamType.Long;
                else if(parmType == typeof(ulong))
                    consoleParmType = ConsoleParamType.UnsignedLong;
                else if(parmType == typeof(float))
                    consoleParmType = ConsoleParamType.Float;
                else if(parmType == typeof(double))
                    consoleParmType = ConsoleParamType.Double;
                else if(parmType == typeof(Vector2))
                    consoleParmType = ConsoleParamType.Vector2;
                else if(parmType == typeof(Vector3))
                    consoleParmType = ConsoleParamType.Vector3;
                else if(parmType == typeof(Vector4))
                    consoleParmType = ConsoleParamType.Vector4;
                else if(parmType == typeof(Color))
                    consoleParmType = ConsoleParamType.Color;
                else if(parmType == typeof(string))
                    consoleParmType = ConsoleParamType.String;
                else if(parmType == typeof(char))
                    consoleParmType = ConsoleParamType.Char;
                else if(parmType == typeof(bool))
                    consoleParmType = ConsoleParamType.Boolean;

                consoleParms[i] = consoleParmType;
            }

            return consoleParms;
        }

        public static bool Parse(ConsoleParamType[] parmTypes, string line, out object[] output) {
            var parmStrings = Split(line);
            if(parmStrings.Length < parmTypes.Length) {
                Debug.LogWarningFormat("Error Parsing [Param count: {0}]: \"{1}\"", parmStrings.Length, line);
                output = null;
                return false;
            }

            output = new object[parmTypes.Length];

            for(int i = 0; i < parmTypes.Length; i++) {
                var parmString = parmStrings[i];

                switch(parmTypes[i]) {
                    case ConsoleParamType.Integer: {
                            int var;
                            if(!int.TryParse(parmString, out var))
                                return false;

                            output[i] = var;
                        }
                        break;
                    case ConsoleParamType.UnsignedInteger: {
                            uint var;
                            if(uint.TryParse(parmString, out var))
                                output[i] = var;
                            //try hex format
                            else if(parmString.StartsWith("0x", System.StringComparison.CurrentCultureIgnoreCase) || parmString.StartsWith("&H", System.StringComparison.CurrentCultureIgnoreCase)) {
                                uint.TryParse(parmString.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out var);
                                output[i] = var;
                            }
                        }
                        break;
                    case ConsoleParamType.Long: {
                            long var;
                            if(!long.TryParse(parmString, out var))
                                return false;
                            output[i] = var;
                        }
                        break;
                    case ConsoleParamType.UnsignedLong: {
                            ulong var;
                            if(ulong.TryParse(parmString, out var))
                                output[i] = var;
                            //try hex format
                            else if(parmString.StartsWith("0x", System.StringComparison.CurrentCultureIgnoreCase) || parmString.StartsWith("&H", System.StringComparison.CurrentCultureIgnoreCase)) {
                                ulong.TryParse(parmString.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out var);
                                output[i] = var;
                            }
                        }
                        break;
                    case ConsoleParamType.Float: {
                            float var;
                            if(!float.TryParse(parmString, out var))
                                return false;

                            output[i] = var;
                        }
                        break;
                    case ConsoleParamType.Double: {
                            double var;
                            if(!double.TryParse(parmString, out var))
                                return false;

                            output[i] = var;
                        }
                        break;
                    case ConsoleParamType.Vector2: {
                            //split by comma
                            var elems = parmString.Split(',');
                            if(elems.Length >= 2) {
                                float x, y;

                                float.TryParse(elems[0].Trim(), out x);
                                float.TryParse(elems[1].Trim(), out y);

                                output[i] = new Vector2(x, y);
                            }
                            else
                                output[i] = Vector2.zero;
                        }
                        break;
                    case ConsoleParamType.Vector3: {
                            //split by comma
                            var elems = parmString.Split(',');
                            if(elems.Length >= 3) {
                                float x, y, z;

                                float.TryParse(elems[0].Trim(), out x);
                                float.TryParse(elems[1].Trim(), out y);
                                float.TryParse(elems[2].Trim(), out z);

                                output[i] = new Vector3(x, y, z);
                            }
                            else
                                output[i] = Vector3.zero;
                        }
                        break;
                    case ConsoleParamType.Vector4: {
                            //split by comma
                            var elems = parmString.Split(',');
                            if(elems.Length >= 4) {
                                float x, y, z, w;

                                float.TryParse(elems[0].Trim(), out x);
                                float.TryParse(elems[1].Trim(), out y);
                                float.TryParse(elems[2].Trim(), out z);
                                float.TryParse(elems[3].Trim(), out w);

                                output[i] = new Vector4(x, y, z, w);
                            }
                            else
                                output[i] = Vector4.zero;
                        }
                        break;
                    case ConsoleParamType.Color: {
                            var elems = parmString.Split(',');
                            if(elems.Length >= 4) {
                                float r, g, b, a;

                                float.TryParse(elems[0].Trim(), out r);
                                float.TryParse(elems[1].Trim(), out g);
                                float.TryParse(elems[2].Trim(), out b);
                                float.TryParse(elems[3].Trim(), out a);

                                output[i] = new Color(r, g, b, a);
                            }
                            else if(elems.Length >= 3) {
                                float r, g, b;

                                float.TryParse(elems[0].Trim(), out r);
                                float.TryParse(elems[1].Trim(), out g);
                                float.TryParse(elems[2].Trim(), out b);

                                output[i] = new Color(r, g, b, 1f);
                            }
                            else {
                                //try hex
                                if(parmString.StartsWith("0x", System.StringComparison.CurrentCultureIgnoreCase) || parmString.StartsWith("&H", System.StringComparison.CurrentCultureIgnoreCase)) {
                                    uint var;
                                    uint.TryParse(parmString.Substring(2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out var);
                                    output[i] = new Color(((var & 0xff000000)>>24)/255f, ((var & 0x00ff0000) >> 16) / 255f, ((var & 0x0000ff00) >> 8) / 255f, (var & 0x000000ff) / 255f);
                                }
                            }
                        }
                        break;
                    case ConsoleParamType.String:
                        output[i] = parmString;
                        break;
                    case ConsoleParamType.Char:
                        output[i] = parmString.Length > 0 ? parmString[0] : '\0';
                        break;
                    case ConsoleParamType.Boolean: {
                            parmString = parmString.ToLower();
                            bool val;
                            if(bool.TryParse(parmString, out val))
                                output[i] = val;
                            else
                                output[i] = parmString != "0";
                        }
                        break;
                }
            }

            return true;
        }

        private static bool IsDelimiter(char c) {
            return c == ' ' || c == ',' || c == ';';
        }

        private static bool IsEncloseStart(char c) {
            return c == '"' || c == '(' || c == '[';
        }

        private static bool IsEncloseEndMatch(char encloseChar, char c) {
            if(encloseChar == '"' && c == '"')
                return true;
            else if(encloseChar == '(' && c == ')')
                return true;
            else if(encloseChar == '[' && c == ']')
                return true;
            return false;
        }

        private static string[] Split(string line) {
            List<string> strList = new List<string>();

            bool isEscapeFound = false; //backslash to ignore special character

            bool isEncloseFound = false;
            int startInd = -1;

            for(int i = 0; i < line.Length; i++) {
                char c = line[i];

                if(c == '\\') {
                    isEscapeFound = true;
                    continue;
                }
                else if(isEncloseFound) {
                    if(!isEscapeFound && IsEncloseEndMatch(line[startInd], c)) {
                        startInd++;
                        if(startInd < i)
                            strList.Add(line.Substring(startInd, i - startInd));
                        else
                            strList.Add("");

                        startInd = -1;
                        isEncloseFound = false;
                    }
                }
                else if(IsEncloseStart(c)) {
                    if(!isEscapeFound) {
                        isEncloseFound = true;
                        startInd = i;
                    }
                    else if(startInd == -1)
                        startInd = i;
                }
                else if(IsDelimiter(c)) {
                    if(startInd != -1) {
                        strList.Add(line.Substring(startInd, i - startInd));
                        startInd = -1;
                    }
                }
                else if(startInd == -1)
                    startInd = i;

                isEscapeFound = false;
            }

            //add last element
            if(startInd != -1 && startInd < line.Length) {
                strList.Add(line.Substring(startInd, line.Length - startInd));
            }

            return strList.ToArray();
        }
    }
}