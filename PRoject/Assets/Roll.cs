using UnityEngine;
using System.Collections;

public class Roll : AbilityFSM
{
	Animator animator;
	MovementCC movement;
	public float distance = 2.0f;
	public EntityBehaviour.movementModes mode = EntityBehaviour.movementModes.Fast;
	public enum states
	{
		Rolling,
		Finish
	}
		
	void Start () 
	{
		animator = memoizer.GetMemoizedComponent<Animator>(gameObject);
		animator.SetBool("Rolling", true);
		
		movement = memoizer.GetMemoizedComponent<MovementCC>(gameObject);
		movement.lockInput();
		
		currentState = states.Rolling;
		
	}
	
	// Update is called once per frame
	void RollingUpdate () 
	{
		distance -= movement.Move(mode);
		
		if (distance <= 0)
		{
			animator.SetBool("Rolling", false);
			currentState = states.Finish;
		}
	}
	
	void FinishUpdate ()
	{
		movement.unlockInput();
		TerminateFSM();
	}
	
}
