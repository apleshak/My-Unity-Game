using UnityEngine;
using System.Collections.Generic;

public class Memoizer
{
	Dictionary<GameObject, Dictionary<System.Type, System.Object>> componentMemoizer;
	Dictionary<string, Object> prefabMemoizer;
	
	public Memoizer()
	{
		componentMemoizer = new Dictionary<GameObject, Dictionary<System.Type, System.Object>>();
		prefabMemoizer = new Dictionary<string, Object>();
	}
	
	public T getMemoizedComponent<T> (GameObject target) where T : Component
	{	
		System.Object obj;
		
		if ((obj = componentMemoizer[target][typeof(T)]) == null)
		{
			T component = target.GetComponent<T>();
			obj = component;
			componentMemoizer[target][typeof(T)] = obj;
		}
		
		return (T)obj;
	}
	
	public Object getMemoizedPrefabs (string name)
	{
		Object prefab;
		
		if ((prefab = prefabMemoizer[name]) == null)
		{
			prefab = Resources.Load("Abilities/" + name + "/" + name);
			prefabMemoizer[name] = prefab;
		}
		
		return prefab;
	}
}

