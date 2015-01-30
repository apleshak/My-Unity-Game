/* A finite state machine class for Unity */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class AbilityFSM : FiniteStateMachineBehaviour
{
	public Ability containingAbility;
}

/* FiniteStateMachine base class for game entities (hence the derival from MonoBehaviour). */
public class FiniteStateMachineBehaviour : MonoBehaviour
{
	#region DECLARATIONS AND INSTANTIATIONS
	/* Setup method dictionary and default all methods to do nothing */
	public Dictionary<string, Delegate> mtdDictionary = new Dictionary<string, Delegate>();
	public Action myUpdate = EmptyFunction;
	public Action<Collider> myOnTriggerEnter = EmptyCollider;
	public Action<Collider> myOnTriggerStay = EmptyCollider;
	public Action<Collider> myOnTriggerExit = EmptyCollider;
	public Action<Collision> myOnCollisionEnter = EmptyCollider;
	public Action<Collision> myOnCollisionStay = EmptyCollider;
	public Action<Collision> myOnCollisionExit = EmptyCollider;
	public Action myFreeAction = EmptyFunction;
	public Func<IEnumerator> EnterState = EmptyCoroutine;
	public Func<IEnumerator> ExitState = EmptyCoroutine;
	public Enum currState;

	#endregion

	/* Performs transition routines when set. */
	public Enum currentState
	{
		get
		{
			return currState;
		}
		set
		{
			currState = value;
			updateState();
		}
	}
	
	/* Facilitates the transition between states. */
	void updateState()
	{
		/* Run anything that needed to be done before we can exit. */
		if (ExitState != null)
		{
			StartCoroutine(ExitState());
		}
	
		/* Set all my methods to the ones specified in the child class that derives
		   from this one. Also supply a default empty function to run if the method
		   is not defined. */
		myUpdate = updateDelegate<Action>("Update", EmptyFunction);
		myOnTriggerEnter = updateDelegate<Action<Collider>>("OnTriggerEnter", EmptyCollider);
		myOnTriggerStay = updateDelegate<Action<Collider>>("OnTriggerStay", EmptyCollider);
		myOnTriggerExit = updateDelegate<Action<Collider>>("OnTriggerExit", EmptyCollider);
		myOnCollisionEnter = updateDelegate<Action<Collision>>("OnCollisionEnter", EmptyCollider);
		myOnCollisionStay = updateDelegate<Action<Collision>>("OnCollisionStay", EmptyCollider);
		myOnCollisionExit = updateDelegate<Action<Collision>>("OnCollisionExit", EmptyCollider);
		EnterState = updateDelegate<Func<IEnumerator>>("enterState", EmptyCoroutine);
		ExitState = updateDelegate<Func<IEnumerator>>("exitState", EmptyCoroutine);

		/* Run anything that needs to be done before we can enter. */
		if (EnterState != null)
		{
			StartCoroutine(EnterState());
		}
	}

	/* Facilitates method binding. The "where" constraint is not crucial */
	T updateDelegate<T>(string mtdName, T Default) where T : class
	{
		/* The name of the method we will bind is derived from the state. */
		string newMtdName = currState.ToString() + mtdName;
		T newMethod = mtdDictionary[newMtdName] as T;

		/* Attempt to find method pointer in method dictionary. */
		if (newMethod != null)
		{
			return newMethod;
		}
		/* Otherwise try to find it with reflection. GetType() gets this class type. */
		else
		{
			MethodInfo newMethodInfo = GetType().GetMethod(newMtdName) as MethodInfo;

			/* Found a method! Enter it into the dictonary and return it. */
			if (newMethodInfo != null)
			{
				Delegate newDelegate = Delegate.CreateDelegate(typeof(T), this, newMethodInfo);
				mtdDictionary[newMtdName] = newDelegate;
				return newMethod;
			}
			/* Otherwise return the default method supplied. */
			else
			{
				return Default;
			}
		}
	}

	#region EMPTY METHODS

	static void EmptyFunction () 
	{
		return;
	}

	static void EmptyCollider (Collider collider)
	{
		return;
	}

	static void EmptyCollider (Collision collision)
	{
		return;
	}

	static IEnumerator EmptyCoroutine ()
	{
		yield break;
	}

	#endregion

	#region PASS-THROUGH METHODS

	void Update()
	{
		myUpdate();
	}

	void OnTriggerEnter(Collider other)
	{
		myOnTriggerEnter(other);
	}

	void OnTriggerStay(Collider other)
	{
		myOnTriggerStay(other);
	}

	void OnTriggerExit(Collider other)
	{
		myOnTriggerExit(other);
	}

	void OnCollisionEnter(Collision other)
	{
		myOnCollisionEnter(other);
	}
	
	void OnCollisionStay(Collision other)
	{
		myOnCollisionStay(other);
	}
	
	void OnCollisionExit(Collision other)
	{
		myOnCollisionExit(other);
	}

	#endregion
}
