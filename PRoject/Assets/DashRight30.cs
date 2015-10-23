using UnityEngine;
using System.Collections;

public class DashRight30 : AbilityFSM
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
		Ability faceRight30 = abilityDatabase.GetAbility("FaceGameObjectRight30");
		faceRight30.execute(gameObject, null);
		yield return null;
	}
	
	// Update is called once per frame
	public void TurningUpdate () 
	{
		FaceGameObjectRight30 faceGORight30 = memoizer.GetMemoizedComponent<FaceGameObjectRight30>(gameObject);
		faceGORight30.gameObjectToFace = gameObjectToDashTo;
		
		if (faceGORight30.currentState.Equals(FaceGameObjectRight30.states.Finish))
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
	
	//Set dummy position to user transform, apply rotation to dummy, test its dash
	public static bool Test (GameObject user, GameObject target)
	{
		GameObject dummy = Utilities.dummy;
		dummy.transform.position = user.transform.position;
		dummy.transform.forward = user.transform.forward;
		FaceGameObjectRight30.CommitToDummy(dummy, target);
		return Dash.Test(dummy, target);
	}
	
	public static GameObject CommitToDummy (GameObject dummy, GameObject target)
	{
		FaceGameObjectRight30.CommitToDummy(dummy, target);
		return Dash.CommitToDummy(dummy, target);
	}
}
