using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EntityAction = System.Func<UnityEngine.GameObject, UnityEngine.GameObject, System.Collections.IEnumerator>;

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
	public static ComboDatabase comboDatabase;
	public static EntityDatabase entityDatabase;
	public static HashSet<Commander> commanders;
	
	/* File locations within. */
	string abilityXMLFile = "Assets/Resources/Abilities.txt";
	string comboXMLFile = "Assets/Resources/Combos.txt";
	string entityXMLFile = "Assets/Resources/Entities.txt";
	
	/* Necessary to avoid multiple instantiations. */
	static bool initializedNonMonos = false;
	static bool updatedNonMonos = false;

	/* Instantiates non-MonoBehaviours and utilities. */
	void Awake ()
	{
		if (!initializedNonMonos)
		{
			if (showDebug) Debug.Log("_____MyMonoBehaviour: Performing first-time initialization_____");
			
			//Set this immediately so that subsequent initializations of player and minions don't make a loop
			initializedNonMonos = true;
			instance = this;
			
			InstantiateDatabases();
			InstantiateLevel();
			
			InstantiatePlayer();
			InstantiateCommander();
			
			if (showDebug) Debug.Log("_____MyMonoBehaviour: First-time initialization complete_____");
			Debug.Log ("");
		}
	}
	
	/* Updates all non-MonoBehaviours. */
	void Update ()
	{
		if (!updatedNonMonos)
		{
			foreach (Commander commander in commanders)
			{
				commander.Update();
			}
			
			//levelBuilder.debugVisualize();
			updatedNonMonos = true;
		}
	}
	
	//After all updates have run reset update status on commanders
	void LateUpdate ()
	{
		updatedNonMonos = false;
	}
	
	/* After this we can instatntiate abilities and combos.*/
	void InstantiateDatabases ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating databases...");
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating abilities...");
		abilityDatabase = Utilities.XMLParser.ParseAbilities(abilityXMLFile);
		if (showDebug) Debug.Log(string.Format("MyMonoBehaviour: Finished instantiating {0} abilities, of them {1} universal!", abilityDatabase.Count(), abilityDatabase.universalAbilities.Count));
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating combos...");
		comboDatabase = Utilities.XMLParser.ParseCombos(comboXMLFile, abilityDatabase);
		if (showDebug) Debug.Log(string.Format("MyMonoBehaviour: Finished instantiating {0} combos!", comboDatabase.Count()));
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating entities...");
		entityDatabase = Utilities.XMLParser.ParseEntities(entityXMLFile, abilityDatabase);
		if (showDebug) Debug.Log(string.Format("MyMonoBehaviour: Finished instantiating {0} entities!", entityDatabase.Count()));
		
		memoizer = new Memoizer();
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating databases!");
	}
	
	/* After this the commander is ready to command its units. */
	void InstantiateCommander ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating commander...");
		
		commanders = new HashSet<Commander>();
		commanders.Add(new Commander(abilityDatabase, comboDatabase));
		levelBuilder.AssignMinions(commanders);
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating commander!");
	}

	/* After this the payerAbilityActionBar is ready to be given to the player. */
	AbilityActionBar InstantiatePlayerAbilityActionBar ()
	{
		if (showDebug) Debug.Log("MyMonoBehaviour: Instantiating player action bar...");
		
		AbilityActionBar playerAbilityActionBar = new AbilityActionBar(player);
		MovementActionBar playerMovementActionBar = new MovementActionBar(player);
		
		List<Ability> startingAbilities1 = new List<Ability>();
		startingAbilities1.Add(abilityDatabase.GetAbility("Dash"));
		playerAbilityActionBar.AddAbilityBox(startingAbilities1);
		
		List<Ability> startingAbilities2 = new List<Ability>();
		startingAbilities2.Add(abilityDatabase.GetAbility("Dummy Ability"));
		startingAbilities2.Add(abilityDatabase.GetAbility("Dummy Ability"));
		playerAbilityActionBar.AddAbilityBox(startingAbilities2);
		
		List<Ability> startingAbilities3 = new List<Ability>();
		startingAbilities3.Add(abilityDatabase.GetAbility("Dummy Ability"));
		//startingAbilities3.Add(abilityDatabase.GetAbility("Dash"));
		playerAbilityActionBar.AddAbilityBox(startingAbilities3);
		
		List<Ability> startingAbilities4 = new List<Ability>();
		startingAbilities4.Add(abilityDatabase.GetAbility("Dummy Ability"));
		//startingAbilities4.Add(abilityDatabase.GetAbility("Dummy Ability"));
		//startingAbilities4.Add(abilityDatabase.GetAbility("Dash"));
		//startingAbilities4.Add(abilityDatabase.GetAbility("Dash"));
		playerAbilityActionBar.AddAbilityBox(startingAbilities4);
		
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
		
		//Player2 has MovementCC2 which will break everything
		player = (GameObject)GameObject.Instantiate(Resources.Load("Player2"), new Vector3(1, 1, 1), Quaternion.identity);
		
		if (player == null)
		{
			throw new System.ArgumentException("Could not locate Player prefab!");
		}
		else
		{
			playerDescriptor = player.GetComponent<EntityDescriptor>();
			
			if (playerDescriptor == null) Debug.Log("oh oh player has no descriptor");
			
			playerDescriptor.abilityBar = InstantiatePlayerAbilityActionBar();
			//player.transform.position = levelBuilder.playerStart;
		}
		
		if (showDebug) Debug.Log("MyMonoBehaviour: Finished instantiating player!");
	}	
}
