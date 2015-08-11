using UnityEngine;
using System.Collections;

/* 
   Expells slime on ground in front of user in a small area that
   hinders movement and causes dashes through it to result in
   knockdowns of the dashing users.
   
   Requires user to have a VomitAnchor object and the existance
   of a Slime Emitter prefab that destroys itself and spawns Slime Puddles.
*/
public class StickySlime : AbilityFSM
{
	Animator animator;
	/* This prefab creates both the vomit particles and the slime puddles. */
	Object slimePrefab;
	/* The reference to the spawned prefab. */
	GameObject slimeEmitter;
	/* The controller of the emitter component on the slimeEmitter gameobject. */
	EmitterController emitterController;
	public enum states
	{
		Begin,
		Vomiting,
		Finish
	}
	
	void Start ()
	{
		currentState = states.Begin;
		animator = memoizer.GetMemoizedComponent<Animator>(gameObject);
		slimePrefab = memoizer.GetMemoizedPrefab("Sticky Slime", "Slime Emitter");
	}
	
	/* Spawns vomit prefab, creates references and starts animaton. */
	public void BeginUpdate ()
	{
		/* Get the attachment point of the slimeEmitter. */
		Transform vomitAnchor = gameObject.transform.FindChild("VomitAnchor");
		/* Spawn it. */
		slimeEmitter = (GameObject)Instantiate(slimePrefab, vomitAnchor.position, Quaternion.identity);
		/* Make it a child of the attachment point transform. */
		slimeEmitter.transform.SetParent(vomitAnchor.transform);
		/* Get the emitter to determine when this ability ends. */
		emitterController = slimeEmitter.GetComponent<EmitterController>();
		/* Start the animation. */
		animator.SetTrigger("Vomiting");
		
		currentState = states.Vomiting;
	}
	
	/* Waits for the emitter to stop vomiting, then report success. */
	public void VomitingUpdate ()
	{
		if (emitterController.finished)
		{
			containingAbility.successful = true;
			currentState = states.Finish;
		}
	}
	
	public void FinishUpdate ()
	{
		containingAbility.finished = true;
		TerminateFSM();
	}
}

