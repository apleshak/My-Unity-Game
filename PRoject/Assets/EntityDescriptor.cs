using UnityEngine;
using System.Collections;

/* 	This is the "model" of every entity. Holds any and all relevant information about an entity.
	Holds entity stats and abilityBar. Communicates with animator to facilitate animations. */
	/* NEVER MAKE THIS INHERIT FROM MyMonoBehaviour - there will be some cirlularity and infintie hangs. */
public class EntityDescriptor : MonoBehaviour
{
	/* Used to calculate speed. */
	Vector3 lastPos;
	Vector3 currPos;
	
	/* 	Used to control the animator movement animations. Since MovementCC already scales movement 
		by Time.deltaTime, we don't need to divide by it here? */
	public float speed
	{
		get
		{
			return ((currPos - lastPos) / Time.deltaTime).magnitude;
		}
	}
	
	public int ID;
	public float hp;
	/* For now needs to be set externally. Maybe in the future we can set this from a databse at Awake. */
	public AbilityActionBar abilityBar;
	/* Talk to this to control the animation. */
	public Animator animator;
	/* TODO: May be redundant...unsure.*/
	public bool isDead;
	public bool isStunned;
	public bool isKnockedDown;
	
	/* 
		Returns a float indicating the level of danger for a to interact with b. The
	   	value has no meaning aside from comparison purposes.
	*/
	public static float CompareDanger (EntityDescriptor a, EntityDescriptor b)
	{
		return b.hp - a.hp;
	}
	
	/* Returns a float indicating how well a can hear b. */
	public static float CompareAudibility (EntityDescriptor a, EntityDescriptor b)
	{
		return 1 / Vector3.Distance(a.gameObject.transform.position, b.gameObject.transform.position);
	}
	
	public void TakeDamage (float value)
	{
		hp -= value;
		animator.SetTrigger("TakeDamage");
	}
	
	public bool InAttackRange (GameObject target)
	{
		return false;
	}
	
	public void Attack (GameObject target)
	{
		animator.SetBool("isAttacking", true);
	}
	
	public bool IsAttacking ()
	{
		return false;
	}
	
	// Use this for initialization
	void Start () 
	{
		isDead = false;
		lastPos = transform.position;
		currPos = lastPos;
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		/* Update private position data. */
		lastPos = currPos;
		currPos = transform.position;
		
		/* Update animator parameter. */
		animator.SetFloat("moveSpeed", speed);
	}
}
