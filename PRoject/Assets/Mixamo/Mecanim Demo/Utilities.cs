using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/* All sorts of useful generic algorithms and goodies. */
public static class Utilities
{
	public static int StupidEval (HashSet<int> set)
	{
		if (set.Count == 0)
		{
			return 0;
		}
		else
		{
			int sum = 1;
			
			foreach (int item in set)
			{
				sum *= item;
			}
			
			return sum;
		}
	}
	
	/*Returns all possible subsets of the collection using bit vector mask method. */
	public static HashSet<T> GetAllSubsets<T, K> (T collection) where T : ICollection<K>, new()
	{
		int maxPow = collection.Count;
		HashSet<T> allSubsets = new HashSet<T>();
		
		for (int i = 0; i < Math.Pow(2, maxPow); i++)
		{
			int j = i;
			int index = 0;
			T subset = new T();
			
			while (j > 0)
			{
				if (j % 2 == 1)
				{
					subset.Add(collection.ElementAt<K>(index));
				}
				
				index += 1;
				j /= 2;
			}
			
			allSubsets.Add(subset);
		}
		
		return allSubsets;
	}
	
	/* 	Takes set of items and a function that scores a set of items. Returns a set of sets s.t. the sum of eval calls
		on them is maximized. */
	public static Tuple<HashSet<HashSet<T>>, int> BestGrouping<T> (HashSet<T> items, Func<HashSet<T>, int> eval)
	{
		Tuple<HashSet<HashSet<T>>, int> best = new Tuple<HashSet<HashSet<T>>, int>(new HashSet<HashSet<T>>(), 0);
		
		/* Base case. */
		if (items.Count == 0)
		{
			return best;
		}
		/* Recursive case. */
		else
		{
			HashSet<HashSet<T>> allSubSets = GetAllSubsets<HashSet<T>, T>(items);
			
			foreach (HashSet<T> set in allSubSets)
			{
				/* Don't recurse on empty set - it will cause an infintie loop. */
				if (set.Count > 0)
				{
					/* Get set of all other items. */
					HashSet<T> otherItems = new HashSet<T>(items.Except<T>(set));
					Tuple<HashSet<HashSet<T>>, int> bestRecursive = BestGrouping<T> (otherItems, eval);
					
					/* Improved solution found. */
					if (eval(set) + bestRecursive.second > best.second)
					{
						bestRecursive.first.Add(set);
						best.first = bestRecursive.first;
						best.second = eval(set) + bestRecursive.second;
					}
				}
				/* Evaluate the empty set here. */
				else
				{
					int value = eval(items);
					
					/* Improved solution found. */
					if (value > best.second)
					{
						best.first.Add(items);
						best.second = value;
					}
				}
			}
			
			return best;
		}
	}
	
	public static void PrintCollection<K> (ICollection<K> collection)
	{
		string line = "[";

		foreach (K item in collection)
		{
			line += item.ToString() + ", ";
		}
		
		line += "]\n";
		Debug.Log(line);
	}
	
	/* K is the inner type, T is the enlosing container type. */
	public static void PrintCollection<T, K> (ICollection<T> collection)
	{
		string line = "";
		Debug.Log ("[");
		
		foreach (T item in collection)
		{
			if (item is ICollection<K>)
			{
				PrintCollection<K>(item as ICollection<K>);
			}
			else
			{
				line += item.ToString() + ", ";
			}
		}
		
		Debug.Log(line);
		Debug.Log("]\n");
	}
}
