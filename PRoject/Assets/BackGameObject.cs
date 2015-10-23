using UnityEngine;
using System.Collections;

public class BackGameObject : AbilityFSM
{
	MovementCC movement;
	//Animator animator;
	public GameObject gameObjectToTurnBackOn;
	
	public enum states
	{
		Turning,
		Finish
	};
	
	// Use this for initialization
	void Start () 
	{
		movement = memoizer.GetMemoizedComponent<MovementCC>(gameObject);
		currentState = states.Turning;
	}
	
	// Update is called once per frame
	public void TurningUpdate ()
	{
		Vector3 targetForward = gameObjectToTurnBackOn.transform.position - gameObject.transform.position;
		movement.Rotate(-targetForward);
		
		if (gameObject.transform.forward == -targetForward)
			currentState = states.Finish;
	}
	
	public void FinishUpdate ()
	{
		TerminateFSM();
	}
}
