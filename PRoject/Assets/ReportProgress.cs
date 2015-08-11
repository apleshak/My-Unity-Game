using UnityEngine;
using System.Collections;

/* Ensures we know when an animation exists within some % of its completion. */
public class ReportProgress : StateMachineBehaviour 
{	
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.IsName("Throw"))
		{
			if (stateInfo.normalizedTime > 0.5f && stateInfo.normalizedTime < 0.6f)
			{
				animator.SetBool("ReadyToThrow", true);
			}
			else
			{
				animator.SetBool("ReadyToThrow", false);
			}
		}
		else if (stateInfo.IsName("Slash"))
		{
			if (stateInfo.normalizedTime > 0.5f && stateInfo.normalizedTime < 0.6f)
			{
				animator.SetBool("SlashReadyToHit", true);
			}
			else
			{
				animator.SetBool("SlashReadyToHit", false);
			}
		}
		else if (stateInfo.IsName("LeftPunch"))
		{
			if (stateInfo.normalizedTime > 0.45f && stateInfo.normalizedTime < 0.55f)
			{
				animator.SetBool("LeftPunchReadyToHit", true);
			}
			else
			{
				animator.SetBool("LeftPunchReadyToHit", false);
			}
		}
		else if (stateInfo.IsName("RightPunch"))
		{
			if (stateInfo.normalizedTime > 0.45f && stateInfo.normalizedTime < 0.55f)
			{
				animator.SetBool("RightPunchReadyToHit", true);
			}
			else
			{
				animator.SetBool("RightPunchReadyToHit", false);
			}
		}
	}
}
