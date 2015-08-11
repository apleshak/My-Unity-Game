using UnityEngine;
using System.Collections;

/* Movement through a Character Controller. */
[RequireComponent (typeof(CharacterController))]
public class MovementCC : MyMonoBehaviour 
{
	bool inputLocked;
	public EntityBehaviour behaviour;
	public EntityDescriptor ownerDescriptor;
	public EntityBehaviour.movementModes mode;
	public Vector3 targetForward;
	
	public float turnSmoothing = 15f;   // A smoothing value for turning the player.
	public float speedDampTime = 0.1f;  // The damping for the speed parameter
	private Animator anim;              // Reference to the animator component.
	private CharacterController controller;
	
	void Start ()
	{
		inputLocked = false;
	}
	
	void Awake ()
	{
		ownerDescriptor = GetComponent<EntityDescriptor>();
		controller = GetComponent<CharacterController>();
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
			GetComponent<Rigidbody>().MovePosition(transform.position + (transform.forward * Time.deltaTime) * speedDampTime);
		}
	}
	
	public void Move (float dist)
	{
		GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward*dist);
	}
	
	void Rotate (float horizontal, float vertical)
	{
		// Create a rotation based on this new vector assuming that up is the global y axis.
		Quaternion targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Lerp(GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		GetComponent<Rigidbody>().MoveRotation(newRotation);
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
