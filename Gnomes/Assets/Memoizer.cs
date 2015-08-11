using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

/* */
public class Memoizer
{
	Dictionary<GameObject, Dictionary<System.Type, UnityEngine.Object>> componentMemoizer;
	Dictionary<string, UnityEngine.Object> prefabMemoizer;
	
	public Memoizer()
	{
		componentMemoizer = new Dictionary<GameObject, Dictionary<Type, UnityEngine.Object>>();
		prefabMemoizer = new Dictionary<string, UnityEngine.Object>();
	}
	
	//TODO move this out of here since its buggy when a component is destroyed and the rferecne is not updated
	public T GetMemoizedComponent<T> (GameObject target) where T : Component
	{	
		UnityEngine.Object obj;
		
		if (!componentMemoizer.ContainsKey(target))
		{
			componentMemoizer[target] = new Dictionary<Type, UnityEngine.Object>();
		}
		
		if (!componentMemoizer[target].ContainsKey(typeof(T)))
		{
			T component = target.GetComponent<T>();
			obj = component;
			componentMemoizer[target][typeof(T)] = obj;
		}
		else 
		{
			obj = componentMemoizer[target][typeof(T)];
		}
		
		return (T)obj;
	}
	
	public UnityEngine.Object GetMemoizedPrefab (string name)
	{
		UnityEngine.Object prefab;
		
		if (!prefabMemoizer.ContainsKey(name))
		{
			prefab = Resources.Load("Abilities/" + name + "/" + name);
			prefabMemoizer[name] = prefab;
		}
		else
		{
			prefab = prefabMemoizer[name];
		}
		
		return prefab;
	}
	
	public UnityEngine.Object GetMemoizedPrefab (string resourcePath, string name)
	{
		UnityEngine.Object prefab;
		
		if (!prefabMemoizer.ContainsKey(name))
		{
			prefab = Resources.Load("Abilities/" + resourcePath + "/" + name);
			prefabMemoizer[name] = prefab;
		}
		else
		{
			prefab = prefabMemoizer[name];
		}
		
		return prefab;
	}	
}

