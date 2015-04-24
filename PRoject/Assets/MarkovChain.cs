using System.Collections.Generic;
using System;

/* 
   T is the type of states we hold
   K is the container we supply T's in
*/
public abstract class MarkovChain<T, K> 
{
	/* errorMargin only diversifies nextState results. */
	public float errorMargin;
	/* Returns the next likeliest state. */
	public abstract T nextState (T currState);
	/* Removes state and all transitions associated. */
	public abstract void removeState (T removeState);
	/* Adds a state with specified to- and from states. */
	public abstract void insertState (T updateState, K toStates, K fromState);
	/* Ensures probabilities are maintained correctly with each transition. */
	public abstract void recordTransition (T fromState, T toState);
}

/* K must implement IComparable */
public class Tuple<T, K> : IComparable<Tuple<T,K>>
{
	public T first;
	public K second;

	public Tuple (T item1, K item2)
	{
		first = item1;
		second = item2;
	}
	
	/* Compares by second value */
	public int CompareTo (Tuple<T, K> other)
	{
		return Comparer<K>.Default.Compare(second, other.second);
	}
}

public class Tuple<T, K, L> : IComparable<Tuple<T, K, L>>
{
	public T first;
	public K second;
	public L third;
	
	/* Compares by second value */
	public int CompareTo (Tuple<T, K, L> other)
	{
		return Comparer<L>.Default.Compare(third, other.third);
	}
}

/* 
   Dictionary of dictionaries of integers. Accuracy can be changed on the fly.
   
   Uses a list of state, frequency tuples to build
   O(m) nextState, where m is the number of out states.
   O(n) removeState, where n is the size of the dictionary.
   O(k+m) insertState, where k is the number of from states.
   O(1) recordTransition.
*/
public class HashTableMarkovChain<T> : MarkovChain<T, List<Tuple<T, int>>>
{
	public Dictionary<T, Dictionary<T, int>> dict;
	
	public HashTableMarkovChain (float accuracy)
	{
		errorMargin = accuracy;
		dict = new Dictionary<T, Dictionary<T, int>>();
	}
	
	public override T nextState (T currState)
	{
		if (dict[currState] == null)
		{
			return default(T);
		}
		
		List<Tuple<T, int>> toStates = new List<Tuple<T, int>>();
		
		foreach (T state in dict[currState].Keys)
		{
			toStates.Add(new Tuple<T, int>(state, dict[currState][state]));
		}
		
		return mostLikelyState(toStates);
	}
	
	/* Randomly takes any state within error of most probable one. */
	T mostLikelyState (List<Tuple<T, int>> states)
	{
		states.Sort();
		int startIdx = states.Count-1;
		int endIdx = startIdx;
		int lastProb = states[endIdx].second;
		
		for (int i = endIdx; i >= 0; i--)
		{
			if ((lastProb - states[i].second)/lastProb <= errorMargin)
			{
				startIdx -= 1;
			}
		}

		return states[UnityEngine.Random.Range(startIdx, endIdx+1)].first;
	}
	
	/* Technically recordTransition also handles insertion but this is cleaner. */
	public override void insertState (T newState, List<Tuple<T, int>> toStates,
									  List<Tuple<T, int>> fromStates)
	{	
		foreach (Tuple<T, int> state in toStates) 
		{
			dict[newState][state.first] = state.second;
		}
		
		foreach (Tuple<T, int> state in fromStates) 
		{
			dict[state.first][newState] = state.second;
		}
	}
	
	public override void removeState (T removeState)
	{
		dict.Remove(removeState);
		
		foreach (T fromState in dict.Keys)
		{
			dict[fromState].Remove(removeState);
		}
	}
	
	public override void recordTransition (T fromState, T toState)
	{
		dict[fromState][toState] += 1;
	}
}