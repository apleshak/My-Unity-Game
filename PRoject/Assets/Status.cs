using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class Effect : IComparable<Effect>
{
	public int id;
	public string description;
	/* Main runs every frame. Maybe coroutines are better. */
	public Type effect;
	
	public virtual void apply (GameObject user)
	{
		user.AddComponent(effect);
	}
	
	public virtual void suspend (GameObject user)
	{
		
	}
	
	public virtual void expire (GameObject user)
	{
		
		GameObject.Destroy(user.GetComponent(effect));
	}
	
	/* Compares by id. */
	public virtual int CompareTo (Effect other)
	{
		return Comparer<int>.Default.Compare(id, other.id);
	}
}

/* 
   A temporary effect is visible in the status bar below and usually affect
   gameplay in a minor way like hp regen or speedup or status cleansing.
*/
public class TemporaryEffect : Effect
{
	public bool visible;
	public Texture2D icon;
	public float duration;
	
	public TemporaryEffect (int Id, string Description, Texture2D Icon, bool Visible, float Duration,
	                        Type effectToUse)
	{
		id = Id;
		description = Description;
		icon = Icon;
		visible = Visible;
		duration = Duration;
		effect = effectToUse;
	}
	
	public override void apply (GameObject target)
	{
		base.apply(target); //use old apply and modify the return to get the component and work with it
	}
}

/* 
   A permanent effect is only visible on the player model as an extra GameObject. 
   Usually affects game mechanics in a major way, like altering perception.
*/
public class PermanentEffect : Effect
{
	public GameObject prefab;
	public GameObject parent;
	
	public PermanentEffect (int Id, GameObject Prefab, GameObject Parent, string Description,
							Type effectToUse)
	{
		id = Id;
		description = Description;
		prefab = Prefab;
		parent = Parent;
		effect = effectToUse;
	}
}

/* 
   Serves as a controller for effects and endows them with a user (this gameObject)
   Governs death, buffs, hp, mp and so on. No duplicate effects allowed. 
*/
public class Status : MonoBehaviour
{
	public bool isDead;
	public float armor;
	public float hp;
	public float maxHp;
	public float speed;
	public float baseSpeed;
	public float maxSpeed;
	public HashSet<PermanentEffect> permanentEffects;
	public HashSet<TemporaryEffect> temporaryEffects;
	public float lastTime;
	
	// Use this for initialization
	void Start ()
	{
		armor = 0.0f;
		lastTime = 0.0f;
		isDead = false;
		hp = 100.0f;
		speed = 1.0f;
		baseSpeed = speed;
		permanentEffects = new HashSet<PermanentEffect>();
		temporaryEffects = new HashSet<TemporaryEffect>();
	}
	
	// Update is called once per frame
	void Update ()
	{		
		/* Edge cases. */
		if (hp <= 0.0f)
		{
			hp = 0.0f;
			isDead = true;
		}
		else if (hp > maxHp)
		{
			hp = maxHp;
		}
		
		if (speed < 0.0f)
		{
			speed = 0.0f;
		}
		else if (speed > maxSpeed)
		{
			speed = maxSpeed;
		}
	}
	
	public void dealDamage (float amount)
	{
		hp -= amount - armor;
	}
	
	public void addPermanentEffect (PermanentEffect newEffect)
	{
		if (!permanentEffects.Contains(newEffect))
		{
			permanentEffects.Add(newEffect);
		}
		
	}
	
	public void addTemporaryEffect (TemporaryEffect newEffect)
	{
		if (!temporaryEffects.Contains(newEffect))
		{
			temporaryEffects.Add(newEffect);
		}
	}
}

