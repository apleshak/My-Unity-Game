using UnityEngine;
using System.Collections;

public class DashLeft30 : AbilityFSM
{
	//Animator animator;
	public GameObject gameObjectToDashTo;
	
	public enum states
	{
		Turning,
		Dashing,
		Finish
	};
	
	// Use this for initialization
	void Start () 
	{
		currentState = states.Turning;
	}
	
	public IEnumerator TurningEnterState ()
	{
		Ability faceLeft30 = abilityDatabase.GetAbility("FaceGameObjectLeft30");
		faceLeft30.execute(gameObject, null);
		yield return null;
	}
	
	// Update is called once per frame
	public void TurningUpdate () 
	{
		FaceGameObjectLeft30 faceGOLeft30 = memoizer.GetMemoizedComponent<FaceGameObjectLeft30>(gameObject);
		faceGOLeft30.gameObjectToFace = gameObjectToDashTo;
		
		if (faceGOLeft30.currentState.Equals(FaceGameObjectLeft30.states.Finish))
			currentState = states.Dashing;
	}
	
	public IEnumerator DashingEnterState ()
	{
		Ability dash = abilityDatabase.GetAbility("Dash");
		dash.execute(gameObject, null);
		yield return null;
	}
	
	public void DashingUpdate ()
	{
		Dash dash = memoizer.GetMemoizedComponent<Dash>(gameObject);
		
		if (dash.currentState.Equals(Dash.states.Finish))
			currentState = states.Finish;
	}
	
	public void FinishUpdate () 
	{
		TerminateFSM();
	}
	
	public static bool Test (GameObject user, GameObject target)
	{
		GameObject dummy = Utilities.dummy;
		dummy.transform.position = user.transform.position;
		dummy.transform.forward = user.transform.forward;
		FaceGameObjectLeft30.CommitToDummy(dummy, target);
		return Dash.Test(dummy, target);
	}
	
	public static GameObject CommitToDummy (GameObject dummy, GameObject target)
	{
		FaceGameObjectLeft30.CommitToDummy(dummy, target);
		return Dash.CommitToDummy(dummy, target);
	}
}
