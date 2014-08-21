using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Serializer/Transform")]
[RequireComponent(typeof(SceneSerializer))]
public class SceneSerializerTransform : MonoBehaviour {
    [System.Flags]
    public enum Axis {
        X=0x1,
        Y=0x2,
        Z=0x4
    }

    public bool isLocal;
    public bool persistent;

    public Axis positionFlags = Axis.X | Axis.Y | Axis.Z;
    public Axis rotationFlags = Axis.X | Axis.Y | Axis.Z;

    private SceneSerializer mSerializer;

    void OnDestroy() {
        if(SceneManager.instance)
            SceneManager.instance.sceneChangeCallback -= OnSceneLoad;
        if(persistent && UserData.instance)
            UserData.instance.actCallback -= OnUserDataAction;
    }

    void Awake() {
        if(SceneManager.instance)
            SceneManager.instance.sceneChangeCallback += OnSceneLoad;
        if(persistent && UserData.instance)
            UserData.instance.actCallback += OnUserDataAction;

        mSerializer = GetComponent<SceneSerializer>();
    }

    void Start() {
        _Load();
    }

    void OnSceneLoad(string nextScene) {
        //save
        _Save();
    }

    void OnUserDataAction(UserData ud, UserData.Action act) {
        switch(act) {
            case UserData.Action.Load:
                _Load();
                break;
            case UserData.Action.Save:
                _Save();
                break;
        }
    }

    void _Load() {
        Vector3 pos = isLocal ? transform.localPosition : transform.position;
        if((positionFlags & Axis.X) != 0) pos.x = mSerializer.GetValueFloat("px", pos.x);
        if((positionFlags & Axis.Y) != 0) pos.y = mSerializer.GetValueFloat("py", pos.y);
        if((positionFlags & Axis.Z) != 0) pos.z = mSerializer.GetValueFloat("pz", pos.z);

        Vector3 rot = isLocal ? transform.localEulerAngles : transform.eulerAngles;
        if((rotationFlags & Axis.X) != 0) rot.x = mSerializer.GetValueFloat("rx", rot.x);
        if((rotationFlags & Axis.Y) != 0) rot.y = mSerializer.GetValueFloat("ry", rot.y);
        if((rotationFlags & Axis.Z) != 0) rot.z = mSerializer.GetValueFloat("rz", rot.z);

        if(isLocal) {
            transform.localPosition = pos;
            transform.localEulerAngles = rot;
        }
        else {
            transform.position = pos;
            transform.eulerAngles = rot;
        }
    }

    void _Save() {
        Vector3 pos = isLocal ? transform.localPosition : transform.position;
        if((positionFlags & Axis.X) != 0) mSerializer.SetValueFloat("px", pos.x, persistent);
        if((positionFlags & Axis.Y) != 0) mSerializer.SetValueFloat("py", pos.y, persistent);
        if((positionFlags & Axis.Z) != 0) mSerializer.SetValueFloat("pz", pos.z, persistent);

        Vector3 rot = isLocal ? transform.localEulerAngles : transform.eulerAngles;
        if((rotationFlags & Axis.X) != 0) mSerializer.SetValueFloat("rx", rot.x, persistent);
        if((rotationFlags & Axis.Y) != 0) mSerializer.SetValueFloat("ry", rot.y, persistent);
        if((rotationFlags & Axis.Z) != 0) mSerializer.SetValueFloat("rz", rot.z, persistent);
    }
}
