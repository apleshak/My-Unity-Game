using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

/* 
   Targeted abilities require range and angle. All require a user to execute.
*/
public class Ability : IEquatable<Ability>, IEqualityComparer<Ability>
{
	public System.Type abilityType;
	public bool targeted;
	public Sprite icon;
	public string name;
	public string description;
	public HashSet<string> tags;
	public float range;
	public float angle;
	public ActionBar containingActionBar;
	public bool universal;
	bool Finished;
	Func<GameObject, GameObject, bool> testFunc;
	Func<GameObject, GameObject, GameObject> commitToDummyFunc;
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
	
	/* For initialization from XML. */
	public Ability ()
	{
		Finished = false;
		Successful = false;
	}
	
	/* For non-universal targeted abilties. */
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
		universal = false;
	}
	
	/* For non-universal untargeted abilties. */
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
		universal = false;
	}
	
	/* For targeted abilties. */
	public Ability (System.Type AbilityType, Sprite Icon, string Name, string Description, 
	                HashSet<string> Tags, float Range, float Angle, bool Univeral)
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
		universal = Univeral;
	}
	
	/* For untargeted abilties. */
	public Ability (System.Type AbilityType, Sprite Icon, string Name, string Description, 
	                HashSet<string> Tags, bool Universal)
	{
		Finished = false;
		Successful = false;
		targeted = false;
		abilityType = AbilityType;
		icon = Icon;
		name = Name;
		description = Description;
		tags = Tags;
		universal = Universal;
	}
	
	public override string ToString ()
	{
		return name;
	}
	
	public Ability DeepCopy ()
	{
		if (targeted)
		{
			return new Ability(abilityType, icon, name, description, tags, range, angle, universal);
		}
		else
		{
			return new Ability(abilityType, icon, name, description, tags, universal);
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
	
	/* Attempts to find the method Test of type Func<GameObject, GameObject bool> and runs it. Returns false otherwise. */
	//TODO memoize the mothod lookup
	public bool Test (GameObject user, GameObject target)
	{
		if (testFunc != null)
			return testFunc(user, target);
			
		MethodInfo mtdInfo = abilityType.GetMethod("Test");
		
		if (mtdInfo == null)
			return false;
		
		System.Type mtdType = typeof(Func<GameObject, GameObject, bool>);
		Func<GameObject, GameObject, bool> func = 
			System.Delegate.CreateDelegate(mtdType, mtdInfo) as Func<GameObject, GameObject, bool>;
		testFunc = func;
		return func(user, target);
	}
	
	//TODO memoize the mothod lookup
	public GameObject CommitToDummy (GameObject dummy, GameObject target)
	{
		if (commitToDummyFunc != null)
			return commitToDummyFunc(dummy, target);
			
		MethodInfo mtdInfo = abilityType.GetMethod("CommitToDummy");
		
		if (mtdInfo == null)
			return null;
		
		System.Type mtdType = typeof(Func<GameObject, GameObject, GameObject>);
		Func<GameObject, GameObject, GameObject> func = 
			System.Delegate.CreateDelegate(mtdType, mtdInfo) as Func<GameObject, GameObject, GameObject>;
		commitToDummyFunc = func;
		return func(dummy, target);
	}
	
	//TODO Try to get actionBar from user (overload)?
	/* Ability writes back to the action bar when done. */
	public bool execute (GameObject user, ActionBar actionBar)
	{
		Finished = false;
		Successful = false;
		AbilityFSM addedAbility = (AbilityFSM)user.AddComponent(abilityType);
		addedAbility.containingAbility = this;
		
		if (!universal)
		{
			containingActionBar = actionBar;
		}

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
	
	public bool Equals (Ability a)
	{
		return a.name == name;
	}
	
	public bool Equals (Ability a, Ability b)
	{
		return a.Equals(b);
	}
	
	public override int GetHashCode ()
	{
		return name.GetHashCode();
	}
	
	public int GetHashCode (Ability a)
	{
		return a.name.GetHashCode();
	}
}