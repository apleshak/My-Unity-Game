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
	
	public EntityBehaviour (float sRange, float sAngle, float hRange, float sPersistTime, float hPersistTime, float reactTime, 
							HashSet<string> hostileSet, sightModes sMode, hearingModes hMode)
	{
		sightRange = sRange;
		sightAngle = sAngle;
		sightPersistTime = sPersistTime;
		hearingPersistTime = hPersistTime;
		reactionTime = reactTime;
		hostiles = hostileSet;
		sightMode = sMode;
		hearingMode = hMode;
		hearingRange = hRange;
		movementMode = movementModes.Medium;
	}
	
	public EntityBehaviour (float sRange, float sAngle, float hRange, float sPersistTime, float hPersistTime, float reactTime, 
	                        HashSet<string> hostileSet, sightModes sMode, hearingModes hMode, movementModes mMode)
	{
		sightRange = sRange;
		sightAngle = sAngle;
		sightPersistTime = sPersistTime;
		hearingPersistTime = hPersistTime;
		reactionTime = reactTime;
		hostiles = hostileSet;
		sightMode = sMode;
		hearingMode = hMode;
		hearingRange = hRange;
		movementMode = mMode;
	}
	
	public static EntityBehaviour GetBehaviour (string tag) 
	{
		if (tag == "Minion")
		{
			HashSet<string> hostiles = new HashSet<string>();
			hostiles.Add("Some Other Entity");
			
			return new EntityBehaviour(10, 180, 15, 10, 10, 0.01f, hostiles, sightModes.Closest, hearingModes.Closest);
		}
		else
		{
			return null;
		}
	}
	
	public static EntityBehaviour GetMinionBehaviour ()
	{
		HashSet<string> hostiles = new HashSet<string>();
		hostiles.Add("Player");
		return new EntityBehaviour(10, 180, 15, 10, 10, 0.01f, hostiles, sightModes.Closest, hearingModes.Closest);
	}
}
