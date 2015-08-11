using UnityEngine;
using System.Collections;

/* Throws projectile of type T. Successful upon release of the projectile. Finished upon animation completion. */
public class ThrowProjectile<T> : AbilityFSM where T : AbilityFSM
{
	Object prefab;
	Animator animator;
	public bool creatingProjectile;
	
	public enum states
	{
		Throwing,
		Released
	};

	/* prepares the prefab and starts the throwing animation. */
	void Start () 
	{	
		if (typeof(T) == typeof(Fireball))
		{
			creatingProjectile = true;
			prefab = memoizer.GetMemoizedPrefab("Abilities/Fireball", "Fireball");
		}
		else if (typeof(T) == typeof(Throw))
		{
			creatingProjectile = false;
		}
		
		animator = memoizer.GetMemoizedComponent<Animator>(gameObject);
		// The trigger is reset only after a state change occurs in the animator
		animator.SetTrigger("Throw");
		currentState = states.Throwing;
	}
	
	/* Checks when the animator is ready to release the projectile and spawns it appropriately. */
	void ThrowingUpdate ()
	{
		if (animator.GetBool("ReadyToThrow"))
		{
			/* Spawn the object prefab and let it do its magic. */
			if (creatingProjectile)
			{
				Vector3 position = transform.FindChild("Fireball_SpawnPoint").position;
				GameObject.Instantiate(prefab, position, Quaternion.identity);
				containingAbility.successful = true;
			}
			/* Detatch the grabbed object and put the Throw script on it. */
			else
			{
				Grab grabScript = GetComponent<Grab>();
				grabScript.grabbedObject.transform.SetParent(null);
				grabScript.grabbedObject.AddComponent<Throw>();
			}
			
			currentState = states.Released;
		}
	}
	
	/* The ability is only considered finished when the animation ends. */
	void ReleasedUpdate ()
	{
		if (!animator.GetBool("Throwing"))
		{
			containingAbility.finished = true;
		}
	}
}
