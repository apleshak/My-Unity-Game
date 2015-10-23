using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//expand on this
public class EntityDatabase
{
	Dictionary<string, Entity> entityMap;
	
	public EntityDatabase ()
	{
		entityMap = new Dictionary<string, Entity>();
	}
	
	public void AddEntity (string name, AbilityActionBar actionBar)
	{
		entityMap[name] = new Entity(actionBar);
	}
	
	public AbilityActionBar GetEntityActionBar (string entityName)
	{
		return entityMap[entityName].GetAbilityBar();
	}
	
	public int Count ()
	{
		return entityMap.Count;
	}
	
	class Entity
	{
		public AbilityActionBar initActionBar;
		
		public Entity (AbilityActionBar actionBar)
		{
			initActionBar = actionBar;
		}
		
		//returns a copy of the initial ability bar
		public AbilityActionBar GetAbilityBar ()
		{
			return initActionBar.DeepCopy();
		}
	}
}
