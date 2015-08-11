using UnityEngine;
using System.Collections;

/* Moves through rigidbody. */
public class MovementRB : FiniteStateMachineBehaviour
{
	bool inputLocked;
	public EntityBehaviour behaviour;
	public EntityDescriptor ownerDescriptor;
	public EntityBehaviour.movementModes mode;
	public Vector3 targetForward;
	
	public float turnSmoothing = 15f;   // A smoothing value for turning the player.
	public float speedDampTime = 0.1f;  // The damping for the speed parameter
	private Animator anim;              // Reference to the animator component.
	private Rigidbody rigidbody;
	
	void Start ()
	{
		inputLocked = false;
	}
	
	void Awake ()
	{
		rigidbody = GetComponent<Rigidbody>();
		ownerDescriptor = GetComponent<EntityDescriptor>();
	}
	
	void FixedUpdate ()
	{
		float h;
		float v;
		
		if (inputLocked)
		{
			h = 0.0f;
			v = 0.0f;
			return;
		}
		else
		{
			h = Input.GetAxis("Horizontal");
			v = Input.GetAxis("Vertical");
		}
		
		Move (h, v);
	}
	
	void Move (float h, float v)
	{
		if (h == 0.0f && v == 0.0f)
		{
			targetForward = transform.forward;
		}
		else
		{
			targetForward = new Vector3(h, 0f, v);
			Rotate (h, v);
			rigidbody.MovePosition(transform.position + (transform.forward * Time.deltaTime) * speedDampTime);
		}
	}
	
	public void Move (float dist)
	{
		rigidbody.MovePosition(transform.position + transform.forward*dist);
	}
	
	void Rotate (float horizontal, float vertical)
	{
		// Create a rotation based on this new vector assuming that up is the global y axis.
		Quaternion targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		rigidbody.MoveRotation(newRotation);
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
		StartCoroutine(ApproachCoroutine(target, mode));
	}
	
	public void Approach (GameObject target, EntityBehaviour.movementModes mode)
	{
		lockInput();
		StartCoroutine(ApproachCoroutine(target.transform.position, mode));
	}
	
	IEnumerator ApproachCoroutine (Vector3 target, EntityBehaviour.movementModes mode)
	{
		float originalSpeedDamp = speedDampTime;
		
		if (mode == EntityBehaviour.movementModes.Fast)
		{
			speedDampTime = 0.0f;
		}
		
		while (transform.position != target)
		{
			targetForward = (transform.position - target).normalized;
			yield return null;
		}
		
		inputLocked = false;
		speedDampTime = originalSpeedDamp;
	}
}
/*
	void Awake ()
	{
		// Setting up the references.
		//anim = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();
		
		// Set the weight of the shouting layer to 1.
		//anim.SetLayerWeight(1, 1f);
	}
	
	
	void FixedUpdate ()
	{
		// Cache the inputs.
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		bool sneak = Input.GetButton("Sneak");
		
		MovementManagement(h, v, sneak);
	}
	
	
	void Update ()
	{
		// Cache the attention attracting input.
		//bool shout = Input.GetButtonDown("Attract");
		
		// Set the animator shouting parameter.
		//anim.SetBool(hash.shoutingBool, shout);
	}
	
	
	void MovementManagement (float horizontal, float vertical, bool sneaking)
	{
		// Set the sneaking parameter to the sneak input.
		//anim.SetBool(hash.sneakingBool, sneaking);
		
		// If there is some axis input...
		if(horizontal != 0f || vertical != 0f)
		{
			// ... set the players rotation and set the speed parameter to 5.5f.
			Rotating(horizontal, vertical);
			//anim.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.deltaTime);
		}
		else
		{
			// Otherwise set the speed parameter to 0.
			//anim.SetFloat(hash.speedFloat, 0);
		}
	}
	
	
	void Rotating (float horizontal, float vertical)
	{
		// Create a new vector of the horizontal and vertical inputs.
		Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
		
		// Create a rotation based on this new vector assuming that up is the global y axis.
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		rigidbody.MoveRotation(newRotation);
	}
	
	void Start ()
	{
		inputLocked = false;
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
	
	public void move(float distance)
	{
		gameObject.transform.position += transform.forward * distance;
	}
}
*/