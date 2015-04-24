using UnityEngine;
using System.Collections;

public class Movement : FiniteStateMachineBehaviour
{
	bool inputLocked;
	public enum states
	{
		Stopped,
		Turning,
		Moving
	};
	
	void Start ()
	{
		inputLocked = false;
	}
	
	public void lockInput()
	{
		inputLocked = true;
	}
	
	public void unlockInput()
	{
		inputLocked = false;
	}
	
	public void move(float distance)
	{
		gameObject.transform.position += transform.forward * distance;
	}
}