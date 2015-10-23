using UnityEngine;
using System.Collections;

/* 
	Attach to EmptyGameObjects
	Can be queried and controlled by Commander to issue orders.
	
	Starts off with combatReady false in Idle mode.
	Sight and Hearing provide targets and estimates in the form of GameObject and last known positions.
	Sight targets are prefered to hearing targets.
*/
public class EntitySenses : MyMonoBehaviour, ISense
{
	bool _enabled;
	public bool enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			_enabled = value;
			
			if (value)
			{
				sight.enabled = true;
				hearing.enabled = true;
			}
			else
			{
				sight.enabled = false;
				hearing.enabled = false;
			}
		}
	}
		
	EntityBehaviour behaviour;
	EntityDescriptor ownerDescriptor;
	public bool combatReady;
	public enum targetSourceSense
	{
		Sight,
		Hearing
	};
	
	public GameObject target;
	public targetSourceSense targetSource;
	bool hasTarget;
	Sight sight;
	Hearing hearing;
	float pursueTime;
	
	void Awake ()
	{
		ownerDescriptor = gameObject.GetComponentInParent<EntityDescriptor>();
		sight = gameObject.GetComponentInChildren<Sight>();
		hearing = gameObject.GetComponentInChildren<Hearing>();
	}
	
	void Start ()
	{
		pursueTime = 0.0f;
		sight.ownerDescriptor = ownerDescriptor;
		hearing.ownerDescriptor = ownerDescriptor;
		SetBehaviour(EntityBehaviour.GetMinionBehaviour());
		enabled = true;
		combatReady = false;
	}
	
	void Update ()
	{
		GetTarget();
	}
	
	public bool HasTarget ()
	{
		return sight.HasTarget() || hearing.HasTarget();
	}
	
	/* Prefers sight over hearing. */
	public GameObject GetTarget ()
	{
		return player;
		
		if (sight.GetTarget() != null)
		{
			target = sight.GetTarget();
			return target;
		}
		else if (hearing.GetTarget() != null)
		{
			target = hearing.GetTarget();
			return target;
		}
		else
		{
			target = null;
			return null;
		}
	}
	
	public void DropTarget ()
	{
		sight.DropTarget();
		hearing.DropTarget();
	}
	
	public void SetBehaviour (EntityBehaviour beh)
	{
		behaviour = beh;
		//transitionInterval = beh.reactionTime;
		sight.SetBehaviour(behaviour);
		hearing.SetBehaviour(behaviour);
	}
}
