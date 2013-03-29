using UnityEngine;

//for use with the sequencer
public abstract class SequencerCriteria {
	public Criteria.Eval eval = Criteria.Eval.True;
	
	//implements
	
	public abstract bool Evaluate(MonoBehaviour behaviour, SequencerInstance instance);
}
