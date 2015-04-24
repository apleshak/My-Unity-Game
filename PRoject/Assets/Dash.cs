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
	Memoizer memoizer;
	Movement movement;
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
		currState = states.Begin;
		Instantiator instantiator = Instantiator.instance;
		memoizer = instantiator.memoizer;
		movement = memoizer.getMemoizedComponent<Movement>(this.gameObject);
		animator = memoizer.getMemoizedComponent<Animator>(this.gameObject);
	}
	
	void BeginUpdate ()
	{
		/* Lock player input, play the dash animation and move to next state. */
		movement.lockInput();
		animator.SetBool("Dashing", true);
		currState = states.Dashing;
	}
	
	void DashingUpdate ()
	{
		/* Move forward by step distance. */
		movement.move(stepDistance);
		remainingDistance -= stepDistance;
		
		if (remainingDistance <= 0.0f)
		{
			containingAbility.successful = true;
			currState = states.Finish;
		}
	}
	
	void DashingOnCollisionEnter (Collider other)
	{
		if (other.gameObject.tag == "Structure" || other.gameObject.tag == "Minion" || 
			other.gameObject.tag == "Entity")
		{
			animator.SetBool("Dashing", false);
			containingAbility.successful = true;
			currState = states.Finish;
		}
	}
	
	void FinishUpdate ()
	{
		/* Unlock player input, end animation, report completion and self-destruct. */
		animator.SetBool("Dashing", false);
		movement.unlockInput();
		containingAbility.finished = true;
		Destroy(this);
	}
}