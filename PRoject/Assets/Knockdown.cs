using UnityEngine;
using System.Collections;

public class Knockdown : AbilityFSM 
{
	public enum states
	{
		KnockedDown
	}
	
	public float duration = 1.5f;
	Animator animator;
	EntityDescriptor descriptor;
	
	// Use this for initialization
	void Start () 
	{
		animator = memoizer.GetMemoizedComponent<Animator>(gameObject);
		animator.SetTrigger("KnockedDown");
		
		descriptor = memoizer.GetMemoizedComponent<EntityDescriptor>(gameObject);
		/* TODO: move this into update to avoid stacking issues. */
		descriptor.isKnockedDown = true;
		
		currentState = states.KnockedDown;
	}
	
	// Update is called once per frame
	void KnockedDownUpdate () 
	{
		duration -= Time.deltaTime;
		
		if (duration <= 0.0f)
		{
			animator.SetBool("IsKnockedDown", false);
			descriptor.isKnockedDown = false;
			TerminateFSM();
		}
	}
}
