using UnityEngine;
using System.Collections;

/* 
   Dashes in a straight line forward for 5 meters, locking all movement inputs.
   
   Like all abilities this one starts working when attached and ceases
   when it destroys itself. Communicated with containingAbility for
   completion and success feedback.
   
   TODO: make the movement ease-in, ease-out
*/
public class Dash : AbilityFSM
{
	MovementCC movement;
	//Animator animator;
	static float dashDistance = 5.0f;
	float remainingDistance = dashDistance;
	
	public enum states
	{
		Dashing,
		Finish
	};
	
	void Start ()
	{
		movement = memoizer.GetMemoizedComponent<MovementCC>(gameObject);
		movement.lockInput();
		
		//animator.SetBool("Dashing", true);
		currentState = states.Dashing;
		Debug.Log("Starting Dash!");
	}
	
	public void DashingUpdate ()
	{
		/* Move forward by a step. */
		remainingDistance -= movement.Move(EntityBehaviour.movementModes.Fast);
		
		if (remainingDistance <= 0.0f)
		{
			containingAbility.successful = true;
			currentState = states.Finish;
		}
	}
	
	/* TODO Check that setting finished and successful in separate frames is fine. */
	public void DashingOnCollisionEnter (Collision other)
	{
		if (other.gameObject.tag == "Structure" || other.gameObject.tag == "Entity")
		{
			containingAbility.successful = false;
			currentState = states.Finish;
		}
	}
	
	//TODO its better to terminate her rather than FinishEnterState so that other scripts can detect 
	//that it finished before it self-destructs
	public void FinishUpdate ()
	{
		Debug.Log("Done dashing!");
		/* Unlock player input, report completion and self-destruct. */
		if (gameObject == player)
			movement.unlockInput();
			
		//animator.SetBool("Dashing", false);
		containingAbility.finished = true;
		TerminateFSM();
	}
	
	//Makes sure there is nothing in the way
	public static bool Test (GameObject user, GameObject target)
	{
		return !Physics.Raycast(user.transform.position, user.transform.forward, dashDistance);
	}
	
	public static GameObject CommitToDummy (GameObject dummy, GameObject target)
	{
		dummy.transform.position += dummy.transform.forward * dashDistance;
		return dummy;
	}
}