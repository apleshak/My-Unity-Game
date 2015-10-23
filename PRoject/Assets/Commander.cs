using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Minion = Tuple<AbilityActionBar, UnityEngine.GameObject>;
using MinionAction = System.Func<UnityEngine.GameObject, UnityEngine.GameObject, System.Collections.IEnumerator>;
using Combo = ComboDatabase.Combo;

/* 
	The Commander governs all non-idle AI FSMs. Each Commander has
	its own group of AIs. The Commander queries the Oracle for the
	player's next action and collects the ActionBars and respective
	GameObjects of its group. Then it decides on a separate action 
	for each entity. Needs to have its update method called to update
	Oracle data.
	
	TODO: remove dependence on "Player" tag from Instatiotor
	TODO: only update oracle when the target/player switches states
	TODO: when one entity in a combo finishes return it to the Minion pool immediately
	TODO: change this whole fucking thing to use gameobjects or Minions - CHOOOOOOOOSE!!!!
*/
public class Commander
{
	GameObject player;
	Oracle<Ability> oracle;
	HashSet<Minion> busyMinions;
	HashSet<Minion> freeMinions;
	AbilityDatabase abilityDatabase;
	ComboDatabase comboDatabase;
	enum minionActions
	{
		Move,
		Attack,
		Defend
	}
	
	public Commander (AbilityDatabase AbilityDatabase, ComboDatabase ComboDatabase)
	{
		player = MyMonoBehaviour.player;
		oracle = new Oracle<Ability>(MyMonoBehaviour.playerDescriptor.abilityBar);
		busyMinions = new HashSet<Minion>();
		freeMinions = new HashSet<Minion>();
		abilityDatabase = AbilityDatabase;
		comboDatabase = ComboDatabase;
	}
	
	public void AddMinion (Minion minion)
	{
		freeMinions.Add(minion);
	}
	
	public void FreeMinion (Minion minion)
	{
		if (busyMinions.Contains(minion))
		{
			freeMinions.Add(minion);
			busyMinions.Remove(minion);
		}
	}
	
	public void RemoveMinions (ICollection<GameObject> collection)
	{
		foreach (GameObject obj in collection)
		{
			EntityDescriptor descr = MyMonoBehaviour.memoizer.GetMemoizedComponent<EntityDescriptor>(obj);
			RemoveMinion(new Tuple<AbilityActionBar, GameObject>(descr.abilityBar, obj));
		}
	}
	
	public void RemoveMinion (Minion minion)
	{
		freeMinions.Remove(minion);
		busyMinions.Remove(minion);
	}
	
	Minion GetClosestMinion (Vector3 position)
	{
		Minion closestMinion = null;
		float closestDist = float.MaxValue;
		
		foreach (Minion minion in freeMinions)
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
	void MinionApproach (Minion minion, Vector3 targetPosition)
	{
		MovementCC minionMovement = MyMonoBehaviour.memoizer.GetMemoizedComponent<MovementCC>(minion.second);
		minionMovement.Move(targetPosition - minion.second.transform.position);
	}
	
	
	/* 	Return enum telling what type of action is best for the minion. If we memorize the calls made
		we can alter new minion actions based on past decisions.*/
		/*
	minionActions ChooseMinionAction (Minion minion)
	{
		return minionActions.Attack;
	}
	*/

	/* 	Evaluates a group of minions by taking the maximum from all possible assignments.
		1) Generate all permutations of set
		2) Evaluate the assignments  */
	float MinionGroupEval (ICollection<Minion> set)
	{
		if (set.Count == 0)
		{
			return 0;
		}
		else
		{
			HashSet<GameObject> gameObjects = Utilities.Map<Minion, GameObject, HashSet<GameObject>>(set, (a) => 
				{return a.second;});
			float result = comboDatabase.FindBestComboScore(gameObjects);
			return result;
		}
	}
	
	// TODO: make it actually use the oracle's prediction.
	public void CommandMinions ()
	{
		// Find what Ability the player will use next.
		Ability playerAction = oracle.nextState;
		
		//Edge case
		if (freeMinions.Count == 0)
			return;
			
		// If we only have one minion to command right now, make it do the most effective ability available.
		if (freeMinions.Count == 1)
		{
			Tuple<AbilityActionBar, GameObject> minion = freeMinions.First<Minion>();
			Ability bestCounter = abilityDatabase.GetBestCounter(playerAction, player, minion.first.GetUsableAbilities(), minion.second);
			minion.first.execute(bestCounter);
		}
		
		// Find best combo grouping.
		Tuple<HashSet<HashSet<Minion>>, float> bestGrouping = Utilities.GetBestGrouping<Minion>(freeMinions, MinionGroupEval);
		
		foreach (HashSet<Minion> group in bestGrouping.first)
		{
			List<Minion> listView = group.ToList();
			List<GameObject> gameObjectListView = Utilities.Map<Minion, GameObject, List<GameObject>>(listView, (a) => {return a.second;});
			
			//this is the redundant work. we're recomputing the best assigment even though bestGrouping did it too
			Tuple<List<GameObject>, Combo, float> bestOption = comboDatabase.FindBestCombo(gameObjectListView);
			//this is not redundant only because it uses the order supplied.
			
			bestOption.second.SetCommander(this);
			bestOption.second.AssignInOrder(bestOption.first);
			bestOption.second.Run();
			RemoveMinions(bestOption.first);
		}
	}
	
	/* Populates the level with entities. Maybe marshal the level structure to decouple this class from LevelBuilder. */
	public void PopulateLevel (LevelBuilder level)
	{
		
	}
	
	public void Update () 
	{		
		oracle.Update();
		CommandMinions();
	}
}