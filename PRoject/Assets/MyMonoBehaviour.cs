using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyMonoBehaviour : MonoBehaviour 
{
	/* Utilities - only safe to access after Awake finishes initializing non-MonoBehavious. */
	public static bool showDebug = true;
	public static MyMonoBehaviour instance;
	public static GameObject player;
	public static EntityDescriptor playerDescriptor;
	
	/* Non-MonoBehaviours - stored here. */
	public static Memoizer memoizer;
	public static LevelBuilder levelBuilder;
	public static AbilityDatabase abilityDatabase;
	public static Commander commander;
	
	/* Necessary to avoid multiple instantiations. */
	static bool initializedNonMonos = false;
	
	/* Instantiates non-MonoBehaviours and utilities. */
	void Awake ()
	{
		if (!initializedNonMonos)
		{
			if (showDebug) Debug.Log("_____MyMonoBehaviour: Performing first-time initialization_____");
			
			/* Utilities */
			instance = this;
			
			InstantiateDatabases();
			InstantiateAbilities();
			InstantiateLevel();
			
			InstantiatePlayer();
			InstantiateCommander();
			
			initializedNonMonos = true;
			
			if (showDebug) Debug.Log("_____MyMonoBehaviour: First-time initialization complete_____");
		}
	}
	
	/* Updates all non-MonoBehaviours. */
	void Update ()
	{
		commander.Update();
		levelBuilder.debugVisualize();
	}
	
	/* After this we can instatntiate abilities.*/
	void InstantiateDatabases ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating databases...");
		
		abilityDatabase = new AbilityDatabase();
		memoizer = new Memoizer();
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating databases!");
	}
	
	/* After this the commander is ready to command its units. */
	void InstantiateCommander ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating commander...");
		
		commander = new Commander(abilityDatabase);
		commander.PopulateLevel(levelBuilder);
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating commander!");
	}
	
	
	/* After this the abilityDatabase can be used to make abilities. */
	/* TODO: optimize by opening from one file descriptor. */
	void InstantiateAbilities ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating abilities...");
		
		//Grab____________________________________________________________________________
		Type grabType = typeof(Grab);
		Sprite grabIcon = Resources.Load<Sprite>("Abilities/Grab/Icon");
		HashSet<string> grabTags = new HashSet<string>();
		grabTags.Add("Preperatory");
		grabTags.Add("Close Combat");
		grabTags.Add ("Facing");
		grabTags.Add ("Quick");
		string grabDescription = "Pick up the first thing underfoot.";
		
		Ability grab = new Ability(grabType, grabIcon, "Grab", grabDescription, grabTags);
		abilityDatabase.addAbility(grab);
		
		//Throw____________________________________________________________________________
		Type throwType = typeof(ThrowProjectile<Throw>);
		Sprite throwIcon = Resources.Load<Sprite>("Abilities/Throw/Icon");
		HashSet<string> throwTags = new HashSet<string>();
		throwTags.Add("Offensive");
		throwTags.Add("Ranged");
		throwTags.Add ("Facing");
		string throwDescription = "Throw whatever you have in your hand.";
		
		Ability throwAbility = new Ability(throwType, throwIcon, "Throw", throwDescription, throwTags);
		abilityDatabase.addAbility(throwAbility);
		
		//One____________________________________________________________________________
		Type oneType = typeof(One);
		Sprite oneIcon = Resources.Load<Sprite>("Abilities/One/Icon");
		HashSet<string> oneTags = new HashSet<string>();
		oneTags.Add("Offensive");
		oneTags.Add("Close Combat");
		oneTags.Add ("Facing");
		oneTags.Add ("Quick");
		string oneDescription = "Deliver a hard left jab.";
		
		Ability one = new Ability(oneType, oneIcon, "One", oneDescription, oneTags);
		abilityDatabase.addAbility(one);
		
		//Two____________________________________________________________________________
		Type twoType = typeof(Two);
		Sprite twoIcon = Resources.Load<Sprite>("Abilities/Two/Icon");
		HashSet<string> twoTags = new HashSet<string>();
		twoTags.Add("Offensive");
		twoTags.Add("Close Combat");
		twoTags.Add ("Facing");
		twoTags.Add ("Slow");
		string twoDescription = "Deliver a hard right straight.";
		
		Ability two = new Ability(twoType, twoIcon, "Two", twoDescription, twoTags);
		abilityDatabase.addAbility(two);
		
		//Squat____________________________________________________________________________
		Type squatType = typeof(Squat);
		Sprite squatIcon = Resources.Load<Sprite>("Abilities/Squat/Icon");
		HashSet<string> squatTags = new HashSet<string>();
		squatTags.Add("Preperatory");
		squatTags.Add("Movement");
		squatTags.Add ("Facing");
		string squatDescription = "Quickly squat down to the ground.";
		
		Ability squat = new Ability(squatType, squatIcon, "Squat", squatDescription, squatTags);
		abilityDatabase.addAbility(squat);
		
		//Dash____________________________________________________________________________
		Type dashType = typeof(Dash);
		Sprite dashIcon = Resources.Load<Sprite>("Abilities/Dash/Icon");
		HashSet<string> dashTags = new HashSet<string>();
		dashTags.Add("Movement");
		dashTags.Add("Quick");
		dashTags.Add("Long");
		dashTags.Add ("Facing");
		string dashDescription = "Quickly move a short distance forward.";
		
		Ability dash = new Ability(dashType, dashIcon, "Dash", dashDescription, dashTags);
		abilityDatabase.addAbility(dash);
		
		//Roll____________________________________________________________________________
		Type rollType = typeof(Roll);
		Sprite rollIcon = Resources.Load<Sprite>("Abilities/Roll/Icon");
		HashSet<string> rollTags = new HashSet<string>();
		rollTags.Add("Movement");
		rollTags.Add("Quick");
		rollTags.Add("Short");
		rollTags.Add ("Facing");
		string rollDescription = "Roll a short distance forward.";
		
		Ability roll = new Ability(rollType, rollIcon, "Roll", rollDescription, rollTags);
		abilityDatabase.addAbility(roll);
		
		//Sticky Slime____________________________________________________________________
		Type stickySlimeType = typeof(StickySlime);
		Sprite stickySlimeIcon = Resources.Load<Sprite>("Abilities/Sticky Slime/Icon");
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
		Sprite dummyAbilityIcon = Resources.Load<Sprite>("Abilities/Dummy Ability/Icon");
		HashSet<string> dummyAbilityTags = new HashSet<string>();
		stickySlimeTags.Add("BS");
		string dummyAbilityDescription = "bs boyz";
		
		Ability dummyAbility = new Ability(dummyAbilityType, dummyAbilityIcon, "Dummy Ability", 
		                                   dummyAbilityDescription, dummyAbilityTags, 6.0f, 90.0f);
		abilityDatabase.addAbility(dummyAbility);
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating abilities!");
	}
	
	/* After this the payerAbilityActionBar is ready to be given to the player. */
	AbilityActionBar InstantiatePlayerAbilityActionBar ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating player action bar...");
		
		AbilityActionBar playerAbilityActionBar = new AbilityActionBar(player);
		MovementActionBar playerMovementActionBar = new MovementActionBar(player);
		
		List<Ability> startingAbilities1 = new List<Ability>();
		startingAbilities1.Add(abilityDatabase.GetAbility("Dash"));
		startingAbilities1.Add(abilityDatabase.GetAbility("Dummy Ability"));
		playerAbilityActionBar.addAbilityBox(startingAbilities1);
		
		List<Ability> startingAbilities2 = new List<Ability>();
		startingAbilities2.Add(abilityDatabase.GetAbility("Dummy Ability"));
		startingAbilities2.Add(abilityDatabase.GetAbility("Dummy Ability"));
		playerAbilityActionBar.addAbilityBox(startingAbilities2);
		
		List<Ability> startingAbilities3 = new List<Ability>();
		startingAbilities3.Add(abilityDatabase.GetAbility("Dash"));
		playerAbilityActionBar.addAbilityBox(startingAbilities3);
		
		List<Ability> startingAbilities4 = new List<Ability>();
		startingAbilities4.Add(abilityDatabase.GetAbility("Dummy Ability"));
		startingAbilities4.Add(abilityDatabase.GetAbility("Dummy Ability"));
		startingAbilities4.Add(abilityDatabase.GetAbility("Dash"));
		startingAbilities4.Add(abilityDatabase.GetAbility("Dash"));
		playerAbilityActionBar.addAbilityBox(startingAbilities4);
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating player action bar!");
		return playerAbilityActionBar;
	}
	
	/* After this the level is ready to play. Here Commander also sets up the minions. */
	void InstantiateLevel ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating level...");
		
		levelBuilder = new LevelBuilder(25, 2);
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating level!");
	}
	
	/* After this the player is ready to face the world. Requires a "Player" prefab. */
	/* The prefab has everything except a*/
	void InstantiatePlayer ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating player...");
		
		player = (GameObject)GameObject.Instantiate(Resources.Load("Player2"));
		
		if (player == null)
		{
			throw new System.ArgumentException("Could not locate Player prefab!");
		}
		else
		{
			playerDescriptor = player.GetComponent<EntityDescriptor>();
			if (playerDescriptor == null) Debug.Log("oh oh");
			playerDescriptor.abilityBar = InstantiatePlayerAbilityActionBar();
			player.transform.position = levelBuilder.levelStart;
		}
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating player!");
	}
}
