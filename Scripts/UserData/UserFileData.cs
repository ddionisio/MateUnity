using UnityEngine;
using System.Collections;

using System;
using System.IO;

namespace M8 {
    [CreateAssetMenu(fileName = "userDataFile", menuName = "M8/UserData/From File")]
    public class UserFileData : UserData {
        public const string fileHeader = "MATE";

        public int version = 0;

        public string path = "Save/user.sav";

        public void Load(string path) {
            Unload();

            this.path = path;

            Load();
        }

        protected override byte[] LoadRawData() {
            if(!string.IsNullOrEmpty(path)) {
                string absolutePath = string.Format("{0}/{1}", Application.persistentDataPath, path);
                if(File.Exists(absolutePath)) {
                    using(BinaryReader bs = new BinaryReader(File.Open(absolutePath, FileMode.Open), System.Text.Encoding.UTF8)) {
                        string saveHeader = bs.ReadString();
                        if(saveHeader != fileHeader) {
                            Debug.LogError("Invalid file: " + absolutePath + " header: " + saveHeader);
                            return null;
                        }

                        short saveVer = bs.ReadInt16();
                        if(saveVer != version) {
                            //TODO: call backwards compatibility interface
                            Debug.LogError("Invalid version: " + saveVer + " file: " + absolutePath);
                            return null;
                        }

                        int size = bs.ReadInt32();
                        return bs.ReadBytes(size);
                    }
                }
            }

            return null;
        }

        protected override void SaveRawData(byte[] dat) {
            if(!string.IsNullOrEmpty(path)) {
                var pathSb = new System.Text.StringBuilder();
                pathSb.Append(Application.persistentDataPath);

                var dirs = path.Split('/');

                //create the folders if they don't exist, build pathSb along the way
                for(int i = 0; i < dirs.Length - 1; i++) {
                    pathSb.Append('/').Append(dirs[i]);

                    var curDirPath = pathSb.ToString();
                    if(!Directory.Exists(curDirPath))
                        Directory.CreateDirectory(curDirPath);
                }

                pathSb.Append('/').Append(dirs[dirs.Length - 1]);

                string absolutePath = pathSb.ToString();

                using(BinaryWriter bw = new BinaryWriter(File.Open(absolutePath, FileMode.Create), System.Text.Encoding.UTF8)) {
                    bw.Write(fileHeader);
                    bw.Write((short)version);
                    bw.Write(dat.Length);
                    bw.Write(dat);
                }
            }
        }

        protected override void DeleteRawData() {
            if(!string.IsNullOrEmpty(path)) {
                string absolutePath = string.Format("{0}/{1}", Application.persistentDataPath, path);
                File.Delete(absolutePath);
            }
        }
    }
}