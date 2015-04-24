using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//###################
//gameplay idea box #
//###################
//Goals:
// non-trivial
// non-linear
// witty

//Key features:
// Evolutionary AI
// Random City/Level Generation
// Reactionary+Strategic Combat
// Synergetic Skills/Items
// Story-based Item/Skill Acquisition
// Single-goal Levels
// Vision Cone Lighting
// Smart Experience Scheduler

//Evolutionary AI
// Based on Markov Chains. 
// Aids AI in predicting player action. 
// Uses skill tags to identify optimal response.
// 
// E.g.:
//	Roll | Movement; Instant; Front-facing
//		"Roll X meters in the direction you are facing. Knocks you down for Y seconds if performed with poor footing. Destroys
//		lanterns."
//
//	Sticky Goo | AOE; Instant; Front-facing; Movement-impairment
//		"Spreads sticky goo in a X meter radius that slows movement. If a target rolls while inside the area of effect it will
//		be knocked down and immobilized for Y seconds."

//Random City/Level Generation
// Voronoi/Laspeires/Flocking/Maze methods
// Separate Instance Generators

//Reactionary+Strategic Combat
// Low Global Cooldowns
// Positionining
// Quick Pattern Inference

//Synergetic Skills/Items
// Items Modify Skills
// Drastic Changes

//Story-based Item/Skill Acquisition
// Ex Ante, In Medias Res and Ex Post acquisition scenarios
// Short, Long and Permanent Experiences

//Single-goal Levels
// Find a gate
// All other entrances may or may not lead to a gate

//Vision Cone Lighting
// Follows mouse
// All light sources behave similarly

//Smart Experience Scheduler
// Doesn't allow experiences to accumulate
// Maintains player focus
// Evaluates when to introduce experiences and how long they should be
//___________________________________________________________________________________________________________________________

//#################
//progress report #
//#################

//Evolutionary AI
// Done:
//  Markov chains
//  Oracle
//  ActionBar
//  Abilities
//  Commander
//  Ability matcher
//  Status
//  Effects

// Remaining:
//  AI FSM that can take orders from commander
//  Senses incorporation

//___________________________________________________________________________________________________________________________
//#####################
//early ideas (scrap) #
//#####################

//desanguination = blood loss
//mithridatism - immunity to poison
//Status manipulator class - alchemist
//Minions class - necromancer/mechanic
//Combat class - warrior
//Investigator class - rogue
// Can traverse roofs - needs to listen to creaking to not fall into house
//

//Come home to your town but everyone is gone - Croaton. You can unravel the mystery too.
//CAn clang with crowbar to get ppl to turn their lights on - this fights certain mobs
//its all like a dream

//player hp is the hue of some item
//By default enemy hp is invisible
//
//invisibility blanket - removes almost all vision - can light flashlight inside to reveal a small area around you
//old man that only appears on corners of peripheral vision cone
//man gives you watch that doesnt tick but works and it draws a certain blind monster to you


//clips on to an empty object
//instantiates and handles all objects related to ability execution and report gathering
//inherits from the single controlls class (to hold keystroke data) that is static and needs no references to it
//update looks for key presses and executes abilities
//if ability reports a successful execution the chain moves up one and the report is updated
//if ability reports a failure the chain rolls back to the beginning and nothing is recorded
/* probabilities in reports are recoded and reports are hashed based on the ability set in use -
this way we only consider ability chains that could be constructed from the present abilities */
/* chain is built as normal but without caring which ability slot it came from - thus the analysis is held 
in the ability bar class */
//will need a dictionary of as many keys as there are subsets of ability sets
//4 abilities at a tme, 35 total abilities = (35 choose 4) sets = 52360 sets (a short holds 65536)
//must map dict keys (ints) to sets...use a tree? At most 4 comparisons 
//tree likely holds 128 megabytes - can select which of these to load at a time
//dont need to hold probs, just map last used ability id to priority queue of next abil
//if multiple abils are close in priority then choose randomly

//QUESTONS?
//what is forr
//HIDDEN MARKOV CHAINS
//RED BLACK TREES - what they do

//___________________________________________________________________________________________________________________________

//#####################
//AI discourse - good #
//#####################

//GRAPH FOR FMS
/* regular directed graph with nodes as states and edges implied through state contained exit/enter
 routines */
/* the starting structure is immutable in the sence that all nodes in it and their edges cannot be removed
 thus each node has a bool field indicating whether the node is immutable - this also prevents edge removal between
 immutable nodes *make sure this only holds for immu-immu edges* */
//e.g. add state to death self-loop to despawn body and delete yourself from eneies db
//e.g add state to death self-loop to revive after set time with higher stats
//e.g. add state to attack state to stop attack half way if player is in attack animation and jump directly away
//e.g. add state to combat state to jump towards back-turned player before moving to attempt-attack phase
//e.g. add run state to combat state that leaves its self loop only upon some condition
//IMPORTANT to keep all edge/transition data on states themselves so that removal does not break logic

//new states are constructed based on rough fast natural selection relying on a heuristic
//tags for abilities work as trees
//node 1 : combat,movement,idle,death
//e.g. combat->close_quarters->fast->with_debuff->

//states have tolerances
//state with harsh negative consequences if it's not executed is unlikely to be dropped
//state with minor consequences is likely to be forgotten/skipped
/*harshness and minority are determined by the state of the system entities reside in as well as
the properties they have built in */
//in our case global params are just the player analysis since its the only enemy

//IMPORTANT try to instead build probability chart based on how player responds to tags of certain kind
//then use the least efficient enemy action vs player reaction sequence as a point of improvement
//enemies can improve certain types of abilities better than others
//This is how we build a state

/* player activates ability
   if ability succeeds it is added to the report
   if ability fails the abiity chain the player used is reset

  weak form: 
  	designate a target state that you know counters the ability a player is most likely to use next
	the dynamic portion of the report should tell you what ability the player can still use

  strong form:
  	insert new states dynamically such that their probability of being executed in time is proportionate to how
	damaging it is to fail to reach them in time.
		weak form:
			only inserts states at a hub node
		strong form:
			inserts states to any node and handles execution order via priority queue

*/


/* suppose we know that we need to insert a new state that counters a player's ability plab */


//___________________________________________________________________________________________________________________________
