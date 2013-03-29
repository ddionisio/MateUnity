using UnityEngine;

public class SequencerInstance {
	public bool pause=false;
	public bool terminate=false;
	public int counter=0;
	public float startTime=0; //the time when start happened
	
	//set this to change phase after current action is finish
	//(skips evaluation and breaks current phase)
	public string toPhase=null;
	
	//set this to true if you want to break the current phase and go back to evaluation
	public bool breakPhase=false;
	
	public Object holder; //put whatever you want
	
	public bool IsDelayReached(float delay) {
		return Time.time >= startTime + delay;
	}
}
