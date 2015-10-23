using UnityEngine;
using System.Collections;

public class FaceGameObject : AbilityFSM 
{
	MovementCC movement;
	//Animator animator;
	public GameObject gameObjectToFace;
	
	public enum states
	{
		Turning,
		Finish
	};
	
	//Make sure that arguments like gameObjectToFace are set at initialization (thats when Awake is called)
	//so that at this point we have a valid gameObjectToFace
	void Start () 
	{
		movement = memoizer.GetMemoizedComponent<MovementCC>(gameObject);
		currentState = states.Turning;
	}
	
	//TODO make sure we are consistent across AbilityFSMs regarding when we perform a check for transition: before or after we change the object
	public void TurningUpdate () 
	{
		Vector3 targetForward = (gameObjectToFace.transform.position - gameObject.transform.position).normalized;
		movement.Rotate(targetForward);
		
		if (gameObject.transform.forward == targetForward)
			currentState = states.Finish;
	}
	
	public void FinishUpdate ()
	{
		TerminateFSM();
	}
	
	public static bool Test (GameObject dummy, GameObject target)
	{
		return true;
	}
	
	public static GameObject CommitToDummy (GameObject dummy, GameObject target)
	{
		Vector3 targetForward = (target.transform.position - dummy.transform.position).normalized;
		dummy.transform.forward = targetForward;
		return dummy;
	}
}
