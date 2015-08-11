using UnityEngine;
using System.Collections.Generic;

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
	HashSet<Tuple<ActionBar, GameObject>> minions;
	AbilityDatabase abilityDatabase;
	
	public Commander (AbilityDatabase AbilityDatabase)
	{
		player = MyMonoBehaviour.player;
		oracle = new Oracle<Ability>(MyMonoBehaviour.playerAbilityActionBar);
		minions = new HashSet<Tuple<ActionBar, GameObject>>();
		abilityDatabase = AbilityDatabase;
	}
	
	public void addDatabase (AbilityDatabase database)
	{
		abilityDatabase = database;
	}
	
	public void addMinion (Tuple<ActionBar, GameObject> minion)
	{
		minions.Add(minion);
	}
	
	public void removeMinion (Tuple<ActionBar, GameObject> minion)
	{
		minions.Remove(minion);
	}
	
	/* Manipulates the ActionBar of each minion for optimal combat results. */
	public void commandMinions()
	{
		/* Greedy approach: (Can be altered for DP-bruteforce like Knapsack problems)
			Sort ActionBars from most promising to least with eval
			Choose action for first ActionBar
		   	Choose action for next ActionBar assuming all actions up to now have succeeded (can modify with probability weighted values)
		 */
		
	}
	
	/* Should not need to be used if commandMinions is correct. */
	public void commandMinion()
	{
		
	}
	
	/* Ignores other minions choices. */
	public void commandMinionsNaively()
	{
		Ability playerAction = oracle.nextState;
		
		foreach (Tuple<ActionBar, GameObject> enemyInfo in minions)
		{
			Ability bestOption = abilityDatabase.getBestCounter(playerAction, player, 
			                                                    enemyInfo.first.getUsableAbilities(), enemyInfo.second);
			enemyInfo.first.execute(bestOption);
		}
	}
	
	public void Update () 
	{
		oracle.Update();
	}
}