using UnityEngine;
using System.Collections;

using System;
using System.IO;

[AddComponentMenu("M8/Core/UserFileData")]
public class UserFileData : UserData {
    public int version = 0;
    public string fileHeader = "MATE";
    public string ext = "sav";
    public string folder = "Save";

    public string loadOnStartName;

    public void LoadFile(string name) {
        if(mKey != name) {
            if(!string.IsNullOrEmpty(mKey))
                Delete();

            mKey = name;

            Load();
        }
    }

    protected override void LoadOnStart() {
        if(!string.IsNullOrEmpty(loadOnStartName))
            LoadFile(loadOnStartName);
    }

    protected override byte[] LoadRawData() {
        if(!string.IsNullOrEmpty(mKey)) {
            string folderPath = string.Format("{0}/{1}", Application.persistentDataPath, folder);

            if(Directory.Exists(folderPath)) {
                string path = string.Format("{0}/{1}.{2}", folderPath, mKey, ext);
                if(File.Exists(path)) {
                    using(BinaryReader bs = new BinaryReader(File.Open(path, FileMode.Open), System.Text.Encoding.UTF8)) {
                        string saveHeader = bs.ReadString();
                        if(saveHeader != fileHeader) {
                            Debug.LogError("Invalid file: "+path+" header: "+saveHeader);
                            return null;
                        }

                        short saveVer = bs.ReadInt16();
                        if(saveVer != version) {
                            //TODO: call backwards compatibility interface
                            Debug.LogError("Invalid version: "+saveVer+" file: "+path);
                            return null;
                        }

                        int size = bs.ReadInt32();
                        return bs.ReadBytes(size);
                    }
                }
            }
        }

        return null;
    }

    protected override void SaveRawData(byte[] dat) {
        if(!string.IsNullOrEmpty(mKey)) {
            string folderPath = string.Format("{0}/{1}", Application.persistentDataPath, folder);

            if(!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string path = string.Format("{0}/{1}.{2}", folderPath, mKey, ext);

            using(BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create), System.Text.Encoding.UTF8)) {
                bw.Write(fileHeader);
                bw.Write((short)version);
                bw.Write(dat.Length);
                bw.Write(dat);
            }
        }
    }

    protected override void DeleteRawData() {
        if(!string.IsNullOrEmpty(mKey)) {
            string folderPath = string.Format("{0}/{1}", Application.persistentDataPath, folder);

            if(Directory.Exists(folderPath)) {
                string path = string.Format("{0}/{1}.{2}", folderPath, mKey, ext);
                File.Delete(path);
            }
        }
        mKey = "";
    }

    protected override void Awake() {
        mKey = "";
        base.Awake();
    }
}
