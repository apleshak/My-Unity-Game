using UnityEngine;
using System.Collections;

/* Ensures the animator knows when we are done with a transition animation. */
public class ReportCompletion : StateMachineBehaviour 
{
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.IsName("Crouch"))
		{
			animator.SetBool("Crouched", true);
		}
		else if (stateInfo.IsName("Sneak"))
		{
			animator.SetBool("Sneaking", true);
		}
		else if (stateInfo.IsName("Throw"))
		{
			animator.SetBool("Throwing", false);
		}
	}
}
