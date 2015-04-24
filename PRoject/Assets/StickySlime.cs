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
	Memoizer memoizer;
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
		currState = states.Begin;
		Instantiator instantiator = Instantiator.instance;
		memoizer = instantiator.memoizer;
		animator = memoizer.getMemoizedComponent<Animator>(gameObject);
		slimePrefab = memoizer.getMemoizedPrefabs("Slime Emitter");
	}

	void BeginUpdate ()
	{
		/* could optimize with memoization too */
		Transform vomitAnchor = gameObject.transform.FindChild("VomitAnchor");
		slimeEmitter = (GameObject)GameObject.Instantiate(slimePrefab, vomitAnchor.position, Quaternion.identity);
		emitterController = slimeEmitter.GetComponent<EmitterController>();
		animator.SetBool("Vomiting", true);
		currState = states.Vomiting;
	}
	
	void VomitingUpdate ()
	{
		if (emitterController.finished)
		{
			containingAbility.successful = true;
			currState = states.Finish;
		}
	}
	
	void FinishUpdate ()
	{
		animator.SetBool("Vomiting", false);
		containingAbility.finished = true;
		Destroy(this);
	}
}

