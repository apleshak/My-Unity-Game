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
	Object slimePrefab;
	GameObject slimeEmitter;
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
		animator = memoizer.getMemoizedComponent<Animator>(gameObject);
		slimePrefab = memoizer.getMemoizedPrefab("Sticky Slime", "Slime Emitter");
	}

	public void BeginUpdate ()
	{
		/* could optimize with memoization too */
		Transform vomitAnchor = gameObject.transform.FindChild("VomitAnchor");
		slimeEmitter = (GameObject)GameObject.Instantiate(slimePrefab, vomitAnchor.position, Quaternion.identity);
		emitterController = slimeEmitter.GetComponent<EmitterController>();
		animator.SetBool("Vomiting", true);
		currentState = states.Vomiting;
	}
	
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
		animator.SetBool("Vomiting", false);
		containingAbility.finished = true;
		Destroy(this);
	}
}

