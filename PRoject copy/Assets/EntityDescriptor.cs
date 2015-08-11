using UnityEngine;
using System.Collections;

/* Holds entity stats. */
public class EntityDescriptor : MyMonoBehaviour
{
	public bool isDead;
	
	/* 
		Returns a float indicating the level of danger for a to interact with b. The
	   	value has no meaning aside from comparison purposes.
	*/
	public static float CompareDanger (EntityDescriptor a, EntityDescriptor b)
	{
		return 42;
	}
	
	public static float CompareAudibility (EntityDescriptor a, EntityDescriptor b)
	{
		return 42;
	}
	
	public bool InAttackRange (GameObject target)
	{
		return false;
	}
	
	public void Attack (GameObject target)
	{
	
	}
	
	public bool IsAttacking ()
	{
		return false;
	}
	
	// Use this for initialization
	void Awake () 
	{
		isDead = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
