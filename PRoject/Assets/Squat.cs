using UnityEngine;
using System.Collections;

public class Squat : AbilityFSM 
{
	Animator animator;
	public enum states
	{
		Crouching
	}

	void Start () 
	{
		animator = memoizer.GetMemoizedComponent<Animator>(gameObject);
		animator.SetTrigger("Crouch");
		currentState = states.Crouching;
	}
	
	/* Wait until we have crouched and quit. */
	void CrouchingUpdate () 
	{
		if (animator.GetBool("Crouched"))
		{
			containingAbility.successful = true;
			containingAbility.finished = true;
			TerminateFSM();
		}
	}
}
