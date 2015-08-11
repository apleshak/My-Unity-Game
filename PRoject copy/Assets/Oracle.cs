using System.Collections.Generic;

/* Anything that uses states and can be predicted. Use for ActionBar. */
public class Stateful<T>
{
	public T currState;
}

/* 
	Communicates with ActionBar script on Player object's NonMonoScriptContainer
	script. ActionBar derives from Stateful. Since its not MonoBehaviour its 
	Update method needs to be invoked externally.
	
	T is the type of state the target can occupy.
*/
public class Oracle<T> where T : class
{
	MarkovChain<T, List<Tuple<T, int>>> report;
	Stateful<T> target;
	T lastState;
	
	/* Used by Markov chain. */
	public float errorMargin
	{
		get
		{
			return report.errorMargin;
		}
		set
		{	
			report.errorMargin = value;
		}
	}
	
	public Oracle (Stateful<T> Target)
	{
		/* errorMargin set to 0.0f must be done only after the report is instantioated. */
		report = new HashTableMarkovChain<T>(0.0f);
		errorMargin = 0.0f;
		target = Target;
		lastState = default(T);
	}
	
	/* 
		Queries player ActionBar for changes in its state recording them to report. Also
		updates lastState. Should be called from the Update of a MonoBehaviour object.
	*/
	public void Update ()
	{
		if (target.currState != lastState)
		{
			report.recordTransition(lastState, target.currState);
			lastState = target.currState;
		}
	}
	
	/* Assumes that lastState is still valid. */
	public T nextState
	{
		get
		{
			return report.nextState(lastState);
		}
		set
		{
			/* Should not need to be manipulated, but just in case. */
			lastState = value;
		}
	}
}
