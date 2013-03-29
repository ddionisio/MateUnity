using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SequencerState {
	public Sequencer.StateData[] sequences;
	public string sequenceDefaultState;
	
	private string mCurState;
	private Dictionary<string, Sequencer> mSequences;
	
	public string curState {
		get {
			return mCurState;
		}
	}
	
	public void Load() {
		mSequences = Sequencer.Load(sequences);
	}
	
	public void Start(MonoBehaviour behaviour, SequencerInstance instance, string stateName) {
		if(stateName == null) {
			stateName = sequenceDefaultState;
		}
		
		mCurState = stateName;
		
		if(!string.IsNullOrEmpty(stateName)) {
			if(mSequences != null) {
				
				Sequencer seq;
				if(mSequences.TryGetValue(stateName, out seq)) {
					behaviour.StartCoroutine(seq.Go(instance, behaviour));
				}
				else {
					Debug.LogError("State not found: "+stateName, behaviour);
				}
			}
		}
	}
}