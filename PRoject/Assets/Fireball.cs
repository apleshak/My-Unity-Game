using UnityEngine;
using System.Collections;

public class Fireball : AbilityFSM 
{
	public float distanceRemaining;
	public float damage;
	public Vector3 direction;
	public Vector3 spawnLocation;
	
	public enum states
	{
		Flying,
		Hitting,
		Finish
	};
	
	// Use this for initialization
	void Start () 
	{
		//prefab = memoizer.GetMemoizedPrefab("Abilities/Fireball", "Fireball");
		//direction = 
	}
	
	void FlyingUpdate ()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
