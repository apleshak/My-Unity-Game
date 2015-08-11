using UnityEngine;
using System.Collections.Generic;
using Minion = Tuple<AbilityActionBar, UnityEngine.GameObject>;
/* 
	The Commander governs all non-idle AI FSMs. Each Commander has
	its own group of AIs. The Commander queries the Oracle for the
	player's next action and collects the ActionBars and respective
	GameObjects of its group. Then it decides on a separate action 
	for each entity. Needs to have its update method called to update
	Oracle data.
	
	TODO: remove dependence on "Player" tag from Instatiotor
	TODO: only update oracle when the target/player switches states
*/
public class Commander
{
	GameObject player;
	Oracle<Ability> oracle;
	HashSet<Minion> minions;
	AbilityDatabase abilityDatabase;
	enum minionActions
	{
		Move,
		Attack,
		Defend
	}
	
	public Commander (AbilityDatabase AbilityDatabase)
	{
		player = MyMonoBehaviour.player;
		oracle = new Oracle<Ability>(MyMonoBehaviour.playerDescriptor.abilityBar);
		minions = new HashSet<Minion>();
		abilityDatabase = AbilityDatabase;
	}
	
	public void SetDatabase (AbilityDatabase database)
	{
		abilityDatabase = database;
	}
	
	public void AddMinion (Minion minion)
	{
		minions.Add(minion);
	}
	
	public void RemoveMinion (Minion minion)
	{
		minions.Remove(minion);
	}
	
	Minion GetClosestMinion (Vector3 position)
	{
		Minion closestMinion = null;
		float closestDist = float.MaxValue;
		
		foreach (Minion minion in minions)
		{
			float distance = Vector3.Distance(position, minion.second.transform.position);
			
			if (distance < closestDist)
			{
				closestMinion = minion;
				closestDist = distance;
			}
		}
		
		return closestMinion;
	}
	
	/* Move minion towards a location. */
	void MinionApproach (Minion minion, Vector3 position)
	{
		MovementCC minionMovement = MyMonoBehaviour.memoizer.GetMemoizedComponent<MovementCC>(minion.second);
		minionMovement.Move(position - minion.second.transform.position);
	}
	
	/* 	Return enum telling what type of action is best for the minion. If we memorize the calls made
		we can alter new minion actions based on past decisions.*/
	minionActions ChooseMinionAction (Minion minion)
	{
		return minionActions.Attack;
	}
	
	/* Returns best ability for the minion to use. */
	Ability ChooseMinionAbility (Minion minions, minionActions actionType)
	{
		return null;
	}
	
	/* Scores the options of a minion against the player action. No future analysis. Only currently available abilities. */
	/* Can use AB-pruning to get clearer picture.*/
	float ScoreMinion (Minion minion, Ability playerAction)
	{
		float score = 0.0f;
		
		foreach (Ability ability in minion.first.GetUnlockedAbilities())
		{
			if (ability.targeted && ability.inRange(minion.second.transform.position, 
													player.transform.position, minion.second.transform.forward))
			{
				score += abilityDatabase.CompareAbilities(ability, playerAction);
			}
			else
			{
			
			}
		}
		
		return score;
	}
	
	int MinionGroupEval (HashSet<Minion> set)
	{
		if (set.Count == 0)
		{
			return 0;
		}
		else
		{
			HashSet<AbilityActionBar> actionBars = new HashSet<AbilityActionBar>();
			
			foreach (Minion minion in set)
			{
				actionBars.Add(minion.first);
			}
			
			return abilityDatabase.ComboEval(actionBars);
		}
	}
	
	/* Lets not goldplate this just yet. */
	public void CommandMinions()
	{
		/* GOOD ENOUGH */
		/* 	1) Find player action
			2) Find all possible combos and take the best ones. 
			3) Check for and perform tactical combinations selecting the best ones.
		*/
		/* Find what Ability the player will use next. */
		Ability playerAction = oracle.nextState;
		
		/* Find best combo grouping. */
		HashSet<HashSet<Minion>> bestGrouping = Utilities.BestGrouping<Minion>(minions, MinionGroupEval);
	}
	
	/* Populates the level with entities. Maybe marshal the level structure to decouple this class from LevelBuilder. */
	public void PopulateLevel (LevelBuilder level)
	{
	
	}
	
	public void Update () 
	{
		oracle.Update();
	}
}