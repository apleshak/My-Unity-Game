using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/* Same as Sight but only has a target as long as the sound memory lasts. The position
   returned for any target is always the currentSound position. */
[RequireComponent (typeof(SphereCollider))]
public class Hearing : MyMonoBehaviour, ISense
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
			
			if (value == false)
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
	public float persistTime = 0.01f;
	public float timeSinceTargetHeard;
	public EntityBehaviour.hearingModes mode;
	EntityBehaviour behaviour;
	public EntityDescriptor ownerDescriptor;
	
	SphereCollider collider;
	List<Sound> sounds;
	Sound currentSound;
	bool hasTarget;
	
	// Use this for initialization
	void Awake () 
	{
		collider = gameObject.GetComponent<SphereCollider>();
		collider.isTrigger = true;
		
		sounds = new List<Sound>();
		hasTarget = false;
		timeSinceTargetHeard = 0.0f;
		enabled = true;
	}
	
	void Update ()
	{
		timeSinceTargetHeard += Time.deltaTime;
	}
	
	/* Using OnCollisionStay because Enter adn Exit only happen once. */
	/* NO NEED FOR OVERRIDE SINCE WE'RE NOT USING STATES */
	/* Must occur in all states so we have to override the FSM pass-through. */
	/* O(n*k) where n is sounds and k is colliders. */
	void OnTriggerStay (Collider other)
	{		
		//Debug.Log("HEARING TRIGGER");
		//Debug.Log(other.gameObject.name);
		/* If enabled, within LOS, within vision cone and the other entity is considered hostile */
		if (enabled && behaviour.hostiles.Overlaps(Utilities.GetAllTags(other.gameObject)))
		{			
			/* Check if you've heard it already and update the sound */
			bool heard = false;
			
			foreach (Sound sound in sounds)
			{
				if (sound.source == other.gameObject)
				{
					heard = true;
					sound.time = 0.0f;
					sound.position = other.transform.position;
					break;
				}
			}
			
			/* Otherwise make a new sound */
			if (!heard)
			{
				EntityDescriptor descriptor = other.gameObject.GetComponent<EntityDescriptor>();
				Sound newSighting = new Sound(other.gameObject.transform.position, other.gameObject, descriptor);
				sounds.Add(newSighting);
			}
		}
	}
	
	bool UpdateAndRemove (Sound sound)
	{
		/* Add accumulated time since last target was seen. */
		sound.time += timeSinceTargetHeard;
		
		/* Do we purge the sighting? */
		if (sound.time >= persistTime)
		{
			/* If the expired sighting is the current one then update some values. */
			if (sound == currentSound)
			{
				currentSound = null;
				hasTarget = false;
			}
			
			return true;
		}
		
		return false;
	}
	
	/* Within one frame only one thing can happen:
		If we have no target: A new target is found.
		If we have a target: It's closest neighbor sound is found (possibly traversing a series of sounds). */
	public GameObject GetTarget ()
	{
		if (!enabled)
		{
			return null;
		}
		else
		{
			Sound bestSoFar = null;
			sounds.RemoveAll(UpdateAndRemove);
			timeSinceTargetHeard = 0.0f;
			
			/* TODO: Make sure this iterator is safe to remove from. */
			foreach (Sound sound in sounds)
			{
				/* We already have a target. Check if this Sound is close and has the same source. */
				if (hasTarget && currentSound.source == sound.source)
				{
					if (bestSoFar != null)
					{
						float oldDist = Vector3.Distance(bestSoFar.position, currentSound.position);
						float newDist = Vector3.Distance(sound.position, currentSound.position);
						
						if (newDist < oldDist)
						{
							bestSoFar = sound;
						} 
					}
					else
					{
						bestSoFar = sound;
					}
				}
				/* Try to make it a new target. */
				else if (!hasTarget)
				{
					if (Sound.cmp(sound, bestSoFar, mode, gameObject, ownerDescriptor))
					{
						bestSoFar = sound;
					}
				}
				/* Have target but this sound is from a different source. */
				else
				{
					//Ignore it
				}
			}
			
			/* We found a new target. */
			if (bestSoFar != null)
			{
				hasTarget = true;
				currentSound = bestSoFar;
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
	
	/* If currentSound is null then hasTarget is false. Drops target even when disabled. */
	public void DropTarget ()
	{
		if (currentSound != null)
		{
			foreach (Sound sound in sounds)
			{
				if (sound.source == currentSound.source)
				{
					sounds.Remove(sound);
				}
			}
			
			currentSound = null;
			hasTarget = false;
		}
	}
	
	public void SetBehaviour (EntityBehaviour beh)
	{
		behaviour = beh;
		range = beh.hearingRange;
		persistTime = beh.hearingPersistTime;
		mode = beh.hearingMode;
	}
	
	class Sound
	{
		public Vector3 position;
		public float time;
		public GameObject source;
		public EntityDescriptor descriptor;
		
		public Sound (Vector3 Position, GameObject Source, EntityDescriptor Descriptor)
		{
			position = Position;
			source = Source;
			time = 0.0f;
			descriptor = Descriptor;
		}
		
		/* True if a > b in the context of the mode and user. */
		public static bool cmp (Sound a, Sound b, EntityBehaviour.hearingModes mode, GameObject user, EntityDescriptor descr)
		{
			if (a == null)
			{
				return false;
			}
			else if (b == null)
			{
				return true;
			}
			else if (mode == EntityBehaviour.hearingModes.Closest)
			{
				float distA = Vector3.Distance(a.position, user.transform.position);
				float distB = Vector3.Distance(b.position, user.transform.position);
				return Math.Min(distA, distB) == distA;
			}
			else if (mode == EntityBehaviour.hearingModes.Farthest)
			{
				float distA = Vector3.Distance(a.position, user.transform.position);
				float distB = Vector3.Distance(b.position, user.transform.position);
				return Math.Max(distA, distB) == distA;
			}
			else if (mode == EntityBehaviour.hearingModes.Loudest)
			{
				float audibilityA = EntityDescriptor.CompareAudibility(descr, a.descriptor);
				float audibilityB = EntityDescriptor.CompareAudibility(descr, b.descriptor);
				return Math.Min(audibilityA, audibilityB) == audibilityA;
			}
			else if (mode == EntityBehaviour.hearingModes.Quietest)
			{
				float audibilityA = EntityDescriptor.CompareAudibility(descr, a.descriptor);
				float audibilityB = EntityDescriptor.CompareAudibility(descr, b.descriptor);
				return Math.Max(audibilityA, audibilityB) == audibilityA;
			}
			else //unrecognized behaviour
			{
				return false;
			}
		}	
	}
}
