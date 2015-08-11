using UnityEngine;
using System.Collections.Generic;

/* 
   These two are necessary to have 2 action bar components on the same object.
   Will need to fix everything that tries to obtain ActionBar
*/
public class MovementActionBar : ActionBar
{
	public MovementActionBar (GameObject owner) : base(owner)
	{
		return;
	}
}

public class AbilityActionBar : ActionBar
{
	public AbilityActionBar (GameObject owner) : base(owner)
	{
		return;
	}
}

/* 
   Holds abilityBoxes that hold a list of abilities. 
   
   When executed the lastBoxIdx box locks and that box's last ability is used. This ability 
   sets finished and successful, advance the box's nextAbilityIdx, if successful,
    and set inProgress to false, which unlocks the box.
   
   When a box executes no other box may be used until the ability in it reports completion. 
   When a box executes no other ability in it can be executed until the ability reports completion.
   
   Ability Action bar is separate from movement actionbar
   TODO: Find a way to seamlessly integrate abilities that reset their box once failed.
   TODO: find way to get multiple abilities to work at the same time
*/
public class ActionBar : Stateful<Ability>
{
	public List<AbilityBox> abilityBoxes;
	public GameObject owner;
	public int lastBoxIdx;
	bool InProgress;
	/* Touched by Ability when done. */
	public bool inProgress
	{
		get
		{
			return InProgress;
		}
		set
		{
			InProgress = value;
			
			if (value == false && (abilityBoxes.Count > lastBoxIdx))
			{
				Debug.Log("Unlocked action bar");
				abilityBoxes[lastBoxIdx].isLocked = false;
				Debug.Log ("Unlocked ability box");
			}
		}
	}
	
	public ActionBar (GameObject Owner)
	{
		lastBoxIdx = 0;
		abilityBoxes = new List<AbilityBox>();
		inProgress = false;
		owner = Owner;
	}
	
	/* Never call with stack-allocated lists. */
	public void addAbilityBox (List<Ability> abilities)
	{
		AbilityBox newBox = new AbilityBox(abilities, this);
		abilityBoxes.Add(newBox);
	}
	
	/* 
	   Still cheap if we only have a few abilities. Also sets currState in accordance with 
	   being Stateful and sets inProgress to true.
	*/
	public bool execute (Ability ability)
	{
		if (!inProgress)
		{
			for (int i = 0; i < abilityBoxes.Count; i++)
			{
				AbilityBox box = abilityBoxes[i];
				
				if (box.currAbility == ability)
				{
					if (box.execute(owner))
					{
						lastBoxIdx = i;
						inProgress = true;
						currState = box.currAbility;
						return true;
					}
				}
			}
		}
		
		return false;
	}
	
	/* Executes ability and sets currState in accordance with being Stateful. */
	public bool execute (int idx)
	{
		if (!inProgress)
		{	
			if (abilityBoxes[idx].execute(owner))
			{
				inProgress = true;
				currState = abilityBoxes[idx].currAbility;
				return true;
			}
		}
		
		return false;
	}
	
	public List<Ability> getUnlockedAbilities()
	{
		List<Ability> unlockedAbilities = new List<Ability>();
		
		foreach (AbilityBox abilityBox in abilityBoxes)
		{
			if (!abilityBox.isLocked)
			{
				unlockedAbilities.Add(abilityBox.currAbility);
			}
		}
		
		return unlockedAbilities;
	}
	
	public List<Ability> getUsableAbilities()
	{	
		if (!inProgress)
		{
			return getUnlockedAbilities();
		}
		
		return new List<Ability>();
	}
	
	/* 
      Locks while ability is being executed. Unlocked by containingActionBar when
      inProgress is set to false. Also sets inProgress on the containing ActionBar to false 
      when ability finishes.
	*/
	public class AbilityBox
	{
		public ActionBar containingActionBar;
		public List<Ability> abilities;
		public Ability currAbility;
		/* Touched by containing action bar. */
		public bool isLocked;
		public int lastAbilityIdx;
		
		/* Be sure states are in order. States rotate in index order. */
		public AbilityBox (List<Ability> states, ActionBar actionBar)
		{
			containingActionBar = actionBar;
			abilities = states;
			lastAbilityIdx = 0;
			isLocked = false;
		}
		
		/* The ability executed will unlock the containig action bar, which will unlock this ability box. */
		public bool execute (GameObject user)
		{
			if (!isLocked)
			{
				currAbility = abilities[lastAbilityIdx];
				abilities[lastAbilityIdx].execute(user, containingActionBar);
				isLocked = true;
				Debug.Log("executed ability from action bar");
				return true;
			}
			
			return false;
		}
		
		/* If success then load next, if not go back to the first one. */
		public void loadNextAbility(bool success)
		{
			if (success)
			{
				if (lastAbilityIdx >= abilities.Count)
				{
					lastAbilityIdx = 0;
				}
				else
				{
					lastAbilityIdx += 1;
				}
			}
			else
			{
				lastAbilityIdx = 0;
			}
		}
	}
}



