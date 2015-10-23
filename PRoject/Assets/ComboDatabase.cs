using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Minion = Tuple<AbilityActionBar, UnityEngine.GameObject>;

public class ComboDatabase 
{
	Dictionary<int, HashSet<Combo>> combos;
	int count = 0;
	bool showDebug = false;
	
	public ComboDatabase ()
	{
		combos = new Dictionary<int, HashSet<Combo>>();
	}
	
	public int Count ()
	{
		return count;
	}
	
	public void AddCombo (Combo combo)
	{	
		count++;
		
		if (!combos.ContainsKey(combo.actors))
			combos[combo.actors] = new HashSet<Combo>();
		
		combos[combo.actors].Add(combo);
	}
	
	/* Finds best score from all possible orderings of actors, evaluate each ordering with EvaluateAssignment. */
	public float FindBestComboScore (ICollection<GameObject> actors)
	{
		Tuple<List<GameObject>, Combo, float> solution = FindBestCombo(actors);
		return solution.third;
	}
	
	public Tuple<List<GameObject>, Combo, float> FindBestCombo (ICollection<GameObject> actors)
	{		
		//No combos of size less than 2 exist
		if (actors.Count < 2)
			return new Tuple<List<GameObject>, Combo, float>(new List<GameObject>(), new ComboDatabase.Combo(), 0.0f);
			
		HashSet<Combo> possibleCombos = combos[actors.Count];		
		Func<List<GameObject>, Combo, float> eval = (a, b) => {return b.EvaluateAssignment(a);};
		return Utilities.GetBestOrdering<Combo, GameObject>(actors, possibleCombos, eval);
	}
	
	//Default validator checks that all the abilities necessary for an FSM are on the user and
	//that the first state in the FSM is an ability currently usable by the assigned actor
	public class Combo
	{
		public int actors;
		Commander commander;
		List<EntityFSM> fsms;
		Dictionary<GameObject, EntityFSM> actorFSMMap;
		
		public Combo ()
		{
			fsms = new List<EntityFSM>();
			actors = 0;
			actorFSMMap = new Dictionary<GameObject, EntityFSM>();
		}
		
		public Combo (List<EntityFSM> FSMS)
		{
			fsms = FSMS;
			actors = FSMS.Count;
			actorFSMMap = new Dictionary<GameObject, EntityFSM>();
		}
		
		public void SetCommander (Commander comndr)
		{
			commander = comndr;
		}
		
		//TODO generalize action bar to be a collection of states
		public void AddFSM (EntityFSM fsm, Func<GameObject, int> eval)
		{
			fsms.Add(fsm);
			actors += 1;
		}
		
		public void Run ()
		{
			foreach (GameObject entity in actorFSMMap.Keys)
			{
				MonoBehaviourFSM newFSM = entity.AddComponent<MonoBehaviourFSM>();
				newFSM.Initialize(actorFSMMap[entity], commander);
			}
		}
		
		public bool Assign (ICollection<Minion> actors)
		{
			return Assign(Utilities.Map<Minion, GameObject, HashSet<GameObject>>(actors, (a) => {return a.second;}));
		}
		
		//no need to filter out useless actors because the commander does that for us		
		bool Assign (ICollection<GameObject> actors)
		{			
			/* Find best combo grouping. */
			HashSet<List<GameObject>> allPerms = Utilities.GetAllPermutations<GameObject>(actors);
			List<GameObject> best = null;
			float bestScore = 0;
			
			foreach (List<GameObject> perm in allPerms)
			{
				float score = EvaluateAssignment(perm);
				
				if (bestScore < score)
				{
					bestScore = score;
					best = perm;
				}
			}
			
			if (bestScore <= 0)
				return false;
				
			//Perform the assignment
			int i = 0;
			
			foreach (GameObject actionBar in best)
			{
				actorFSMMap[actionBar] = fsms[i];
				i++;
			}
			
			return true;
		}
		
		public bool AssignInOrder (List<GameObject> actors)
		{
			int i = 0;
			
			foreach (GameObject actor in actors)
			{
				actorFSMMap[actor] = fsms[i];
				i++;
			}
			
			return true;
		}
		
		public float EvaluateAssignment (List<GameObject> actors)
		{			
			if (fsms.Count != actors.Count)
				return 0.0f;
				
			float sum = 0;
			int i = 0;

			foreach (EntityFSM fsm in fsms)
			{
				//If one of the actors can't use the fsm assigned then return 0
				if (!CanUse(actors[i], fsms[i]))
					return 0.0f;
					
				sum += fsm.Rate(actors[i], Utilities.dummy);
				i++;
			}
			
			return sum;
		}
		
		public bool CanUse (GameObject actor, EntityFSM fsm)
		{
			EntityDescriptor actorDescr = MyMonoBehaviour.memoizer.GetMemoizedComponent<EntityDescriptor>(actor);
			AbilityActionBar actorActionBar = actorDescr.abilityBar;
			HashSet<Ability> requiredAbilities = new HashSet<Ability>();
			
			foreach (Utilities.Container<Ability> item in fsm.states)
			{
				requiredAbilities.Add(item.data);
			}
			
			requiredAbilities.ExceptWith(MyMonoBehaviour.abilityDatabase.universalAbilities);
			return requiredAbilities.IsSubsetOf(actorActionBar.GetAllAbilities());
		}
	}
}
