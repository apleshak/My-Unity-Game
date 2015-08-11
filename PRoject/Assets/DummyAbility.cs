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
	
	public IEnumerator Test()
	{
		Debug.Log("--Starting coroutine--");
		yield return new WaitForSeconds(2);
		Debug.Log(System.String.Format("--Coroutine over with ID: {0} --", ID));
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
		StartCoroutine(Test());
		currentState = states.DoNothing;
	}
	
	public void DoNothingUpdate () 
	{
		currentState = states.Finish;
		containingAbility.successful = true;
		Debug.Log(System.String.Format("Finished dummy ability with ID: {0}", ID));
		containingAbility.finished = true;
	}
	
	public void FinishUpdate () 
	{
		//
	}
}
