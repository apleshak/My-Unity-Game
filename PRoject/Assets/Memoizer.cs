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
	Dictionary<string, Delegate> methodMemoizer;
	
	public Memoizer()
	{
		componentMemoizer = new Dictionary<GameObject, Dictionary<Type, UnityEngine.Object>>();
		prefabMemoizer = new Dictionary<string, UnityEngine.Object>();
		methodMemoizer = new Dictionary<string, Delegate>();
	}
	
	//TODO move this out of here since its buggy when a component is destroyed and the reference is not updated
	public T GetMemoizedComponent<T> (GameObject target) where T : Component
	{	
		UnityEngine.Object obj;
		
		if (!componentMemoizer.ContainsKey(target))
		{
			componentMemoizer[target] = new Dictionary<Type, UnityEngine.Object>();
		}
		
		//no such component - extract it and add it to the dictionary
		if (!componentMemoizer[target].ContainsKey(typeof(T)))
		{
			T component = target.GetComponent<T>();
			obj = component;
			componentMemoizer[target][typeof(T)] = obj;
		}
		//component found but its invalid - extract it and add it to the dictionary
		else if (componentMemoizer[target][typeof(T)] == null)
		{
			T component = target.GetComponent<T>();
			obj = component;
			componentMemoizer[target][typeof(T)] = obj;
		}
		else //component found and valid - return it
		{
			obj = componentMemoizer[target][typeof(T)];
		}
		
		return (T)obj;
	}
	
	public T GetMemoizedComponentInChildren<T> (GameObject target) where T : Component
	{
		Transform t = target.transform;
		
		for (int i = 0; i < t.childCount; i++)
		{
			T obj = GetMemoizedComponent<T> (t.GetChild(i).gameObject);
			
			if (obj != null)
				return obj;
		}
		
		return null;
	}
	
	public UnityEngine.Object GetMemoizedPrefab (string name)
	{
		UnityEngine.Object prefab;

		if (!prefabMemoizer.ContainsKey(name))
		{
			prefab = Resources.Load(name);
			prefabMemoizer[name] = prefab;
		}
		else
		{
			prefab = prefabMemoizer[name];
		}
		
		return prefab;
	}
	
	public UnityEngine.Object GetMemoizedAbilityPrefab (string resourcePath, string name)
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
	
	
	public T GetMemoizedMethod<T> (string name) where T : class
	{
		if (!methodMemoizer.ContainsKey(name))
		{
			MethodInfo newMethodInfo = typeof(T).GetMethod(name);
			
			// Found a method! Enter it into the dictonary and return it.
			if (newMethodInfo != null)
			{
				T newDelegate = System.Delegate.CreateDelegate(typeof(T), this, newMethodInfo) as T;
				methodMemoizer[name] = newDelegate as System.Delegate;
				return methodMemoizer[name] as T;
			}
			else
			{
				return null;
			}
		}
		
		return methodMemoizer[name] as T;
	}
}

