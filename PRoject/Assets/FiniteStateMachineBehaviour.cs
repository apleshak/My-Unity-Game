/* A finite state machine class for Unity */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/* Interfaces between ability and the action bar that holds it. */
public class AbilityFSM : FiniteStateMachineBehaviour
{
	public Ability containingAbility;
}

/* FiniteStateMachine base class for game entities (hence the derival from MonoBehaviour). */
public class FiniteStateMachineBehaviour : MyMonoBehaviour
{
	#region DECLARATIONS AND INSTANTIATIONS
	/* Setup method dictionary and default all methods to do nothing */
	public Dictionary<string, Delegate> methodMemoizer = new Dictionary<string, Delegate>();
	public Action myUpdate = EmptyFunction;
	public Action<Collider> myOnTriggerEnter = EmptyCollider;
	public Action<Collider> myOnTriggerStay = EmptyCollider;
	public Action<Collider> myOnTriggerExit = EmptyCollider;
	public Action<Collision> myOnCollisionEnter = EmptyCollision;
	public Action<Collision> myOnCollisionStay = EmptyCollision;
	public Action<Collision> myOnCollisionExit = EmptyCollision;
	public Action myFreeAction = EmptyFunction;
	public Func<IEnumerator> EnterState = EmptyCoroutine;
	public Func<IEnumerator> ExitState = EmptyCoroutine;
	
	/* Ensures the speed of transitions is independent of the framerate. */
	public float transitionInterval;
	private float timeSinceTransition;
	private bool canTransition;
	
	private Enum currState;

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
		myOnCollisionEnter = updateDelegate<Action<Collision>>("OnCollisionEnter", EmptyCollision);
		myOnCollisionStay = updateDelegate<Action<Collision>>("OnCollisionStay", EmptyCollision);
		myOnCollisionExit = updateDelegate<Action<Collision>>("OnCollisionExit", EmptyCollision);
		EnterState = updateDelegate<Func<IEnumerator>>("enterState", EmptyCoroutine);
		ExitState = updateDelegate<Func<IEnumerator>>("exitState", EmptyCoroutine);

		/* Run anything that needs to be done before we can enter. */
		if (EnterState != null)
		{
			StartCoroutine(EnterState());
		}
		
		timeSinceTransition = 0.0f;
		canTransition = false;
	}
	
	/* Facilitates method binding. */
	T updateDelegate<T> (string mtdName, T Default) where T : class
	{
		T method;
		//Debug.Log("Attempting type method lookup:" + currentState.ToString() + mtdName);
		if((method = getMemoizedMethod<T>(currentState.ToString() + mtdName)) != null)
		{
			return method;
		}
		
		return Default;
	}
	
	/* Improvement can skip searching for unfound methods on objects of the same type as seen before. */
	T getMemoizedMethod<T> (string name) where T : class
	{
		if (!methodMemoizer.ContainsKey(name))
		{
			MethodInfo newMethodInfo = GetType().GetMethod(name);
			
			/* Found a method! Enter it into the dictonary and return it. */
			if (newMethodInfo != null)
			{
				//Debug.Log("Found method!");
				T newDelegate = System.Delegate.CreateDelegate(typeof(T), this, newMethodInfo) as T;
				methodMemoizer[name] = newDelegate as System.Delegate;
				return methodMemoizer[name] as T;
			}
			else
			{
				return null;
			}
		}

		Debug.Log("Found memoized method!");
		return methodMemoizer[name] as T;
	}
	
	public void TerminateFSM ()
	{
		Destroy(this);
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

	static void EmptyCollision (Collision collision)
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
		if ((timeSinceTransition += Time.deltaTime) > transitionInterval)
		{
			canTransition = true;
		}
		
		if (canTransition) myUpdate();
	}

	void OnTriggerEnter(Collider other)
	{
		if (canTransition) myOnTriggerEnter(other);
	}

	void OnTriggerStay(Collider other)
	{
		if (canTransition) myOnTriggerStay(other);
	}

	void OnTriggerExit(Collider other)
	{
		if (canTransition) myOnTriggerExit(other);
	}

	void OnCollisionEnter(Collision other)
	{
		if (canTransition) myOnCollisionEnter(other);
	}
	
	void OnCollisionStay(Collision other)
	{
		if (canTransition) myOnCollisionStay(other);
	}
	
	void OnCollisionExit(Collision other)
	{
		if (canTransition) myOnCollisionExit(other);
	}

	#endregion
}
