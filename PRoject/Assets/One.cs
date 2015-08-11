using UnityEngine;
using System.Collections;

public class One : AbilityFSM 
{
	GameObject punchAnchor;
	Animator animator;
	float fistSize = 0.15f;
	float damage = 1.0f;
	float stunDuration = 0.5f;
	bool punchTriedToHit = false;
	
	public enum states
	{
		Punching,
		Finish
	}
	// Use this for initialization
	void Start () 
	{
		punchAnchor = transform.FindChild("LeftHandAnchor").gameObject;
		animator = memoizer.GetMemoizedComponent<Animator>(gameObject);
		animator.SetTrigger("LeftPunch");
		currentState = states.Punching;
	}
	
	// Update is called once per frame
	void PunchingUpdate () 
	{
		/* This bool stays true for some number of frames and allows us to check whether the punhc landed. */
		if (animator.GetBool("LeftPunchReadyToHit"))
		{
			/* Record that attempts have been made. */
			punchTriedToHit = true;
			bool hasValidHit = false;
			/* Find what the fist hit. */
			Collider[] fistHits = Physics.OverlapSphere(punchAnchor.transform.position, fistSize);
			
			/* Go through the list and look for a gameobject with the proper tag. */
			foreach (Collider collider in fistHits)
			{	
				/* If we hit a structure we play an animation. */
				if (collider.gameObject.tag == "Structure")
				{
					/* Play an extra animation but only once - hence trigger. Like a puff of dust. */
					animator.SetTrigger("LeftPunchedStructure");
					hasValidHit = true;
				}
				/* if we hit an entity try to deal some damage to it and stun it. */
				else if (collider.gameObject.tag == "Entity")
				{
					/* Get the hit entity's descriptor.*/
					EntityDescriptor victimDescriptor = memoizer.GetMemoizedComponent<EntityDescriptor>(collider.gameObject);
					/* Deal it some damage. */
					victimDescriptor.TakeDamage(damage);
					/* Add the stun. */
					Stun stun = collider.gameObject.AddComponent<Stun>();
					stun.duration = stunDuration;
					/* Play an extra animation but only once - hence trigger. Like a puff of blood. */
					animator.SetTrigger("LeftPunchedEntity");
					hasValidHit = true;
				}
			}
			
			/* If we hit something confirm success and transition. */
			if (hasValidHit)
			{
				containingAbility.successful = true;
				currentState = states.Finish;
			}
		}
		/* Window to check for hits is past or not arrived yet? */
		else if (punchTriedToHit)
		{
			containingAbility.successful = false;
			currentState = states.Finish;
		}
	}
	
	void FinishUpdate ()
	{
		containingAbility.finished = true;
		TerminateFSM();
	}
}
