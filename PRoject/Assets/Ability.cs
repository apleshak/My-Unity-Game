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
	public Sprite icon;
	public string name;
	public string description;
	public HashSet<string> tags;
	public float range;
	public float angle;
	public ActionBar containingActionBar;
	bool Finished;
	public bool finished
	{
		set
		{
			Finished = value;
			
			if (value == true && containingActionBar != null)
			{
				containingActionBar.inProgress = false;
			}
		}
		get
		{
			return Finished;
		}
	}
	
	bool Successful;
	public bool successful
	{
		set
		{
			Successful = value;
			
			if (containingActionBar != null)
			{
				int lastIdx = containingActionBar.lastBoxIdx;
				
				if (value == true)
				{
					containingActionBar.abilityBoxes[lastIdx].loadNextAbility(true);
				}
				else
				{
					containingActionBar.abilityBoxes[lastIdx].loadNextAbility(false);
				}
			}
		}
		get
		{
			return Successful;
		}
	}
	
	/* For targeted abilties. */
	public Ability (System.Type AbilityType, Sprite Icon, string Name, string Description, 
				   	HashSet<string> Tags, float Range, float Angle)
	{
		Finished = false;
		Successful = false;
		targeted = false;
		abilityType = AbilityType;
		icon = Icon;
		name = Name;
		description = Description;
		tags = Tags;
		range = Range;
		angle = Angle;
	}
	
	/* For untargeted abilties. */
	public Ability (System.Type AbilityType, Sprite Icon, string Name, string Description, 
					HashSet<string> Tags)
	{
		Finished = false;
		Successful = false;
		targeted = false;
		abilityType = AbilityType;
		icon = Icon;
		name = Name;
		description = Description;
		tags = Tags;
	}
	
	public Ability DeepCopy ()
	{
		if (targeted)
		{
			return new Ability(abilityType, icon, name, description, tags, range, angle);
		}
		else
		{
			return new Ability(abilityType, icon, name, description, tags);
		}
	}
	
	public bool inRange (Vector3 source, Vector3 destination, Vector3 direction)
	{
		if (targeted)
		{
			return SphericalSector.inRange(source, destination, angle/2, direction, range);
		}
		
		return true;
	}
	
	//TODO Try to get actionBar from user (overload)?
	/* Ability writes back to the action bar when done. */
	public bool execute (GameObject user, ActionBar actionBar)
	{
		Debug.Log("Executing from ability");
		Finished = false;
		Successful = false;
		AbilityFSM addedAbility = (AbilityFSM)user.AddComponent(abilityType);
		addedAbility.containingAbility = this;
		containingActionBar = actionBar;
		return true;
	}
	
	public bool execute (GameObject user, GameObject target, ActionBar actionBar)
	{
		if (!targeted || inRange(user.transform.position, 
			target.transform.position, user.transform.forward))
		{
			execute(user, actionBar);
		}
		
		return false;
	}
}