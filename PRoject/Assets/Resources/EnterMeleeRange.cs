using UnityEngine;
using System.Collections;

public class EnterMeleeRange : AbilityFSM
{
	MovementCC movement;
	EntitySenses senses;
	public GameObject target;
	static float meleeRange = 1.0f;
	
	public enum states
	{
		Approaching,
		Finish
	};
	
	// Use this for initialization
	void Start () 
	{
		movement = memoizer.GetMemoizedComponent<MovementCC>(gameObject);
		senses = memoizer.GetMemoizedComponentInChildren<EntitySenses>(gameObject);
		currentState = states.Approaching;
	}
	
	// Update is called once per frame
	public void ApproachingUpdate ()
	{		
		if (target == null)
		{
			target = senses.GetTarget();
			
			if (target == null)
				currentState = states.Finish;
		}
		else
		{
			Vector3 targetDir = (gameObject.transform.position - target.transform.position).normalized;
			RaycastHit[] hits = Physics.RaycastAll(gameObject.transform.position, targetDir, meleeRange);
			
			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.gameObject == target)
				{
					currentState = states.Finish;
					return;
				}
			}
			
			movement.Approach(target);
		}
	}
	
	public void FinishUpdate ()
	{
		containingAbility.finished = true;
		TerminateFSM();
	}
	
	public static bool Test (GameObject user, GameObject target)
	{
		return true;
	}
	
	public static GameObject CommitToDummy (GameObject dummy, GameObject target)
	{
		Vector3 targetDir = (dummy.transform.position - target.transform.position).normalized;
		float distToTarget = Vector3.Distance(dummy.transform.position, target.transform.position);
		dummy.transform.position += targetDir * (distToTarget - meleeRange);
		dummy.transform.forward = targetDir;
		return dummy;
	}
}
