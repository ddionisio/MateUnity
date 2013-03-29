using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fastJSON;

public class SequencerEval {
	public string phase = "";
	public List<SequencerCriteria> criterias = null;
	
	public bool Check(MonoBehaviour behaviour, SequencerInstance instance) {
		foreach(SequencerCriteria criteria in criterias) {
			if(criteria.Evaluate(behaviour, instance)) {
				return true;
			}
		}
		
		return false;
	}
}

public class SequencerPhase {
	public string name = "";
	public List<SequencerAction> actions = null;
	public bool loop = false;
}

//data to load from file
public class SequencerFile {
	public string startPhase = "";
	public List<SequencerEval> evals;
	public List<SequencerPhase> phases;
}

public class Sequencer {
	[System.Serializable]
	public class StateData {
		public string name;
		public TextAsset source;
	}
	
	private class Phase {
		public SequencerAction[] actions = null;
		public bool loop = false;
		
		public Phase(SequencerPhase dat) {
			actions = dat.actions.ToArray();
			loop = dat.loop;
		}
	}
	
	
	private string mStartPhase;
	private SequencerEval[] mEvals;
	private Dictionary<string, Phase> mPhases;
	
	
	public static Dictionary<string, Sequencer> Load(StateData[] sequences) {
		JSON.Instance.Parameters.UseExtensions = true;
		
		Dictionary<string, Sequencer> ret = new Dictionary<string, Sequencer>(sequences.Length);
		
		foreach(StateData dat in sequences) {
			if(dat.source != null) {
				//load file data
				SequencerFile sequenceFile = JSON.Instance.ToObject<SequencerFile>(dat.source.text);
				
				//construct sequencer
				ret[dat.name] = new Sequencer(sequenceFile);
			}
		}
		
		return ret;
	}
	
	public Sequencer(SequencerFile fileData) {
		mStartPhase = fileData.startPhase;
		
		mEvals = fileData.evals != null ? fileData.evals.ToArray() : new SequencerEval[0];
		
		if(fileData.phases != null) {
			mPhases = new Dictionary<string, Phase>(fileData.phases.Count);
			
			foreach(SequencerPhase phase in fileData.phases) {
				mPhases.Add(phase.name, new Phase(phase));
			}
		}
		else {
			mPhases = new Dictionary<string, Phase>(0);
		}
	}
		
	public IEnumerator Go(SequencerInstance instance, MonoBehaviour behaviour) {
		//start off with given 'toPhase' in instance or start phase
		string toPhase = !string.IsNullOrEmpty(instance.toPhase) ? instance.toPhase : mStartPhase;
		
		while(!instance.terminate) {
			//stay
			if(instance.pause) {
				yield return new WaitForFixedUpdate();
				continue;
			}
			
			Phase curPhase = null;
			
			//get phase
			if(string.IsNullOrEmpty(toPhase) || !mPhases.TryGetValue(toPhase, out curPhase)) {
				//go through evals
				foreach(SequencerEval eval in mEvals) {
					//check then try to fill phase data
					if(eval.Check(behaviour, instance)
						&& !string.IsNullOrEmpty(eval.phase) 
						&& mPhases.TryGetValue(eval.phase, out curPhase)) {
						break;
					}
				}
			}
			else {
				toPhase = null;
			}
			
			SequencerAction[] actions = curPhase != null ? curPhase.actions : null;
			
			if(actions != null) {
				int actionInd = 0;
				int len = actions.Length;
				while(!instance.terminate && actionInd < len) {
					SequencerAction action = actions[actionInd];
					
					if(action.delay > 0) {
						yield return new WaitForSeconds(action.delay);
					}
					
					//ensure nothing is started when we pause or terminate early
					while(!instance.terminate && instance.pause) {
						yield return new WaitForFixedUpdate();
					}
					
					if(instance.terminate) {
						break;
					}
					
					instance.startTime = Time.time;
					action.Start(behaviour, instance);
					
					while(!instance.terminate && !action.Update(behaviour, instance)) {
						yield return new WaitForFixedUpdate();
						
						//ensure we wait until unpaused before updating again
						//warning: state from start might change.........
						//look at this comment if shit hits the fan in game
						//however...we don't want finish or update to happen when we
						//rely on certain state to be consistent outside...
						while(!instance.terminate && instance.pause) {
							yield return new WaitForFixedUpdate();
							continue;
						}
					}
					
					action.Finish(behaviour, instance);
					
					//check to see if we want to go to next phase
					if(!string.IsNullOrEmpty(instance.toPhase)) {
						toPhase = instance.toPhase;
						break;
					}
					//just end current phase
					else if(instance.breakPhase) {
						break;
					}
					//continue
					else {
						actionInd++;
						
						//loop back or change to a new phase?
						if(actionInd == len && curPhase.loop) {
							actionInd = 0;
							yield return new WaitForFixedUpdate();
						}
					}
				}
				
				//reset action specific instance params
				instance.toPhase = null;
				instance.breakPhase = false;
			}
			
			//go back to evaluate again
			yield return new WaitForFixedUpdate();
		}
		
		yield break;
	}
}
