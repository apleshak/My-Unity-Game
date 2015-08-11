using UnityEngine;
using System.Collections;

/* Movement through a Character Controller. */
[RequireComponent (typeof(CharacterController))]
public class MovementCC : MonoBehaviour 
{
	/* 	When a call to approach is made the coroutine flips and remembers this value. If it
		is different furing execution the coroutine aborts. */
	Coroutine approachCoroutine;
	bool approaching;
	bool inputLocked;
	bool modeLocked;
	public float gravity = 2.0f;
	public float touchDist = 0.5f;
	public EntityBehaviour behaviour;
	public EntityDescriptor ownerDescriptor;
	EntityBehaviour.movementModes _mode;
	EntityBehaviour.movementModes defaultMode;
	public EntityBehaviour.movementModes mode
	{
		get
		{
			return _mode;
		}
		set
		{
			if (!modeLocked)
			{
				_mode = value;
				
				if (value == EntityBehaviour.movementModes.Slow)
				{
					speed = 3.0f;
				}
				else if (value == EntityBehaviour.movementModes.Medium)
				{
					speed = 8.0f;
				}
				else
				{
					speed = 15.0f;
				}
			}
		}
	}

	public float rotationSpeed = 15f;   // A smoothing value for turning the player.
	public float speed;  // The damping for the speed parameter
	private Animator anim;              // Reference to the animator component.
	private CharacterController controller;
	
	void Start ()
	{
		approaching = false;
		inputLocked = false;
		modeLocked = false;
		mode = EntityBehaviour.movementModes.Medium;
		defaultMode = mode;
	}
	
	void Awake ()
	{
		ownerDescriptor = GetComponent<EntityDescriptor>();
		controller = GetComponent<CharacterController>();
	}
	
	void Update ()
	{
		/* isGrounded only gets updated if we move. */
		controller.Move(Vector3.zero);
		
		/* Always aply gravity. */
		if (!controller.isGrounded)
		{
			Debug.Log("not grounded");
			Vector3 offset = Vector3.zero;
			offset.y -= gravity * Time.deltaTime;
			controller.Move(offset);
		}
		
		if (!inputLocked)
		{
			/* To get back slowdown effect use GetAxis. */
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");
			
			/* If we have some kind of input move and rotate. */
			if (h != 0.0f || v != 0.0f)
			{
				Move (h, v);
				Rotate (h, v);
			}
		}
	}
	
	/* 	Notice we don't use forward, so movement precedes rotation. We can change that by
		replacing vec with forward. */
	float Move (float h, float v)
	{
		Vector3 vec = new Vector3(h, 0.0f, v);
		Vector3 offset = vec.normalized * Time.deltaTime * speed;
		controller.Move(offset);
		return offset.magnitude;
	}
	
	/* 	Move one frame worth of movement. Doesn't preserve direction. 
		Reports the distance traveled. */
	public float Move (Vector3 dir)
	{
		// Perform the movement and rotation
		float dist = Move(dir.x, dir.z);
		Rotate(dir.x, dir.z);
		return dist;
	}
	
	/* 	Move one frame worth of movement. Preserves mode. Doesn't preserve direction. 
		Reports the distance traveled. */
	public float Move (Vector3 dir, EntityBehaviour.movementModes moveMode)
	{
		// Save the old mode, then set the new mode
		EntityBehaviour.movementModes oldMode = mode;
		mode = moveMode;
		
		// Perform the movement and rotation
		float dist = Move (dir.x, dir.z);
		Rotate(dir.x, dir.z);
		
		// Restore old mode
		mode = oldMode;
		return dist;
	}
	
	/* 	Move one frame worth of movement in the forward direction. Preserves mode. Doesn't preserve direction. 
		Reports the distance traveled. */
	public float Move (EntityBehaviour.movementModes moveMode)
	{
		EntityBehaviour.movementModes oldMode = mode;
		mode = moveMode;
		
		float dist = Move(transform.forward.x, transform.forward.z);
		
		mode = oldMode;
		return dist;
	}
	
	/* Moves one frame worth of movement. Reports the actual distance traveled. */
	public float Move ()
	{
		float dist = Move(transform.forward.x, transform.forward.z);
		return dist;
	}
	
