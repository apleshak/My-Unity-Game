using UnityEngine;
using System.Collections;

public class DummyAbility : AbilityFSM
{
	int ID;
	
	public enum states
	{
		Begin,
		DoNothing,
		Finish
	}
	
	public IEnumerator DummyCoroutine()
	{
		Debug.Log("--Starting Dummy coroutine--");
		yield return new WaitForSeconds(1);
		Debug.Log(System.String.Format("-- Dummy coroutine over with ID: {0} --", ID));
		TerminateFSM();
	}
	
	// Use this for initialization
	void Start () 
	{
		currentState = states.Begin;
		ID = Random.Range(0, 10000);
		Debug.Log("Starting Dummy!");
	}
	
	// Runs once then changes state
	public void BeginUpdate () 
	{
		StartCoroutine(DummyCoroutine());
		currentState = states.DoNothing;
	}
	
	public void DoNothingUpdate () 
	{
		currentState = states.Finish;
		containingAbility.successful = true;
		containingAbility.finished = true;
		Debug.Log(System.String.Format("Finished dummy ability with ID: {0}", ID));
	}
	
	public void FinishUpdate () 
	{
		//
	}
	
	public static bool Test (GameObject user, GameObject target)
	{
		return true;
	}
}
