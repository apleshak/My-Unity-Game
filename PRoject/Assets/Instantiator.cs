using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/* 
   Serves as the access point for all nonMono scripts and their instantiator. May be found by 
   instance variable instead of gameobject its attached to.
*/
public class Instantiator : MonoBehaviour
{
	public Memoizer memoizer;
	public LevelBuilder levelBuilder;
	public AbilityDatabase abilityDatabase;
	public Commander commander;
	public static Instantiator instance;
	public AbilityActionBar playerAbilityActionBar;
	public MovementActionBar playerMovementActionBar;
	public GameObject player;
	
	/* Runs once. Instantiates everything. Everything. */
	void Awake ()
	{
		instance = this;
		instantiatePlayer();
		instantiateDatabases();
		instantiateAbilities();
		instantiateLevel();
	}
	
	/* Updates all the nonMono objects as well. */
	void Update ()
	{
		commander.Update();
		levelBuilder.debugVisualize();
	}
	
	void instantiateDatabases ()
	{
		abilityDatabase = new AbilityDatabase();
		memoizer = new Memoizer();
		commander = new Commander(abilityDatabase);
	}
	
	void instantiateAbilities ()
	{
		//Dash____________________________________________________________________________
		Type dashType = typeof(Dash);
		Texture2D dashIcon = (Texture2D)Resources.Load("Abilities/Dash/Icon");
		HashSet<string> dashTags = new HashSet<string>();
		dashTags.Add("Movement");
		dashTags.Add("Quick");
		dashTags.Add ("Facing");
		string dashDescription = "Quickly move a short distance forward.";
		
		Ability dash = new Ability(dashType, dashIcon, "Dash", dashDescription, dashTags);
		abilityDatabase.addAbility(dash);
		
		//Sticky Slime____________________________________________________________________
		Type stickySlimeType = typeof(StickySlime);
		Texture2D stickySlimeIcon = (Texture2D)Resources.Load("Abilities/Sticky Slime/Icon");
		HashSet<string> stickySlimeTags = new HashSet<string>();
		stickySlimeTags.Add("Movement Impairing");
		stickySlimeTags.Add("Projectile");
		stickySlimeTags.Add ("Facing");
		string stickySlimeDescription = "Vomit sticky slime that hinders movement. Trips any who attempt to dash through.";
		
		Ability stickySlime = new Ability(stickySlimeType, stickySlimeIcon, "Sticky Slime", 
										  stickySlimeDescription, stickySlimeTags, 6.0f, 90.0f);
		abilityDatabase.addAbility(stickySlime);
	}
	
	void instantiateLevel ()
	{
		levelBuilder = new LevelBuilder(25, 2);
	}
	
	/* Requires a "Player" prefab. */
	void instantiatePlayer ()
	{
		player = (GameObject)GameObject.Instantiate(Resources.Load("Player"));
		
		if (player == null)
		{
			return;
		}
		
		playerAbilityActionBar = new AbilityActionBar(player);
		return;
		playerMovementActionBar = new MovementActionBar(player);
	}
}