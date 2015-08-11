using UnityEngine;
using System.Collections;

/* Attaches to an object that has just been thrown. Determines its arc, properties and fate. */
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Throw : AbilityFSM 
{
	Object hitPrefab;
	SphereCollider collider;
	Rigidbody rigidbody;
	public float damage = 1.0f;
	public enum states
	{
		Flying,
		Landed
	}

	void Start () 
	{
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<SphereCollider>();
		
		/* Figure out what hit prefab to use. */
		if (gameObject.tag == "Bottle")
		{
			hitPrefab = memoizer.GetMemoizedPrefab("Effects", "GeneralHit");
			damage = 2.0f;
		}
		/* Actually throw the object. Give force forward and random torque. */
		rigidbody.AddForce(transform.forward);
		rigidbody.AddTorque(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
		currentState = states.Flying;
	}

	void FlyingOnCollisionEnter (Collision other) 
	{
		/* If we hit an entity deal osme damage to it. */
		if (other.gameObject.tag == "Entity")
		{
			EntityDescriptor victimDescriptor = memoizer.GetMemoizedComponent<EntityDescriptor>(other.gameObject);
			victimDescriptor.TakeDamage(damage);
		}
		/* Spawn the hitPrefab. */
		Instantiate(hitPrefab, transform.position, Quaternion.identity);
		currentState = states.Landed;
	}
	
	/* Since this ability was not fired from actionBar we don't need to report success or completion. */
	void LandedUpdate ()
	{
		TerminateFSM();
	}
}
