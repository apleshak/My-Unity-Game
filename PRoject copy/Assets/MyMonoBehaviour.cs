using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyMonoBehaviour : MonoBehaviour 
{
	/* Utilities - only safe to access after Awake. */
	public static MyMonoBehaviour instance;
	public static GameObject player;
	
	/* Non-MonoBehaviours - stored here. */
	public static Memoizer memoizer;
	public static LevelBuilder levelBuilder;
	public static AbilityDatabase abilityDatabase;
	public static Commander commander;
	public static AbilityActionBar playerAbilityActionBar;
	public static MovementActionBar playerMovementActionBar;
	
	/* Necessary to avoid multiple instantiations. */
	static bool initializedNonMonos = false;
	
	/* Instantiates non-MonoBehaviours and utilities. */
	void Awake ()
	{
		if (!initializedNonMonos)
		{
			instance = this;
			
			InstantiatePlayer();
			InstantiateDatabases();
			InstantiateAbilities();
			InstantiateAbilityActionBar();
			InstantiateLevel();
			
			initializedNonMonos = true;
		}
	}
	
	/* Updates all non-MonoBehaviours. */
	void Update ()
	{
		commander.Update();
		levelBuilder.debugVisualize();
	}
	
	void InstantiateDatabases ()
	{
		abilityDatabase = new AbilityDatabase();
		memoizer = new Memoizer();
		commander = new Commander(abilityDatabase);
	}
	
	void InstantiateAbilities ()
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
		
		//Dummy Ability____________________________________________________________________
		Type dummyAbilityType = typeof(DummyAbility);
		Texture2D dummyAbilityIcon = (Texture2D)Resources.Load("Abilities/Dummy Ability/Icon");
		HashSet<string> dummyAbilityTags = new HashSet<string>();
		stickySlimeTags.Add("BS");
		string dummyAbilityDescription = "bs boyz";
		
		Ability dummyAbility = new Ability(dummyAbilityType, dummyAbilityIcon, "Dummy Ability", 
		                                   dummyAbilityDescription, dummyAbilityTags, 6.0f, 90.0f);
		abilityDatabase.addAbility(dummyAbility);
	}
	
	void InstantiateAbilityActionBar ()
	{
		playerAbilityActionBar = new AbilityActionBar(player);
		playerMovementActionBar = new MovementActionBar(player);
		
		List<Ability> startingAbilities1 = new List<Ability>();
		startingAbilities1.Add(abilityDatabase.getAbility("Dummy Ability"));
		startingAbilities1.Add(abilityDatabase.getAbility("Dummy Ability"));
		playerAbilityActionBar.addAbilityBox(startingAbilities1);
		
		List<Ability> startingAbilities2 = new List<Ability>();
		startingAbilities2.Add(abilityDatabase.getAbility("Dummy Ability"));
		startingAbilities2.Add(abilityDatabase.getAbility("Dummy Ability"));
		playerAbilityActionBar.addAbilityBox(startingAbilities2);
		
		List<Ability> startingAbilities3 = new List<Ability>();
		startingAbilities3.Add(abilityDatabase.getAbility("Dash"));
		playerAbilityActionBar.addAbilityBox(startingAbilities3);
		
		List<Ability> startingAbilities4 = new List<Ability>();
		startingAbilities4.Add(abilityDatabase.getAbility("Dummy Ability"));
		startingAbilities4.Add(abilityDatabase.getAbility("Dummy Ability"));
		startingAbilities4.Add(abilityDatabase.getAbility("Dash"));
		startingAbilities4.Add(abilityDatabase.getAbility("Dash"));
		playerAbilityActionBar.addAbilityBox(startingAbilities4);
	}
	
	void InstantiateLevel ()
	{
		levelBuilder = new LevelBuilder(25, 2);
	}
	
	/* Requires a "Player" prefab. */
	void InstantiatePlayer ()
	{
		player = (GameObject)GameObject.Instantiate(Resources.Load("Player"));
		
		if (player == null)
		{
			//TODO find a way tp throw this correctly
			//throw System.ArgumentException;
		}
	}
}
