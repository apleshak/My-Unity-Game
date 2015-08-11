using UnityEngine;
using System.Collections;

public class ChaseResource : MyMonoBehaviour 
{
	public EntityDescriptor descriptor;
	public MovementCC movement;
	public Animator animator;
	GameObject interactingResource;
	bool interacting = false;
	float interactTime = 3.0f;
	float interactElapsed = 0.0f;
	float interactDist = 2.0f;
	
	// Use this for initialization
	void Awake () 
	{
		//descriptor = memoizer.GetMemoizedComponent<EntityDescriptor>(gnome);
		//movement = memoizer.GetMemoizedComponent<MovementCC>(gameObject);
		//animator = memoizer.GetMemoizedComponent<Animator>(gnome);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (interacting)
		{
			//resource disappeared
			if (interactingResource == null)
			{
				interacting = false;
				interactElapsed = 0.0f;
				animator.SetBool("Interacting", false);
			}
			else
			{
				//face the resource
				movement.Rotate(interactingResource.transform.position-transform.position);
				//resource moved out of range
				if (Vector3.Distance(interactingResource.transform.position, transform.position) > interactDist)
				{
					interacting = false;
					interactElapsed = 0.0f;
					animator.SetBool("Interacting", false);
					return;
				}
				
				interactElapsed += Time.deltaTime;
				
				//interaction completed
				if (interactElapsed >= interactTime)
				{
					interacting = false;
					interactElapsed = 0.0f;
					animator.SetBool("Interacting", false);
					Destroy(interactingResource);
					
					//penalty
					if (descriptor.hp == 3)
					{
						movement.HoldTempMode(EntityBehaviour.movementModes.Slow, 3.0f);
					}
					else //hp boost
					{
						descriptor.hp += 1;
					}
				}
			}
		}
		else
		{
			GameObject closestResource = null;
			float closestDist = float.MaxValue;
			
			foreach (GameObject resource in GameObject.FindGameObjectsWithTag("Resource"))
			{
				float dist = Vector3.Distance(transform.position, resource.transform.position);
				
				if (dist < closestDist)
				{
					closestDist = dist;
					closestResource = resource;
				}
			}
			
			interactingResource = closestResource;
			
			if (interactingResource == null)
			{
				return;
			}
			else
			{
				if (closestDist > interactDist)
				{
					movement.Approach(closestResource);
				}
				else
				{
					interacting = true;
					animator.SetBool("Interacting", true);
				}
			}
		}
	}
}
