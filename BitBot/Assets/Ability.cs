using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 
   Targeted abilities require range and angle. All require a user to execute.
*/
public class Ability
{
	/* 
	   Shared between all Ability instances and used to evaluate commanders. Leave for later.
	   ConcurrentDictionary is threadsafe btw.
	*/
	public static Dictionary<Ability, float> successReport = new Dictionary<Ability, float>();
	public System.Type abilityType;
	public bool targeted;
	public Texture2D icon;
	public string name;
	public string description;
	public HashSet<string> tags;
	public float range;
	public float angle;
	public ActionBar containingActionBar;
	public bool finished
	{
		set
		{
			finished = value;
			
			if (value == true && containingActionBar != null)
			{
				containingActionBar.inProgress = false;
			}
		}
		get
		{
			return finished;
		}
	}
	public bool successful
	{
		set
		{
			successful = value;
			
			if (containingActionBar != null)
			{
				int lastIdx = containingActionBar.lastBoxIdx;
				
				if (value == true)
				{
					containingActionBar.abilityBoxes[lastIdx].loadNextAbility(true);
				}
				else
				{
					containingActionBar.abilityBoxes[lastIdx].loadNextAbility(true);
				}
			}
		}
		get
		{
			return successful;
		}
	}
	
	public Ability (System.Type AbilityType, Texture2D Icon, string Name, string Description, 
				   	HashSet<string> Tags, float Range, float Angle)
	{
		finished = false;
		successful = false;
		targeted = true;
		abilityType = AbilityType;
		icon = Icon;
		name = Name;
		description = Description;
		tags = Tags;
		range = Range;
		angle = Angle;
	}
	
	public Ability (System.Type AbilityType, Texture2D Icon, string Name, string Description, 
					HashSet<string> Tags)
	{
		finished = false;
		successful = false;
		targeted = false;
		abilityType = AbilityType;
		icon = Icon;
		name = Name;
		description = Description;
		tags = Tags;
	}
	
	public bool inRange (Vector3 source, Vector3 destination, Vector3 direction)
	{
		if (targeted)
		{
			return SphericalSector.inRange(source, destination, angle/2, direction, range);
		}
		
		return true;
	}
	
	// Try to get actionBAr from user? 
	public bool execute (GameObject user, ActionBar actionBar)
	{
		if (!targeted)
		{
			finished = false;
			successful = false;
			AbilityFSM addedAbility = (AbilityFSM)user.AddComponent(abilityType);
			addedAbility.containingAbility = this;
			containingActionBar = actionBar;
			return true;
		}
		
		return false;
	}
}