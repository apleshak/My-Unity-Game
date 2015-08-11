using UnityEngine;
using System.Collections;

public class Stun : AbilityFSM 
{
	public enum states
	{
		Stunned
	}
	
	/* Default value. Can be changed externally. */
	public float duration = 0.5f;
	Animator animator;
	Object stunPrefab;
	GameObject stunObject;
	EntityDescriptor descriptor;
	
	// Use this for initialization
	void Start () 
	{
		/* Get the animator and play the stunned animation. */
		animator = memoizer.GetMemoizedComponent<Animator>(gameObject);
		animator.SetBool("Stunned", true);
		
		/* Find the anchor and child the stun prefab to it. */
		Transform stunAnchor = transform.FindChild("StunAnchor");
		stunPrefab = memoizer.GetMemoizedPrefab("Abilities/Stun", "StunPrefab");
		stunObject = (GameObject)Instantiate(stunPrefab, stunAnchor.position, Quaternion.identity);
		stunObject.transform.SetParent(stunAnchor);
		
		/* Get the decriptor and switch states. */
		descriptor = memoizer.GetMemoizedComponent<EntityDescriptor>(gameObject);
		/* TODO: move this into update to avoid stacking issues. */
		descriptor.isStunned = true;
		currentState = states.Stunned;
	}
	
	// Update is called once per frame
	void StunnedUpdate () 
	{
		duration -= Time.deltaTime;
		
		if (duration <= 0.0f)
		{
			Destroy(stunObject);
			animator.SetBool("Stunned", false);
			descriptor.isStunned = false;
			TerminateFSM();
		}
	}
}
