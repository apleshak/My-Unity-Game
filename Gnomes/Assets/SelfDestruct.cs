using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour 
{
	public float duration;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (duration <= 0.0)
		{
			Destroy(gameObject);
		}
		
		duration -= Time.deltaTime;
	}
}