	void Rotate (float h, float v)
	{
		Vector3 targetForward = new Vector3(h, 0.0f, v);
		
		// Create a rotation based on this new vector assuming that up is the global y axis.
		Quaternion targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		transform.rotation = newRotation;
	}
	
	public void Rotate (Vector3 targetForward)
	{
		Quaternion targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		transform.rotation = newRotation;
	}
	
	public void SetBehaviour (EntityBehaviour beh)
	{
		behaviour = beh;
		mode = behaviour.movementMode;
		
	}
	
	public void lockInput()
	{
		inputLocked = true;
	}
	
	public void unlockInput()
	{
		inputLocked = false;
	}
	
	public void Approach (Vector3 target, EntityBehaviour.movementModes moveMode)
	{
		if (Vector3.Distance(target, transform.position) > touchDist)
		{
			lockInput();
			ValidateApproach();
			approachCoroutine = StartCoroutine(ApproachCoroutine(target, moveMode));
		}
	}
	
	public void Approach (GameObject target, EntityBehaviour.movementModes moveMode)
	{
		if (Vector3.Distance(target.transform.position, transform.position) > touchDist)
		{
			lockInput();
			ValidateApproach();
			approachCoroutine = StartCoroutine(ApproachCoroutine(target, moveMode));
		}
	}
	
	public void Approach (Vector3 target)
	{
		if (Vector3.Distance(target, transform.position) > touchDist)
		{
			lockInput();
			ValidateApproach();
			approachCoroutine = StartCoroutine(ApproachCoroutine(target, mode));
		}
	}
	
	public void Approach (GameObject target)
	{
		if (Vector3.Distance(target.transform.position, transform.position) > touchDist)
		{
			lockInput();
			ValidateApproach();
			approachCoroutine = StartCoroutine(ApproachCoroutine(target, mode));
		}
	}
	
	void ValidateApproach ()
	{
		/* If coroutine is running stop it and restore the mode. */
		if (approaching)
		{
			StopCoroutine(approachCoroutine);
			mode = defaultMode;
		}
		else
		{
			approaching = true;
		}
	}
	
	/* Approaches a mobile target with custom movement mode.*/
	IEnumerator ApproachCoroutine (GameObject target, EntityBehaviour.movementModes moveMode)
	{
		mode = moveMode;
		Vector3 heightDiff = new Vector3(0.0f, target.transform.position.y - transform.position.y, 0.0f);
		
		while (target != null && Vector3.Distance(transform.position + heightDiff, target.transform.position) > touchDist)
		{
			Vector3 dir = target.transform.position - transform.position;
			dir.y = 0;
			heightDiff.y = target.transform.position.y - transform.position.y;
			
			Move (dir.x, dir.z);
			Rotate (dir);
			yield return null;
		}
		
		approaching = false;
		unlockInput();
		mode = defaultMode;
	}
	
	/* Approaches a static target with custom movement mode. */
	IEnumerator ApproachCoroutine (Vector3 target, EntityBehaviour.movementModes moveMode)
	{
		mode = moveMode;
		Vector3 dir = target - transform.position;
		Vector3 heightDiff = new Vector3(0.0f, target.y - transform.position.y, 0.0f);
		dir.y = 0;
		
		while (Vector3.Distance(transform.position + heightDiff, target) > touchDist)
		{
			Move (dir.x, dir.z);
			Rotate (dir);
			yield return null;
		}
		
		approaching = false;
		unlockInput();
		mode = defaultMode;
	}
	
	IEnumerator HoldModeCoroutine (EntityBehaviour.movementModes tempMode, float duration)
	{
		float elapsed = 0.0f;
		mode = tempMode;
		modeLocked = true;
		
		while (elapsed <= duration)
		{
			elapsed += Time.deltaTime;
			yield return null;
		}
		
		modeLocked = false;
		mode = defaultMode;
	}
	
	public void HoldTempMode (EntityBehaviour.movementModes tempMode, float duration)
	{
		StartCoroutine(HoldModeCoroutine(tempMode, duration));
	}
}
