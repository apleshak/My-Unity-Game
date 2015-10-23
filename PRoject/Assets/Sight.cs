using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/* Holds and remembers sightings of entities. Can track targets and remember the 
   last known position even when disabled. The duration of the memory depends on
   persistTime. */
[RequireComponent (typeof(SphereCollider))] //does this collide work in 3d mode? NO!
public class Sight : MyMonoBehaviour, ISense
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
			
			if (!value)
			{
				hasTarget = false;
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
	EntityBehaviour behaviour;
	public EntityDescriptor ownerDescriptor;
	
	SphereCollider collider;
	List<Sighting> sightings;
	Sighting currentSighting;
	bool hasTarget;
	
	void Awake () 
	{
		collider = gameObject.GetComponent<SphereCollider>();
		collider.isTrigger = true;
		
		sightings = new List<Sighting>();
		hasTarget = false;
		timeSinceTargetSighting = 0.0f;
		enabled = true;
	}
	
	void Update ()
	{
		timeSinceTargetSighting += Time.deltaTime;
	}
	
	//Introduces new sightings
	void OnTriggerStay (Collider other)
	{
		// If enabled, within LOS, within vision cone and the other entity is considered hostile 
		if (enabled && InLOS(gameObject.transform.position, other.gameObject) &&
		    SphericalSector.inRange(gameObject.transform.position, other.gameObject.transform.position, 
		                        angle, gameObject.transform.forward, range) && 
		    behaviour.hostiles.Overlaps(Utilities.GetAllTags(other.gameObject)))
		{	
			// Check if you've seen it already and update the sighting 
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
			
			// Otherwise make a new sighting 
			if (!seen)
			{
				EntityDescriptor descriptor = memoizer.GetMemoizedComponent<EntityDescriptor>(other.gameObject);
				Sighting newSighting = new Sighting(other.gameObject.transform.position, other.gameObject, descriptor);
				sightings.Add(newSighting);
			}
		}
	}
	
	
	//Returns true if obj is within line of sight from pos. Note that the raycast will not hit
	//a collider if the ray was cast from inside of it.
			
	//TODO: optimize with layermasks
	//TODO: this is not and LOS, its a range check
	bool InLOS (Vector3 pos, GameObject obj)
	{
		RaycastHit[] hits = Physics.RaycastAll(pos, (obj.transform.position - pos).normalized, range);
		Array.Sort(hits, (a,b) => 
			{
				float aDist = Vector3.Distance(pos, a.transform.position);
				float bDist = Vector3.Distance(pos, b.transform.position);
				float diff = bDist - aDist;
				return Mathf.FloorToInt(diff) + Mathf.CeilToInt(diff);
			});
		bool seen = false;
		
		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.gameObject == obj)
				seen = true;
			
			if (!seen && hit.collider.tag == "Structure")
				return false;
		}
		
		return true;
	}
	
	bool UpdateAndRemove (Sighting sighting)
	{
		// Add accumulated time since last target was seen.
		sighting.time += timeSinceTargetSighting;
		
		// Do we purge the sighting?
		if (sighting.time >= persistTime)
		{
			// If the expired sighting is the current one then update some values.
			if (sighting == currentSighting)
			{
				currentSighting = null;
				hasTarget = false;
			}
			
			return true;
		}
		
		return false;
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
			sightings.RemoveAll(UpdateAndRemove);
			timeSinceTargetSighting = 0.0f;
			
			//Find best sighting
			foreach (Sighting sighting in sightings)
			{
				if (Sighting.cmp(sighting, bestSoFar, gameObject, ownerDescriptor, mode))
				{
					bestSoFar = sighting;
				}
			}
			
			// We have a new target.
			if (bestSoFar != null)
			{
				hasTarget = true;
				currentSighting = bestSoFar;
				return bestSoFar.source;
			}
			else
			{
				hasTarget = false;
				return null;
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
	
	// If currentSighting is null then hasTarget is false. Drops target even when disabled.
	public void DropTarget ()
	{
		if (currentSighting != null)
		{
			sightings.RemoveAll((a) => {return a.source == currentSighting.source;});
			currentSighting = null;
			hasTarget = false;
		}
	}
	
	public void SetBehaviour (EntityBehaviour beh)
	{
		behaviour = beh;
		range = beh.sightRange;
		angle = beh.sightAngle;
		persistTime = beh.sightPersistTime;
		mode = beh.sightMode;
	}
	
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
		
		// True if a > b in the context of the mode and user. 
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
}