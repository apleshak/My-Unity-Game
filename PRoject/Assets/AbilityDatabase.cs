using UnityEngine;
using System.Collections.Generic;

/* Holds abilities and can provide deep copies via GetAbility. */
public class AbilityDatabase
{	
	public int maxComboSize;
	public HashSet<Ability> universalAbilities;
	Dictionary<string, Ability> abilities;
	Dictionary<string, List<string>> tagCounters;
	
	public AbilityDatabase ()
	{
		tagCounters = new Dictionary<string, List<string>>();
		abilities = new Dictionary<string, Ability>();
		universalAbilities = new HashSet<Ability>();
		maxComboSize = 3;
	}
	
	public int Count ()
	{ 
		return abilities.Count;
	}
	
	public void AddAbility (Ability toAdd)
	{
		abilities[toAdd.name] = toAdd;
		
		if (toAdd.universal)
			universalAbilities.Add(toAdd);
	}
	
	public void AddUniversalAbility (Ability toAdd)
	{
		universalAbilities.Add(toAdd);
		AddAbility(toAdd);
	}
	
	public Ability GetAbility (string name)
	{
		return abilities[name].DeepCopy();
	}
	
	public void AddCounter (string thisTag, string countersThisTag)
	{
		tagCounters[thisTag].Add(countersThisTag);
	}
	
	/* Of the current options each AI has, return the one that counters playerAction best. Can be null. */
	public Ability GetBestCounter (Ability playerAction, GameObject player,
	                              ICollection<Ability> enemyOptions, GameObject enemy)
	{
		Ability bestOption = null;
		int maxScore = 0;
		
		foreach (Ability enemyAbility in enemyOptions)
		{
			int score = CompareAbilities(enemyAbility, playerAction);
			bool inRange = enemyAbility.inRange(enemy.transform.position, player.transform.position, enemy.transform.forward);
			
			if (score > maxScore && inRange)
			{
				bestOption = enemyAbility;
				maxScore = score;
			}
		}
		
		return bestOption;
	}
	
	/* Used by compareAbilities. */
	bool IsCounter (string thisTag, string toThisTag)
	{
		return tagCounters[thisTag].Contains(toThisTag);
	}
	
	/* Return a postive int if ability1 has an inherent advantage over ability2. Returns a negative int otherwise. */
	public int CompareAbilities (Ability ability1, Ability ability2)
	{
		int score = 0;
		
		foreach (string tag1 in ability1.tags)
		{
			foreach (string tag2 in ability2.tags)
			{
				if (IsCounter(tag1, tag2))
				{
					score += 1;
				}
				else if (IsCounter(tag2, tag1))
				{
					score -= 1;
				}
			}
		}
		
		return score;
	}
}

