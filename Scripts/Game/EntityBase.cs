//#define PLAYMAKER
//#define POOLMANAGER

using UnityEngine;
using System.Collections;

#if PLAYMAKER
using HutongGames.PlayMaker;
#endif

[AddComponentMenu("M8/Entity/EntityBase")]
public class EntityBase : MonoBehaviour {
    public const int StateInvalid = -1;

    public delegate void OnSetState(EntityBase ent, int state);
    public delegate void OnSetBool(EntityBase ent, bool b);
    public delegate void OnFinish(EntityBase ent);

    public float spawnDelay = 0.1f;

    public bool activateOnStart = false; //if we want FSM/other stuff to activate on start (when placing entities on scene)

    public event OnSetState setStateCallback;
    public event OnSetBool setBlinkCallback;
    public event OnFinish spawnCallback;
    public event OnFinish releaseCallback;

    private int mState = StateInvalid;
    private int mPrevState = StateInvalid;

#if PLAYMAKER
    private PlayMakerFSM mFSM;
#endif

    private EntityActivator mActivator = null;

    private float mBlinkCurTime = 0;
    private float mBlinkDelay = 0;

    private bool mDoSpawnOnWake = false;

    private PoolDataController mPoolData;

    public static T Spawn<T>(string spawnGroup, string typeName, Vector3 position) where T : EntityBase {
        //TODO: use ours if no 3rd party pool manager
#if POOLMANAGER
        SpawnPool pool = PoolManager.Pools[spawnGroup];

        Transform spawned = pool.Spawn(pool.prefabs[typeName]);
        T ent = spawned != null ? spawned.GetComponent<T>() : null;

        if(ent != null) {
            ent.transform.position = position;

            //add pool data controller
        }

        return ent;
#else
        Transform spawned = PoolController.Spawn(spawnGroup, typeName, typeName, null, null);
        T ent = spawned != null ? spawned.GetComponent<T>() : null;

        if(ent != null) {
            ent.transform.position = position;
        }

        return null;
#endif
    }

    public string spawnType {
        get {
            if(mPoolData == null) {
                return name;
            }

            return mPoolData.factoryKey; 
        }
    }

    /// <summary>
    /// Set by call from Spawn, so use that instead of directly spawning via pool manager.
    /// </summary>
    public string spawnGroup {
        get { return mPoolData == null ? "" : mPoolData.group; }
    }

    ///<summary>entity is instantiated with activator set to disable on start, call spawn after activator sends a wakeup call.</summary>
    public bool doSpawnOnWake {
        get { return mDoSpawnOnWake; }
    }

#if PLAYMAKER
    public PlayMakerFSM FSM {
        get { return mFSM; }
    }
#endif

    public int state {
        get { return mState; }

        set {
            if(mState != value) {
                if(mState != StateInvalid)
                    mPrevState = mState;

                mState = value;

                if(setStateCallback != null) {
                    setStateCallback(this, value);
                }

                StateChanged();
            }
        }
    }

    public int prevState {
        get { return mPrevState; }
    }

    public bool isReleased {
        get {
            if(mPoolData == null)
                return false;

#if POOLMANAGER
            return !PoolManager.Pools[mPoolData.group].IsSpawned(transform);
#else
            return mPoolData.claimed;
#endif
        }
    }

    public bool isBlinking {
        get { return mBlinkDelay > 0 && mBlinkCurTime < mBlinkDelay; }
    }

    public void Blink(float delay) {
        mBlinkDelay = delay;
        mBlinkCurTime = 0;

        bool doBlink = delay > 0;

        if(setBlinkCallback != null) {
            setBlinkCallback(this, doBlink);
        }

        SetBlink(doBlink);
    }

    public void Release() {
        if(mPoolData != null) {
#if POOLMANAGER
            transform.parent = PoolManager.Pools[mPoolData.group].group;
            PoolManager.Pools[mPoolData.group].Despawn(transform);
#else
            PoolController.Release(mPoolData.group, transform);
#endif
        }
        else {
            StartCoroutine(DestroyDelay());
        }
    }

    /// <summary>
    /// This is to tell the entity that spawning has finished. Use this to start any motion, etc.
    /// </summary>
    public virtual void SpawnFinish() {
    }

    protected virtual void OnDespawned() {
#if PLAYMAKER
        if(mFSM != null) {
            mFSM.enabled = false;
        }
#endif

        if(mActivator != null) {
            mActivator.Release(false);
        }

        if(releaseCallback != null) {
            releaseCallback(this);
        }

        mDoSpawnOnWake = false;

        StopAllCoroutines();
    }

