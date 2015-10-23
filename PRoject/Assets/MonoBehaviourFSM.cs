using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EntityAction = System.Func<UnityEngine.GameObject, UnityEngine.GameObject, System.Collections.IEnumerator>;
using Edge = Tuple<EntityAction, EntityAction, Utilities.Validator>;

/* Makes entity its attached act using its sences. */
public class MonoBehaviourFSM : MyMonoBehaviour, IStateful<Ability>
{
	EntityFSM fsm;
	Commander commander;
	AbilityActionBar actionBar;
	public float transitionInterval = 0.0f;
	float timeSinceTransition = 0.0f;
	public bool initialized
	{
		get
		{
			return fsm.initialized;
		}
	}
	
	public bool terminated
	{
		get
		{
			return fsm.terminated;
		}
	}
	
	public void Initialize (EntityFSM FSM, Commander cmndr)
	{
		fsm = FSM;
		commander = cmndr;
	}
	
	void Awake ()
	{
		EntityDescriptor descr = memoizer.GetMemoizedComponent<EntityDescriptor>(gameObject);
		actionBar = descr.abilityBar;
	}
	
	//If advanceing succeeds we execute the new state
	void Update () 
	{
		if (initialized && !terminated)
		{
			timeSinceTransition += Time.deltaTime;
			
			if (timeSinceTransition >= transitionInterval)
			{
				timeSinceTransition = 0.0f;
				
				if (fsm.Advance())
					Execute(fsm.GetCurrentState());
			}
		}
		else if (terminated)
		{
			commander.FreeMinion(new Tuple<AbilityActionBar, GameObject>(actionBar, gameObject));
			Destroy(this);
		}
	}
	
	//Execute the ability using the target prescribed by senses
	void Execute (Ability ability)
	{
		EntitySenses sences = memoizer.GetMemoizedComponentInChildren<EntitySenses>(gameObject);
		ability.execute(gameObject, sences.GetTarget(), actionBar);
	}
	
	public Ability GetCurrentState ()
	{
		return fsm.GetCurrentState();
	}
}
