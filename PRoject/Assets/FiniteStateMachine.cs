/*
	Generic Finite State Machine
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EntityFSM : ValidatorContainerFSM<Ability>
{
	public EntityFSM () : base()
	{
		
	}
	
	//This method is called in the inherited method FromEdges to set up a dummy node to start from so that
	//the first call to Advance will return state instead of some other state
	public override void Initialize (Utilities.Container<Ability> state)
	{
		base.Initialize(state);
		Ability dummy = MyMonoBehaviour.abilityDatabase.GetAbility("Dummy Ability");
		Utilities.Validator validByDefault = new Utilities.Validator(true);
		Utilities.Container<Ability> dummyContainer = new Utilities.Container<Ability>(dummy);
		AddTransition(new Utilities.Container<Ability>(dummy), state, validByDefault);
		base.Initialize(dummyContainer);
	}
	
	/* Tests all possible travelsals of the FSM to see if the user can execute at least one traversal. */
	public bool Test (GameObject user, GameObject target)
	{
		return Test(user, target, GetCurrentStateBase());
	}
	
	bool Test (GameObject user, GameObject target, Utilities.Container<Ability> initState)
	{
		//Base case one - this state is unsuccessful
		if (!initState.data.Test(user, target))
			return false;
		
		//Base case two - terminal state reached
		if (transitionMap[initState].Count == 0)
			return true;
			
		//Try to recursively solve on each possible next state
		foreach (Transition transition in transitionMap[initState])
		{
			//Apply state effects to dummy
			GameObject dummy = Utilities.dummy;
			dummy.transform.position = user.transform.position;
			dummy.transform.forward = user.transform.forward;
			transition.to.data.CommitToDummy(dummy, target);
			
			//Recurse with dummy as user
			if (Test(dummy, target, transition.to))
				return true;
		}
		
		//Base case three - no transitions successfull
		return false;
	}
	
	/* Counts the number of travelsals the user can achieve in the FSM, then normalize. */
	public float Rate (GameObject user, GameObject target)
	{
		return CountStatesReachable(user, target, GetCurrentStateBase(), 0) / (float)states.Count;
	}
	
	int CountStatesReachable (GameObject user, GameObject target, Utilities.Container<Ability> initState, int accum)
	{
		//Base case one - this state is impossible to achieve
		if (!initState.data.Test(user, target))
		{
			return 0;
		}
		else //State achievable - count it
		{
			accum++;
		}
		
		foreach (Transition transition in transitionMap[initState])
		{
			//Apply state effects to dummy
			GameObject dummy = Utilities.dummy;
			dummy.transform.position = user.transform.position;
			dummy.transform.forward = user.transform.forward;
			transition.to.data.CommitToDummy(dummy, target);
			
			//Recurse with dummy as user
			accum += CountStatesReachable(dummy, target, transition.to, accum);
		}
		
		return accum;
	}
}

public class ValidatorContainerFSM<S> : FiniteStateMachine<Utilities.Container<S>>, IStateful<S>
{	
	Dictionary<Utilities.Container<S>, Utilities.Validator> validatorMap;

	public ValidatorContainerFSM ()
	{
		validatorMap = new Dictionary<Utilities.Container<S>, Utilities.Validator>();
	}
	
	public virtual void Initialize (Utilities.Container<S> state)
	{
		base.Initialize(state);
	}
	
	//Also Initializes
	public void FromEdges (ICollection<Tuple<S, S, Utilities.Validator>> inputCollection)
	{
		foreach (Tuple<S, S, Utilities.Validator> tuple in inputCollection)
		{
			Utilities.Container<S> fromState = new Utilities.Container<S>(tuple.first);

			//If this was the first state then make it the initial state
			if (validatorMap.Count == 0)
			{
				Initialize(fromState);
			}

			Utilities.Container<S> toState = new Utilities.Container<S>(tuple.second);
			
			//Associate the validator with the State it validates
			if (!validatorMap.ContainsKey(toState))
			{
				validatorMap[toState] = tuple.third;
			}
			
			// Add the transition to the FSM
			AddTransition(fromState, toState, tuple.third);
		}
	}
	
	public S GetCurrentState ()
	{
		return base.GetCurrentState().data;
	}
	
	public Utilities.Container<S> GetCurrentStateBase ()
	{
		return base.GetCurrentState();
	}
}

public class FiniteStateMachine<S> : IStateful<S> where S : IEquatable<S>, IEqualityComparer<S>
{
	public Dictionary<S, HashSet<Transition>> transitionMap;
	public ICollection<S> states {
		get {
			return transitionMap.Keys;
		}
	}
	public bool terminated;
	public bool initialized;
	S currState;
	
	public S GetCurrentState ()
	{
		return currState;
	}
	
	public FiniteStateMachine ()
	{
		transitionMap = new Dictionary<S, HashSet<Transition>>(default(S));
		currState = default(S);
		terminated = false;
		initialized = false;
	}
	
	public virtual void Initialize (S initialState)
	{
		currState = initialState;
		initialized = true;
	}
	
	public void AddTransitions (ICollection<Tuple<S, S, Utilities.Validator>> inputCollection)
	{
		HashSet<Transition> states = new HashSet<Transition>();
		
		foreach (Tuple<S, S, Utilities.Validator> tuple in inputCollection)
		{
			AddTransition(tuple.first, tuple.second, tuple.third);
		}
	}
	
	public void AddTransition (S from, S to, Utilities.Validator validator)
	{
		Transition newTransition = new Transition(to, validator);
		
		//Try to add first vertex
		if (!transitionMap.ContainsKey(from))
		{
			transitionMap[from] = new HashSet<Transition>();
		}
		
		//Try to add second vertex
		if (!transitionMap.ContainsKey(to))
		{
			transitionMap[to] = new HashSet<Transition>();
		}
		
		//Try to add the transition
		transitionMap[from].Add(newTransition);
	}
	
	//Returns true if fsm can advance to a new state and performs the transition
	//false otherwise
	public bool Advance ()
	{
		//If there is nowhere to transition to we set terminated to true
		if (transitionMap[currState].Count == 0)
		{
			terminated = true;
			return false;
		}
		
		//Otherwise we look for the first transition we can make and make it
		foreach (Transition transition in transitionMap[currState])
		{
			if (transition.CanTransition())
			{
				currState = transition.to;
				return true;
			}
		}
		
		return false;
	}

	public class Transition
	{
		public S to;
		Utilities.Validator canTransition;
		
		internal Transition (S targetState, Utilities.Validator validator)
		{
			to = targetState;
			canTransition = validator;
		}
		
		public bool CanTransition ()
		{
			return canTransition.valid;
		}
	}
}