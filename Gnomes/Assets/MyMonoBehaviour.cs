using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyMonoBehaviour : MonoBehaviour 
{
	/* Utilities - only safe to access after Awake finishes initializing non-MonoBehavious. */
	public static bool showDebug = true;
	public static MyMonoBehaviour instance;
	
	/* Non-MonoBehaviours - stored here. */
	public static Memoizer memoizer;
	
	/* Necessary to avoid multiple instantiations. */
	static bool initializedNonMonos = false;
	
	/* Instantiates non-MonoBehaviours and utilities. */
	void Awake ()
	{
		if (!initializedNonMonos)
		{
			if (showDebug) Debug.Log("_____MyMonoBehaviour: Performing first-time initialization_____");
			
			/* Utilities */
			instance = this;
			InstantiateDatabases();
			
			initializedNonMonos = true;
			
			if (showDebug) Debug.Log("_____MyMonoBehaviour: First-time initialization complete_____");
		}
	}
	
	/* Updates all non-MonoBehaviours. */
	void Update ()
	{
		
	}
	
	/* After this we can instatntiate abilities.*/
	void InstantiateDatabases ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating databases...");
		
		memoizer = new Memoizer();
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating databases!");
	}
}
