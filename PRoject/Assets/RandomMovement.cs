using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomMovement : MonoBehaviour 
{
	List<MovementCC> entities;
	float timeCounter;
	public float interval;
	public GameObject test;
	
	// Use this for initialization
	void Start () 
	{
		entities = new List<MovementCC>();
		entities.Add(test.GetComponent<MovementCC>());
		
		/*
		foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
		{
			MovementCC mov = obj.GetComponent<MovementCC>();
			
			if (mov != null)
			{
				entities.Add(mov);
			}
		}
		*/
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeCounter += Time.deltaTime;
		
		if (timeCounter >= interval)
		{
			foreach (MovementCC entity in entities)
			{
				Vector3 heading = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
				Vector3 target = entity.gameObject.transform.position + heading.normalized * 5;
				
				entity.Approach(target, EntityBehaviour.movementModes.Slow);
			}
			
			timeCounter = 0.0f;
		}
	}
}
