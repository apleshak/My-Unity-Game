using UnityEngine;
using System.Collections.Generic;

public class AbilityDatabase
{	
	Dictionary<string, Ability> abilities;
	Dictionary<string, List<string>> tagComparator;
	
	public AbilityDatabase()
	{
		tagComparator = new Dictionary<string, List<string>>();
		abilities = new Dictionary<string, Ability>();
	}
	
	public void addAbility(Ability toAdd)
	{
		abilities[toAdd.name] = toAdd;
	}
	
	public Ability getAbility (string name)
	{
		return abilities[name];
	}
	
	public void addCounter(string thisTag, string countersThisTag)
	{
		tagComparator[thisTag].Add(countersThisTag);
	}
	
	/* Of the current options each AI has, return the one that counters playerAction best. Can be null. */
	public Ability getBestCounter(Ability playerAction, GameObject player,
	                              List<Ability> enemyOptions, GameObject enemy)
	{
		Ability bestOption = null;
		int maxScore = 0;
		
		foreach(Ability enemyAbility in enemyOptions)
		{
			int score = compareAbilities(enemyAbility, playerAction);
			bool inRange = enemyAbility.inRange(enemy.transform.position, player.transform.position, enemy.transform.forward);
			
			if (score > maxScore && inRange)
			{
				bestOption = enemyAbility;
				maxScore = score;
			}
		}
		
		return bestOption;
	}
	
	bool isCounter(string thisTag, string toThisTag)
	{
		return tagComparator[thisTag].Contains(toThisTag);
	}
	
	int compareAbilities(Ability ability1, Ability ability2)
	{
		int score = 0;
		
		foreach (string tag1 in ability1.tags)
		{
			foreach (string tag2 in ability2.tags)
			{
				if (isCounter(tag1, tag2))
				{
					score += 1;
				}
				else if (isCounter(tag2, tag1))
				{
					score -= 1;
				}
			}
		}
		
		return score;
	}
}

