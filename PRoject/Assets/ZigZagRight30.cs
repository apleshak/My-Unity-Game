using UnityEngine;
using System.Collections;

public class ZigZagRight30 : AbilityFSM
{
	public GameObject gameObjectToZigZagTo;
	EntitySenses senses;
	
	public enum states
	{
		DashingLeft30,
		DashingRight30,
		Finish
	};
	
	void Start () 
	{
		senses = memoizer.GetMemoizedComponentInChildren<EntitySenses>(gameObject);
		
		if (gameObjectToZigZagTo == null)
			gameObjectToZigZagTo = senses.GetTarget();
		currentState = states.DashingRight30;
	}
	
	public IEnumerator DashingLeft30EnterState ()
	{
		Ability dashLeft30 = abilityDatabase.GetAbility("DashLeft30");
		dashLeft30.execute(gameObject, null);
		yield return null;
	}
	
	public void DashingLeft30Update ()
	{
		DashLeft30 dashLeft30 = memoizer.GetMemoizedComponent<DashLeft30>(gameObject);
		dashLeft30.gameObjectToDashTo = gameObjectToZigZagTo;
		
		if (dashLeft30.currentState.Equals(DashLeft30.states.Finish))
			currentState = states.Finish;
	}
	
	public IEnumerator DashingRight30EnterState ()
	{
		Ability dashRight30 = abilityDatabase.GetAbility("DashRight30");
		dashRight30.execute(gameObject, null);
		yield return null;
	}
	
	public void DashingRight30Update ()
	{
		DashRight30 dashRight30 = memoizer.GetMemoizedComponent<DashRight30>(gameObject);
		dashRight30.gameObjectToDashTo = gameObjectToZigZagTo;
		
		if (dashRight30.currentState.Equals(DashRight30.states.Finish))
			currentState = states.DashingLeft30;
	}
	
	public void FinishUpdate ()
	{
		TerminateFSM();
	}
	
	public static bool Test (GameObject user, GameObject target)
	{
		if (DashRight30.Test(user, target))
		{
			GameObject dummy = Utilities.dummy;
			dummy.transform.position = user.transform.position;
			dummy.transform.forward = user.transform.forward;
			DashRight30.CommitToDummy(dummy, target);
			
			return DashLeft30.Test(dummy, target);
		}
		
		return false;
	}
	
	public static GameObject CommitToDummy (GameObject dummy, GameObject target)
	{
		DashRight30.CommitToDummy(dummy, target);
		return DashLeft30.CommitToDummy(dummy, target);
	}
}