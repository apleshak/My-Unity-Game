using UnityEngine;
using System.Collections;

public class FaceGameObjectRight30 : AbilityFSM
{
	MovementCC movement;
	//Animator animator;
	public GameObject gameObjectToFace;
	
	public enum states
	{
		Turning,
		Finish
	};
	
	// Use this for initialization
	void Start () 
	{
		movement = memoizer.GetMemoizedComponent<MovementCC>(gameObject);
		EntitySenses senses = memoizer.GetMemoizedComponentInChildren<EntitySenses>(gameObject);
		
		if (gameObjectToFace == null)
			gameObjectToFace = senses.GetTarget();
		
		currentState = states.Turning;
	}
	
	// Update is called once per frame
	public void TurningUpdate () 
	{
		Vector3 targetForward = (Quaternion.Euler(0, 30, 0) * (gameObjectToFace.transform.position - gameObject.transform.position)).normalized;
		movement.Rotate(targetForward);
		
		if (gameObject.transform.forward == targetForward)
			currentState = states.Finish;
	}
	
	public void FinishUpdate () 
	{
		TerminateFSM();
	}
	
	public static bool Test (GameObject user, GameObject target)
	{
		return true;
	}
	
	public static GameObject CommitToDummy (GameObject dummy, GameObject target)
	{
		Vector3 targetForward = 
			(Quaternion.Euler(0, 30, 0) * (target.transform.position - dummy.transform.position)).normalized;
		dummy.transform.forward = targetForward;
		return dummy;
	}
}