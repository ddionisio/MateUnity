using UnityEngine;
using System.Collections;

public class SequencerAction  {
	public float delay;
	
	//store any non-readonly fields to given behaviour, don't put them here, store them in behaviour
	//these sequence actions can be shared by different behaviours
	//can set states dependent from outside here
	public virtual void Start(MonoBehaviour behaviour, SequencerInstance instance) {
	}
	
	/// <summary>
	/// Periodic update, return true if done.
	/// </summary>
	public virtual bool Update(MonoBehaviour behaviour, SequencerInstance instance) {
		return true;
	}
	
	//do clean ups here, don't set any states dependent from outside
	public virtual void Finish(MonoBehaviour behaviour, SequencerInstance instance) {
	}
}

//some basic sequencer actions

//change to an entire new sequencer, make sure to implement SequenceChangeState
public class SequencerActionToState : SequencerAction {
	public string state = "";
	
	public override void Start(MonoBehaviour behaviour, SequencerInstance instance) {
		behaviour.SendMessage("SequenceChangeState", state, SendMessageOptions.RequireReceiver);
	}
}

//simple action sequence to change to a new phase, ideally add to the end for chaining phases
public class SequencerActionToPhase : SequencerAction {
	public string phase = "";
	
	public override void Finish(MonoBehaviour behaviour, SequencerInstance instance) {
		instance.toPhase = phase;
	}
}

