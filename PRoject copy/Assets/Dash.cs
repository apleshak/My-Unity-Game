using UnityEngine;
using System.Collections;

/* 
   Dashes in a straight line forward, locking all movement inputs.
   
   Like all abilities this one starts working when attached and ceases
   when it destroys itself. Communicated with containingAbility for
   completion and success feedback.
   
   TODO: make the movement ease-in, ease-out
*/
public class Dash : AbilityFSM
{
	MovementRB movement;
	float stepDistance = 0.25f;
	float remainingDistance = 4.0f;
	Animator animator;
	
	public enum states
	{
		Begin,
		Dashing,
		Finish
	};
	
	void Start ()
	{
		currentState = states.Begin;;
		movement = memoizer.getMemoizedComponent<MovementRB>(this.gameObject);
		animator = memoizer.getMemoizedComponent<Animator>(this.gameObject);
	}
	
	public void BeginUpdate ()
	{
		/* Lock player input, play the dash animation and move to next state. */
		movement.lockInput();
		animator.SetBool("Dashing", true);
		currentState = states.Dashing;
	}
	
	public void DashingUpdate ()
	{
		Debug.Log("Dashing!");
		/* Move forward by step distance. */
		movement.Move(stepDistance);
		remainingDistance -= stepDistance;
		
		if (remainingDistance <= 0.0f)
		{
			containingAbility.successful = true;
			currentState = states.Finish;
		}
	}
	
	/* TODO Check that setting finished and successful in separate frames is fine. */
	public void DashingOnCollisionEnter (Collider other)
	{
		if (other.gameObject.tag == "Structure" || other.gameObject.tag == "Minion" || 
			other.gameObject.tag == "Entity")
		{
			containingAbility.successful = false;
			currentState = states.Finish;
		}
	}
	
	public void FinishUpdate ()
	{
		Debug.Log("Done dashing!");
		/* Unlock player input, end animation, report completion and self-destruct. */
		animator.SetBool("Dashing", false);
		containingAbility.finished = true;
		movement.unlockInput();
		Destroy(this);
	}
}