using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/* Holds and remembers sightings of entities. Can track targets and remember the 
   last known position even when disabled. The duration of the memory depends on
   persistTime. */
[RequireComponent (typeof(CircleCollider2D))] //does this collide work in 3d mode?
public class Sight : MyMonoBehaviour, ISense
{
	/* Bad style but so good for abstraction. */
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
			
			if (!value)
			{
				hasTarget = false;
			}
			else
			{
				GetTarget();
			}
		}
	}
	public float range
	{
		get
		{
			return collider.radius;
		}
		set
		{
			collider.radius = value;
		}
	}
	
	public float angle;
	public float persistTime;
	public float timeSinceTargetSighting;
	public EntityBehaviour.sightModes mode;
	public EntityBehaviour behaviour;
	public EntityDescriptor ownerDescriptor;
	
	CircleCollider2D collider;
	List<Sighting> sightings;
	Sighting currentSighting;
	bool hasTarget;
	
	class Sighting
	{
		public Vector3 position;
		public float time;
		public GameObject source;
		public EntityDescriptor descriptor;
		
		public Sighting (Vector3 Position, GameObject Source, EntityDescriptor Descriptor)
		{
			position = Position;
			source = Source;
			time = 0.0f;
			descriptor = Descriptor;
		}
		
		/* True if a > b in the context of the mode and user. */
		public static bool cmp (Sighting a, Sighting b, GameObject user, EntityDescriptor ownerDescr, EntityBehaviour.sightModes mode)
		{
			if (a == null)
			{
				return false;
			}
			else if (b == null)
			{
				return true;
			}
			else if (mode == EntityBehaviour.sightModes.Closest)
			{
				float distA = Vector3.Distance(a.position, user.transform.position);
				float distB = Vector3.Distance(b.position, user.transform.position);
				return Math.Min(distA, distB) == distA;
			}
			else if (mode == EntityBehaviour.sightModes.Farthest)
			{
				float distA = Vector3.Distance(a.position, user.transform.position);
				float distB = Vector3.Distance(b.position, user.transform.position);
				return Math.Max(distA, distB) == distA;
			}
			else if (mode == EntityBehaviour.sightModes.LeastDangerous)
			{
				float dangerA = EntityDescriptor.CompareDanger(ownerDescr, a.descriptor);
				float dangerB = EntityDescriptor.CompareDanger(ownerDescr, b.descriptor);
				return Math.Min(dangerA, dangerB) == dangerA;
			}
			else if (mode == EntityBehaviour.sightModes.MostDangerous)
			{
				float dangerA = EntityDescriptor.CompareDanger(ownerDescr, a.descriptor);
				float dangerB = EntityDescriptor.CompareDanger(ownerDescr, b.descriptor);
				return Math.Max(dangerA, dangerB) == dangerA;
			}
			else //some unrecognized mode
			{
				return false;
			}
		}	
	}
	
	public bool HasTarget ()
	{
		if (enabled)
		{
			return hasTarget;
		}
		else
		{
			return false;
		}
	}
	
	public GameObject GetTarget ()
	{
		if (!enabled)
		{
			return null;
		}
		else
		{
			Sighting bestSoFar = null;
			
			/* TODO: Make sure this iterator is safe to remove from. */
			foreach (Sighting sighting in sightings)
			{
				/* Add accumulated time since last target was seen. */
				sighting.time += timeSinceTargetSighting;
				
				/* Do we purge the sighting? */
				if (sighting.time >= persistTime)
				{
					/* If the expired sighting is the current one then update some values. */
					if (sighting == currentSighting)
					{
						currentSighting = null;
						hasTarget = false;
					}
					
					sightings.Remove(sighting);
				}
				/* Otherwise, is the sighting a new target? */
				else
				{
					if (Sighting.cmp(sighting, bestSoFar, gameObject, ownerDescriptor, mode))
					{
						bestSoFar = sighting;
					}
				}
			}

			/* We found a new target. */
			if (bestSoFar != null)
			{
				hasTarget = true;
				currentSighting = bestSoFar;
				timeSinceTargetSighting = 0.0f;
				return bestSoFar.source;
			}
			else
			{
				hasTarget = false;
				return null;
			}
		}
	}
	
	/* If currentSighting is null then hasTarget is false. Drops target even when disabled. */
	public void DropTarget ()
	{
		if (currentSighting != null)
		{
			foreach (Sighting sighting in sightings)
			{
				if (sighting.source == currentSighting.source)
				{
					sightings.Remove(sighting);
				}
			}

			currentSighting = null;
			hasTarget = false;
		}
	}
	
	/* Works even when disabled. */
	public Vector3 GetLastPosition ()
	{
		if (currentSighting != null)
		{
			return currentSighting.position;
		}
		else
		{
			return default(Vector3);
		}
	}
	
	public void SetBehaviour (EntityBehaviour beh)
	{
		behaviour = beh;
		range = beh.sightRange;
		collider.radius = range;
		angle = beh.sightAngle;
		persistTime = beh.sightPersistTime;
		mode = beh.sightMode;
	}
	
	// Use this for initialization
	void Awake () 
	{
		enabled = true;
		hasTarget = false;
		sightings = new List<Sighting>();
		collider = gameObject.GetComponent<CircleCollider2D>();
		timeSinceTargetSighting = 0.0f;
	}
	
	void Update ()
	{
		timeSinceTargetSighting += Time.deltaTime;
	}
	
	/* 	
		Returns true if obj is within line of sight from pos. Note that the raycast will not hit
		a collider if the ray was cast from inside of it.
		
		TODO: optimize with layermasks
	*/
	bool InLOS (Vector3 pos, GameObject obj)
	{
		foreach (RaycastHit hit in Physics.RaycastAll(pos, (pos - obj.transform.position).normalized, range))
		{
			if (hit.collider.gameObject == obj)
			{
				return true;
			}
		}
		
		return false;
	}
	
	/* Using OnCollisionStay because Enter adn Exit only happen once. */
	/* NO NEED FOR OVERRIDE SINCE WE'RE NOT USING STATES */
	/* Must occur in all states so we have to override the FSM pass-through. */
	void OnCollisionStay (Collision other)
	{
		/* If enabled, within LOS, within vision cone and the other entity is considered hostile */
		if (enabled && InLOS(gameObject.transform.position, other.gameObject) &&
			SphericalSector.inRange(gameObject.transform.position, other.gameObject.transform.position, 
									angle, gameObject.transform.forward, range) && 
			behaviour.hostiles.Contains(other.gameObject.tag))
		{
			/* Check if you've seen it already and update the sighting */
			bool seen = false;
			
			foreach (Sighting sighting in sightings)
			{
				if (sighting.source == other.gameObject)
				{
					seen = true;
					sighting.time = 0.0f;
					sighting.position = other.transform.position;
					break;
				}
			}
			
			/* Otherwise make a new sighting */
			if (!seen)
			{
				EntityDescriptor descriptor = other.gameObject.GetComponent<EntityDescriptor>();
				Sighting newSighting = new Sighting(other.gameObject.transform.position, other.gameObject, descriptor);
				sightings.Add(newSighting);
			}
		}
	}
}
