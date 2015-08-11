using UnityEngine;
using System.Collections;

public class Grab : AbilityFSM 
{
	Animator animator;
	public GameObject grabbedObject;
	public Object defaultObject;
	public float radius = 1.0f;
	public enum states
	{
		Holding,
		Finish
	}
	// Use this for initialization
	void Start () 
	{
		animator = memoizer.GetMemoizedComponent<Animator>(gameObject);
		GameObject closestObject = null;
		float closestDistance = radius;
		
		Collider[] objectsInRange = Physics.OverlapSphere(transform.position, radius);
		
		foreach (Collider collider in objectsInRange)
		{
			float newDistance = Vector3.Distance(transform.position, collider.transform.position);
			
			if (newDistance < closestDistance)
			{
				closestDistance = newDistance;
				closestObject = collider.gameObject;
			}
		}
		
		Transform handTransform = transform.FindChild("RightHandAnchor");
		
		if (closestObject != null)
		{
			animator.SetTrigger("PickUp");
			/* Move it to the hand and child to it. */
			closestObject.transform.position = handTransform.position;
			closestObject.transform.SetParent(handTransform);
		}
		else
		{
			closestObject = (GameObject)Instantiate(defaultObject, handTransform.position, Quaternion.identity);
			closestObject.transform.SetParent(handTransform);
		}
		
		grabbedObject = closestObject;
		containingAbility.successful = true;
		containingAbility.finished = true;
		currentState = states.Holding;
	}
	
	/* Could perform a check here for whether the object held being punched with. */
	void HoldingUpdate () 
	{
	
	}
}
