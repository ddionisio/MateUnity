using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This is a script used to keep the game object scaled to 2/(Screen.height).
/// simpler version of UIRoot without the update, make sure to refresh when screen size changes
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("M8/NGUI/RootLocal")]
public class NGUIRootLocal : MonoBehaviour
{
	Transform mTrans;

	/// <summary>
	/// If set to 'true', the UIRoot will be scaled automatically as the resolution changes, and will keep the UI under it
	/// pixel-perfect. If set to 'false', 'manualHeight' is used instead, and the UI will scale proportionally to screen's height,
	/// always remaining the same relative size regardless of the resolution changes.
	/// </summary>

	public bool automatic = true;

	/// <summary>
	/// Height of the screen when 'automatic' is turned off.
	/// </summary>

	public int manualHeight = 800;

	/// <summary>
	/// If the screen height goes below this value, it will be as if 'automatic' is turned off with the 'manualHeight' set to this value.
	/// </summary>

	public int minimumHeight = 320;

	/// <summary>
	/// If the screen height goes above this value, it will be as if 'automatic' is turned off with the 'manualHeight' set to this value.
	/// </summary>

	public int maximumHeight = 1536;

	/// <summary>
	/// UI Root's active height, based on the size of the screen.
	/// </summary>

	public int activeHeight
	{
		get
		{
			int height = Mathf.Max(2, Screen.height);

			if (automatic)
			{
				if (height < minimumHeight) return minimumHeight;
				if (height > maximumHeight) return maximumHeight;
				return height;
			}
			return manualHeight;
		}
	}

	/// <summary>
	/// Pixel size adjustment. Most of the time it's at 1, unless 'automatic' is turned off.
	/// </summary>

	public float pixelSizeAdjustment
	{
		get
		{
			float height = Screen.height;

			if (automatic)
			{
				if (height < minimumHeight) return minimumHeight / height;
				if (height > maximumHeight) return maximumHeight / height;
				return 1f;
			}
			return manualHeight / height;
		}
	}
	
	void OnEnable() {
		if(mTrans == null) mTrans = transform;
		
		UpdateScale();
	}
	
	void OnSceneScreenChanged() {
		UpdateScale();
	}
	
#if UNITY_EDITOR
	void Update() {
		UpdateScale();
	}
#endif

	private void UpdateScale()
	{
		if (mTrans != null)
		{
			float calcActiveHeight = activeHeight;

			if (calcActiveHeight > 0f )
			{
				float size = 2f / calcActiveHeight;
				
				Vector3 ls = mTrans.localScale;
	
				if (!(Mathf.Abs(ls.x - size) <= float.Epsilon) ||
					!(Mathf.Abs(ls.y - size) <= float.Epsilon) ||
					!(Mathf.Abs(ls.z - size) <= float.Epsilon))
				{
					mTrans.localScale = new Vector3(size, size, 1.0f);
				}
			}
		}
	}
}