    protected virtual void ActivatorWakeUp() {
        if(mDoSpawnOnWake) { //if we haven't properly spawned yet, do so now
            mDoSpawnOnWake = false;
            StartCoroutine(DoSpawn());
        }
#if PLAYMAKER
        else if(mFSM != null) {
            //resume FSM
            mFSM.Fsm.Event(EntityEvent.Wake);
        }
#endif
    }

    protected virtual void ActivatorSleep() {
#if PLAYMAKER
        if(mFSM != null) {
            mFSM.Fsm.Event(EntityEvent.Sleep);
        }
#endif
    }

    protected virtual void OnDestroy() {
        if(mActivator != null) {
            mActivator.Release(true);
        }

        setStateCallback = null;
        setBlinkCallback = null;
        spawnCallback = null;
        releaseCallback = null;
    }

    protected virtual void Awake() {
        mPoolData = GetComponent<PoolDataController>();

        mActivator = GetComponentInChildren<EntityActivator>();
        if(mActivator != null) {
            mActivator.awakeCallback += ActivatorWakeUp;
            mActivator.sleepCallback += ActivatorSleep;
        }

#if PLAYMAKER
        //only start once we spawn
        mFSM = GetComponentInChildren<PlayMakerFSM>();
        if(mFSM != null) {
            mFSM.Fsm.RestartOnEnable = false; //not when we want to sleep/wake
            mFSM.enabled = false;
        }
#endif
    }

    // Use this for initialization
    protected virtual void Start() {
        BroadcastMessage("EntityStart", this, SendMessageOptions.DontRequireReceiver);

        //for when putting entities on scene, skip the spawning state
        if(activateOnStart) {
            StartCoroutine(DoStart());
        }
    }

    protected virtual void StateChanged() {
    }

    protected virtual void SetBlink(bool blink) {
    }

    protected virtual void SpawnStart() {
    }

    //////////internal

    // Update is called once per frame
    private void Update() {
        if(mBlinkDelay > 0) {
            mBlinkCurTime += Time.deltaTime;
            if(mBlinkCurTime >= mBlinkDelay) {
                mBlinkDelay = mBlinkCurTime = 0;

                if(setBlinkCallback != null) {
                    setBlinkCallback(this, false);
                }

                SetBlink(false);
            }
        }
    }

    /// <summary>
    /// Spawn this entity, resets stats, set action to spawning, then later calls OnEntitySpawnFinish.
    /// NOTE: calls after an update to ensure Awake and Start is called.
    /// </summary>
    void OnSpawned() {
        if(mPoolData == null)
            mPoolData = GetComponent<PoolDataController>();

        mState = mPrevState = StateInvalid; //avoid invalid updates

        //allow activator to start and check if we need to spawn now or later
        //ensure start is called before spawning if we are freshly allocated from entity manager
        if(mActivator != null) {
            mActivator.Start();

            if(mActivator.deactivateOnStart) {
                mDoSpawnOnWake = true; //do it later when we wake up
            }
            else {
                StartCoroutine(DoSpawn());
            }
        }
        else {
            StartCoroutine(DoSpawn());
        }
    }

    IEnumerator DoStart() {
#if PLAYMAKER
        if(mFSM != null) {
            mFSM.enabled = true;

            yield return new WaitForFixedUpdate();

            SpawnStart();

            if(spawnCallback != null) {
                spawnCallback(this);
            }

            mFSM.SendEvent(EntityEvent.Start);
        }
        else {
            yield return new WaitForFixedUpdate();

            SpawnStart();

            if(spawnCallback != null) {
                spawnCallback(this);
            }
        }
#else
        yield return new WaitForFixedUpdate();

        SpawnStart();

        if(spawnCallback != null) {
            spawnCallback(this);
        }
#endif

        yield break;
    }

    IEnumerator DoSpawn() {
#if PLAYMAKER
        //start up
        if(mFSM != null) {
            //restart
            mFSM.Fsm.Reinitialize();
            mFSM.enabled = true;
                        
            //allow fsm to boot up, then tell it to spawn
            yield return new WaitForFixedUpdate();

            SpawnStart();

            if(spawnCallback != null) {
                spawnCallback(this);
            }

            mFSM.SendEvent(EntityEvent.Spawn);
        }
        else {
             yield return new WaitForFixedUpdate();

            SpawnStart();

            if(spawnCallback != null) {
                spawnCallback(this);
            }

            yield return new WaitForSeconds(spawnDelay);

            SpawnFinish();
        }
#else
         yield return new WaitForFixedUpdate();

        SpawnStart();

        if(spawnCallback != null) {
            spawnCallback(this);
        }

        yield return new WaitForSeconds(spawnDelay);

        SpawnFinish();
#endif

        yield break;
    }

    IEnumerator DestroyDelay() {
        yield return new WaitForFixedUpdate();

        GameObject.Destroy(gameObject);

        yield break;
    }
}
