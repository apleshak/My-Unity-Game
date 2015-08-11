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
	float remainingDistance = 5.0f;
	
	public enum states
	{
		Dashing,
		Finish
	};
	
	void Start ()
	{
		movement = memoizer.GetMemoizedComponent<MovementCC>(this.gameObject);
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
	
	public void FinishUpdate ()
	{
		Debug.Log("Done dashing!");
		/* Unlock player input, report completion and self-destruct. */
		movement.unlockInput();
		//animator.SetBool("Dashing", false);
		containingAbility.finished = true;
		TerminateFSM();
	}
}