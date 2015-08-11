using UnityEngine;
using System.Collections;

/* 
	Can be queried and controlled by Commander to issue orders. Must attach directly to entity
	because its tag is used to find the behaviour needed for Sight, Hearing and Movement.
	
	Starts off with combatReady false in Idle mode.
	Sight and Hearing provide targets and estimates in the form of GameObject and last known positions.
	Sight targets are prefered to hearing targets.
*/
[RequireComponent (typeof(Sight))]
[RequireComponent (typeof(Hearing))]
[RequireComponent (typeof(MovementRB))]
public class AIController : FiniteStateMachineBehaviour, ISense
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
				movement.enabled = true;
				currentState = states.Idle;
			}
			else
			{
				sight.enabled = false;
				hearing.enabled = false;
				movement.enabled = false;
				currentState = states.Sleeping;
			}
		}
	}
		
	public EntityBehaviour behaviour;
	public EntityDescriptor ownerDescriptor;
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
	MovementRB movement;
	float pursueTime;
	
	public enum states
	{
		Sleeping,
		Idle,
		Pursuing,
		Combat,
		Attacking,
		Dead
	};
	
	public bool HasTarget ()
	{
		return sight.HasTarget() || hearing.HasTarget();
	}
	
	/* Prefers sight over hearing. */
	public GameObject GetTarget ()
	{
		if (sight.HasTarget())
		{
			return sight.GetTarget();
		}
		else if (hearing.HasTarget())
		{
			return hearing.GetTarget();
		}
		else
		{
			return null;
		}
	}
	
	public void DropTarget ()
	{
		sight.DropTarget();
		hearing.DropTarget();
	}
	
	/* Return default(Vector3) if we have no target. */
	public Vector3 GetLastPosition ()
	{
		if (HasTarget())
		{
			if (targetSource == targetSourceSense.Sight)
			{
				return sight.GetLastPosition();
			}
			else
			{
				return hearing.GetLastPosition();
			}
		}
		else
		{
			return default(Vector3);
		}
	}
	
	public void SetBehaviour (EntityBehaviour beh)
	{
		behaviour = beh;
		transitionInterval = beh.reactionTime;
		sight.SetBehaviour(behaviour);
		hearing.SetBehaviour(behaviour);
		movement.SetBehaviour(behaviour);
	}
	
	void Awake ()
	{
		pursueTime = 0.0f;
		ownerDescriptor = gameObject.GetComponent<EntityDescriptor>();
		sight = gameObject.GetComponent<Sight>();
		hearing = gameObject.GetComponent<Hearing>();
		movement = gameObject.GetComponent<MovementRB>();
		sight.ownerDescriptor = ownerDescriptor;
		hearing.ownerDescriptor = ownerDescriptor;
		movement.ownerDescriptor = ownerDescriptor;
		enabled = true;
		combatReady = false;
	}
	
	/* Shifts to Idle and Death mode only. */
	void SleepingUpdate ()
	{
		if (ownerDescriptor.isDead)
		{
			currentState = states.Dead;
			return;
		}
		
		if (enabled)
		{
			currentState = states.Idle;
		}
	}
	
	/* Shifts to Combat and Death mode only. */
	void IdleUpdate ()
	{
		if (ownerDescriptor.isDead)
		{
			currentState = states.Dead;
			return;
		}
		
		/* Look for a target and go to Combat mode when we find one. */
		if (HasTarget() && combatReady)
		{
			target = GetTarget();
			currentState = states.Combat;
		}
	}
	
	void CombatUpdate ()
	{
		if (ownerDescriptor.isDead)
		{
			currentState = states.Dead;
			return;
		}
		
		if (!hasTarget)
		{
			currentState = states.Pursuing;
		}
		else if (hasTarget)
		{
			if (ownerDescriptor.InAttackRange(target))
			{
				ownerDescriptor.Attack(target);
				currentState = states.Attacking;
			}
			else
			{
				movement.Approach(target, EntityBehaviour.movementModes.Fast);
				currentState = states.Pursuing;
			}
		}
		
		
	}
	
	void PursuingUpdate ()
	{
		if (ownerDescriptor.isDead)
		{
			currentState = states.Dead;
			return;
		}
	}
	
	void AttackingUpdate ()
	{
		if (ownerDescriptor.isDead)
		{
			currentState = states.Dead;
			return;
		}
		
		if (!ownerDescriptor.IsAttacking())
		{
			currentState = states.Combat;
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
