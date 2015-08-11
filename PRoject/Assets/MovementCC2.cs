using UnityEngine;
using System.Collections;

/* Movement through a Character Controller. */
[RequireComponent (typeof(CharacterController))]
public class MovementCC2 : MonoBehaviour 
{
	/* 	When a call to approach is made the coroutine flips and remembers this value. If it
		is different furing execution the coroutine aborts. */
	bool approachSwitch;
	bool inputLocked;
	public EntityBehaviour behaviour;
	public EntityDescriptor ownerDescriptor;
	EntityBehaviour.movementModes _mode;
	public EntityBehaviour.movementModes mode
	{
		get
		{
			return _mode;
		}
		set
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

	public float rotationSpeed = 15f;   // A smoothing value for turning the player.
	public float speed;  // The damping for the speed parameter
	private Animator anim;              // Reference to the animator component.
	private CharacterController controller;
	
	void Start ()
	{
		approachSwitch = false;
		inputLocked = false;
		mode = EntityBehaviour.movementModes.Medium;
	}
	
	void Awake ()
	{
		ownerDescriptor = GetComponent<EntityDescriptor>();
		controller = GetComponent<CharacterController>();
	}
	
	void Update ()
	{
		if (!inputLocked)
		{
			/* To get back slowdown effect use GetAxis. */
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");

			if (h != 0.0f || v != 0.0f)
			{
				Move (h, v);
				Rotate (h, v);
			}
		}
	}
	
	/* 	Notice we don't use forward, so movement precedes rotation. We can change that by
		replacing vec with forward. */
	void Move (float h, float v)
	{
		Vector3 vec = new Vector3(h, 0.0f, v);
		controller.Move(vec * Time.deltaTime * speed);
	}
	
	/* 	Move one frame worth of movement. Doesn't preserve direction. 
		Reports the distance traveled. */
	public float Move (Vector3 dir)
	{
		// Set the direction 
		transform.forward = dir.normalized;
		
		// Perform the movement
		Vector3 offsetVector = transform.forward * Time.deltaTime * speed;
		controller.Move(offsetVector);
		
		return Vector3.Magnitude(offsetVector);;
	}
	
	/* 	Move one frame worth of movement. Preserves mode. Doesn't preserve direction. 
		Reports the distance traveled. */
	public float Move (Vector3 dir, EntityBehaviour.movementModes moveMode)
	{
		// Set the direction and save the old mode, then set the new mode
		transform.forward = dir.normalized;
		EntityBehaviour.movementModes oldMode = mode;
		mode = moveMode;
		
		// Perform the movement
		Vector3 offsetVector = transform.forward * Time.deltaTime * speed;
		controller.Move(offsetVector);
		
		// Restore old mode
		mode = oldMode;
		return Vector3.Magnitude(offsetVector);;
	}
	
	/* 	Move one frame worth of movement in the forward direction. Preserves mode. Doesn't preserve direction. 
		Reports the distance traveled. */
	public float Move (EntityBehaviour.movementModes moveMode)
	{
		EntityBehaviour.movementModes oldMode = mode;
		mode = moveMode;
		
		Vector3 offsetVector = transform.forward * Time.deltaTime * speed;
		controller.Move(offsetVector);
		
		mode = oldMode;
		return Vector3.Magnitude(offsetVector);;
	}
	
	/* Moves one frame worth of movement. Reports the actual distance traveled. */
	public float Move ()
	{
		Vector3 offsetVector = transform.forward * speed * Time.deltaTime;
		controller.Move(offsetVector);
		return Vector3.Magnitude(offsetVector);;
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
	
	public void Approach (Vector3 target, EntityBehaviour.movementModes mode)
	{
		lockInput();
		approachSwitch = !approachSwitch;
		StartCoroutine(ApproachCoroutine(target, mode));
	}
	
	public void Approach (GameObject target, EntityBehaviour.movementModes mode)
	{
		lockInput();
		approachSwitch = !approachSwitch;
		StartCoroutine(ApproachCoroutine(target.transform.position, mode));
	}
	
	/* TODO the approachSwitch check maynot always work. For example is 3 Approach calls are made.*/
	IEnumerator ApproachCoroutine (Vector3 target, EntityBehaviour.movementModes moveMode)
	{
		bool approachCheck = approachSwitch;
		EntityBehaviour.movementModes originalMode = mode;
		mode = moveMode;
		
		while (transform.position != target)
		{
			// Quit if another Approach call was made
			if (approachSwitch != approachCheck)
			{
				yield break;
			}
			
			Move ();
			Vector3 targetForward = (target - transform.position).normalized;
			Rotate (targetForward);
			yield return null;
		}
		
		inputLocked = false;
		mode = originalMode;
	}
}
