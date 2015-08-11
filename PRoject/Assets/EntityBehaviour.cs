using UnityEngine;
using System;
using System.Collections.Generic;

/* Holds sensory data for all entites. */
public class EntityBehaviour
{
	public float sightRange;
	public float sightAngle;
	public float sightPersistTime;
	public float hearingRange;
	public float hearingPersistTime;
	public float reactionTime;
	
	public HashSet<string> hostiles;
	public sightModes sightMode;
	public hearingModes hearingMode;
	public movementModes movementMode;
	public enum sightModes
	{
		Closest,
		Farthest,
		MostDangerous,
		LeastDangerous
	};
	public enum hearingModes
	{
		Closest,
		Farthest,
		Loudest,
		Quietest
	};
	public enum movementModes
	{
		Slow,
		Medium,
		Fast
	};
	
	public EntityBehaviour (float sRange, float sAngle, float sPersistTime, float reactTime, 
							HashSet<string> hostileSet, sightModes sMode, hearingModes hMode)
	{
		sightRange = sRange;
		sightAngle = sAngle;
		sightPersistTime = sPersistTime;
		reactionTime = reactTime;
		hostiles = hostileSet;
		sightMode = sMode;
		hearingMode = hMode;
	}
	
	public static EntityBehaviour GetBehaviour (string tag) 
	{
		if (tag == "Some Enemy")
		{
			HashSet<string> hostiles = new HashSet<string>();
			hostiles.Add("Some Other Enemy");
			
			return new EntityBehaviour(42, 42, 42, 0.01f, hostiles, sightModes.Closest, hearingModes.Closest);
		}
		else
		{
			return null;
		}
	}
}
