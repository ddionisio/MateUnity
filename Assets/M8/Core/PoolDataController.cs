using UnityEngine;
using System.Collections;

public class PoolDataController : MonoBehaviour {
	[System.NonSerialized]
	public string factoryKey;
	
	/// <summary>
	/// only entity manager ought to set this.
	/// </summary>
	[System.NonSerialized]
	public bool claimed;
}
