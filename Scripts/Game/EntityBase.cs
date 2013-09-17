//#define PLAYMAKER
//#define POOLMANAGER

using UnityEngine;
using System.Collections;

#if PLAYMAKER
using HutongGames.PlayMaker;
#endif

/*
Basic Framework would be something like this:
    protected override void OnDespawned() {
        //reset stuff here

        base.OnDespawned();
    }

    protected override void OnDestroy() {
        //dealloc here

        base.OnDestroy();
    }

    public override void SpawnFinish() {
        //start ai, player control, etc
    }

    protected override void SpawnStart() {
        //initialize some things
    }

    protected override void Awake() {
        base.Awake();

        //initialize variables
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

        //initialize variables from other sources (for communicating with managers, etc.)
    }
*/

[AddComponentMenu("M8/Entity/EntityBase")]
public class EntityBase : MonoBehaviour {
    public const int StateInvalid = -1;

    public delegate void OnSetState(EntityBase ent, int state);
    public delegate void OnSetBool(EntityBase ent, bool b);
    public delegate void OnFinish(EntityBase ent);

    public float spawnDelay = 0.1f;

    /// <summary>
    /// if we want FSM/other stuff to activate on start (when placing entities on scene).
    /// Set this to true if the entity is not gonna be spawned via Pool
    /// </summary>
    public bool activateOnStart = false; 
    
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

    private bool mDoSpawnOnWake = false;
    private bool mAutoSpawnFinish = true;

    private bool mBlinking = false;

    private byte mStartedCounter = 0;

    private SceneSerializer mSerializer = null;

    private float mBlinkTime;

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

        return ent;
#endif
    }

    private PoolDataController mPoolData;
    protected PoolDataController poolData {
        get {
            if(mPoolData == null) mPoolData = GetComponent<PoolDataController>();
            return mPoolData;
        }
    }

    public string spawnType {
        get {
            if(poolData == null) {
                return name;
            }

            return poolData.factoryKey; 
        }
    }

    /// <summary>
    /// Set by call from Spawn, so use that instead of directly spawning via pool manager.
    /// </summary>
    public string spawnGroup { get { return poolData == null ? "" : poolData.group; } }

    ///<summary>entity is instantiated with activator set to disable on start, call spawn after activator sends a wakeup call.</summary>
    public bool doSpawnOnWake {
        get { return mDoSpawnOnWake; }
    }

#if PLAYMAKER
    public PlayMakerFSM FSM {
        get { return mFSM; }
    }
#endif

    /// <summary>
    /// if true, SpawnFinish is called after SpawnStart based on spawnDelay, this is not used if there is an FSM
    /// </summary>
    public bool autoSpawnFinish {
        get { return mAutoSpawnFinish; }
        set { mAutoSpawnFinish = value; }
    }

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

                if(mFSM != null)
                    mFSM.SendEvent(EntityEvent.StateChanged);
            }
        }
    }

    public int prevState {
        get { return mPrevState; }
    }

    public bool isReleased {
        get {
#if POOLMANAGER
            return !PoolManager.Pools[mPoolData.group].IsSpawned(transform);
#else
            if(poolData == null)
                return false;

            return poolData.claimed;
#endif
        }
    }

    public bool isBlinking {
        get { return mBlinking; }
    }

    public SceneSerializer serializer {
        get { return mSerializer; }
    }

    public void Blink(float delay) {
        if(delay > 0.0f) {
            if(mBlinking)
                mBlinkTime = delay;
            else {
                mBlinkTime = delay;
                StartCoroutine(DoBlink());
            }
        }
        else {
            BlinkStateSet(false);
        }
    }

    /// <summary>
    /// Explicit call to remove entity.
    /// </summary>
    public virtual void Release() {
        if(poolData != null) {
#if POOLMANAGER
            transform.parent = PoolManager.Pools[poolData.group].group;
            PoolManager.Pools[poolData.group].Despawn(transform);
#else
            PoolController.ReleaseByGroup(poolData.group, transform);
#endif
        }
        else {
            if(gameObject.activeInHierarchy)
                StartCoroutine(DestroyDelay());
            else
                Destroy(gameObject);
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
        if(activateOnStart && mStartedCounter == 1) { //we didn't get a chance to start properly before being inactivated
            StartCoroutine(DoStart());
        }
        else if(mDoSpawnOnWake) { //if we haven't properly spawned yet, do so now
            mDoSpawnOnWake = false;
            StartCoroutine(DoSpawn());
        }
#if PLAYMAKER
        else if(mFSM != null && mStartedCounter > 0) {
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

    protected virtual void OnEnable() {
        //we didn't get a chance to start properly before being inactivated
        if((mActivator == null || mActivator.isActive) && activateOnStart && mStartedCounter == 1) {
            StartCoroutine(DoStart());
        }
    }

    protected virtual void Awake() {
        mPoolData = GetComponent<PoolDataController>();

        mSerializer = GetComponent<SceneSerializer>();

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

        mStartedCounter = 1;

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

    /// <summary>
    /// Spawn this entity, resets stats, set action to spawning, then later calls OnEntitySpawnFinish.
    /// NOTE: calls after an update to ensure Awake and Start is called.
    /// </summary>
    void OnSpawned() {
        if(mPoolData == null)
            mPoolData = GetComponent<PoolDataController>();

        mState = mPrevState = StateInvalid; //avoid invalid updates

        mBlinking = false;
        mBlinkTime = 0.0f;

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

            if(autoSpawnFinish) {
                if(spawnDelay > 0.0f) {
                    yield return new WaitForSeconds(spawnDelay);
                }

                SpawnFinish();
            }
        }
#else
        yield return new WaitForFixedUpdate();

        SpawnStart();

        if(spawnCallback != null) {
            spawnCallback(this);
        }

        if(autoSpawnFinish) {
            if(spawnDelay > 0.0f) {
                yield return new WaitForSeconds(spawnDelay);
            }

            SpawnFinish();
        }
#endif

        mStartedCounter = 2;

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

            if(autoSpawnFinish) {
                if(spawnDelay > 0.0f) {
                    yield return new WaitForSeconds(spawnDelay);
                }

                SpawnFinish();
            }
        }
#else
         yield return new WaitForFixedUpdate();

        SpawnStart();

        if(spawnCallback != null) {
            spawnCallback(this);
        }

        if(autoSpawnFinish) {
            if(spawnDelay > 0.0f) {
                yield return new WaitForSeconds(spawnDelay);
            }

            SpawnFinish();
        }
#endif

        mStartedCounter = 2;

        yield break;
    }

    IEnumerator DestroyDelay() {
        yield return new WaitForFixedUpdate();

        GameObject.Destroy(gameObject);

        yield break;
    }

    void BlinkStateSet(bool blink) {
        if(mBlinking != blink) {
            mBlinking = blink;
            if(setBlinkCallback != null)
                setBlinkCallback(this, blink);
            SetBlink(blink);
        }
    }

    IEnumerator DoBlink() {
        BlinkStateSet(true);

        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        while(mBlinkTime > 0.0f) {
            yield return wait;
            mBlinkTime -= Time.fixedDeltaTime;
        }

        BlinkStateSet(false);
    }
}
