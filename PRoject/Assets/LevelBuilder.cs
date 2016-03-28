using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using G = Graph<Structure>;
using V = Graph<Structure>.Vertex;
//Position and forward vector
using Orientation = Tuple<UnityEngine.Vector3, UnityEngine.Vector3>;

public class LevelBuilder
{	
	//TODO: add rotations beyond 90 degrees and figure out how to fill the resulting kinks
	public static float[] rotations = 
		{
			0.0f, 22.5f, 45.0f, 67.5f, 
			90.0f, -22.5f, -45.0f, -67.5f, 
			-90.0f
		};
	public Vector3 levelStart;
	public Vector3 playerStart;
	public int nodes;
	public int minEdges;
	public int maxEdges;
	int nextNodeID = 0;
	BuildingSpawner buildingSpawner;
	Dictionary<V, Orientation> orientationMap;
	
	public LevelBuilder (int Nodes, int MaxEdges, Vector3 levelStartPos)
	{
		nodes = Nodes;
		maxEdges = MaxEdges;
		levelStart = levelStartPos;
		buildingSpawner = new BuildingSpawner();
		orientationMap = new Dictionary<V, Orientation>();
		List<G> areas = GenerateLevel(3, nodes, MaxEdges, 1, 1);
		Dispatch(areas[0].firstVertex);
		DebugVisualize(areas[0].firstVertex);
	}
	
	//Give each commander 2 minions for now
	public void AssignMinions (HashSet<Commander> commanders)
	{
		Object minionPrefab = MyMonoBehaviour.memoizer.GetMemoizedPrefab("Prefabs/Entities/Minion");
		
		foreach (Commander commander in commanders)
		{
			AssignMinion(commander);
			AssignMinion(commander);
		}
	}
	
	//Creates minion from prefab with locked movement (so that player input never moves it)
	void AssignMinion (Commander commander)
	{
		Object minionPrefab = MyMonoBehaviour.memoizer.GetMemoizedPrefab("Prefabs/Entities/Minion");
		GameObject minion = (GameObject)GameObject.Instantiate<Object>(minionPrefab);
		
		EntityDescriptor minionDescr = MyMonoBehaviour.memoizer.GetMemoizedComponent<EntityDescriptor>(minion);
		AbilityActionBar minionAbilityBar = MyMonoBehaviour.entityDatabase.GetEntityActionBar("Minion");
		minionAbilityBar.SetOwner(minion);
		minionDescr.abilityBar = minionAbilityBar;
		
		MovementCC movement = MyMonoBehaviour.memoizer.GetMemoizedComponent<MovementCC>(minion);
		movement.lockInput();
		
		minionDescr.abilityBar = minionAbilityBar;
		commander.AddMinion(new Tuple<AbilityActionBar, GameObject>(minionAbilityBar, minion));
	}
	
	//Generate a few areas, connect them with bridges.
	//TODO use bridges parameters instead of linking sequentially
	List<G> GenerateLevel (int numAreas, int maxStructuresPerArea, int maxStructureConnections, int maxBridgesPerArea, int maxBridgeLength)
	{
		//Generate the areas as DAGs.
		List<G> areas = new List<G>();
		Dictionary<G, Tuple<V, V>> areaCentralityMap = new Dictionary<G, Tuple<V, V>>();
		
		//Populate the areas list and the centrality map
		for (int i = 0; i < numAreas; i++)
		{
			G newArea = G.Map<int,Structure>(GenerateGraph(maxStructuresPerArea, maxStructureConnections));
			areas.Add(newArea);
			areaCentralityMap[newArea] = newArea.MinMaxDegree();
		}
		
		//Connect the areas by linking the most central nodes of one area to the least central ones from another area.
		//The resulting graph is still a DAG since we don't link the first and last area of areas.
		for (int i = 0; i < numAreas - 1; i++)
		{
			Tuple<V, V> minMaxA = areaCentralityMap[areas[i]];
			Tuple<V, V> minMaxB = areaCentralityMap[areas[i+1]];
			
			areas[i].AddEdge(minMaxA.second, minMaxB.first);
			areas[i+i].AddEdge(minMaxA.second, minMaxB.first);
		}
		
		return areas;
	}
	
	//Creates a graph with N nodes, where any node can have at most maxEdges edges
	G GenerateGraph (int N, int maxEdges)
	{
		nextNodeID += 1;
		G graph = new G();
		V startVertex = graph.AddVertex(nextNodeID);
		GrowGraph(graph, N, nextNodeID, maxEdges, startVertex);
		return graph;
	}
	
	// Depth-first, recursive graph creation starting from source. Returns last vertex spawned and the number of 
	//vertecies spawned.
	Tuple<V, int> GrowGraph (G graph, int Nodes, int nextID, int MaxEdges, V source)
	{
		V newEdgeNode = null;
		int spawnedNodes = 0;
		
		//i only increases if we spawn nodes
		for (int i = 0; i < Nodes;)
		{
			nextID = NextNodeID(nextID);
			int edgesToAdd = Random.Range(0, Mathf.Min(MaxEdges, Nodes - i));
			i += edgesToAdd;
			
			for (; edgesToAdd > 0; edgesToAdd--)
			{
				// Decide the next ID, vertex and edge. Add it, then recurse on it.
				nextID = NextNodeID(nextID);
				newEdgeNode = graph.AddVertex(nextID);
				graph.AddEdge(source, newEdgeNode);
				
				if (edgesToAdd != 1)
					i += GrowGraph(graph, (Nodes-i)/edgesToAdd, nextID, MaxEdges, newEdgeNode).second;
			}
			
			/* Start at last vertex spawned (and not recursed upon). */
			if (newEdgeNode != null)
				source = newEdgeNode;
			
			spawnedNodes = i;
		}
		
		return new Tuple<V, int>(source, spawnedNodes);
	}
	
	int NextNodeID (int lastID)
	{
		nextNodeID += 1;
		return nextNodeID;
	}
	
	bool Dispatch (V root)
	{
		foreach (V target in root.Neighbors())
		{
			if (!OrientateStructures(root, target))
				return false;
		}
		
		return true;
	}
	
	//Depth-first backtracking routine to decide the position and rotation of structures.
	//
	//Orientation happens via:
	//	1) Rotating the structures to align with connection points on other structures
	//	2) Rotating structures around connection points to avoid collisions
	bool OrientateStructures (V root, V target)
	{
		Structure rStruct = root.data;
		Structure tStruct = target.data;
		GameObject colliderObj = GameObject.Instantiate(MyMonoBehaviour.memoizer.GetMemoizedPrefab("Structures/Templates/" + tStruct.colliderName));
		
		//For every connector in root try to attach the structure
		foreach (Orientation connector in rStruct.attachPoints)
		{
			//iterate through every possible connector alignment between connector and structure
			foreach (Orientation alignment in tStruct.GetAlignments(rStruct.orientation))
			{
				//apply the alignment to the structure
				tStruct.orientation.first = alignment.first;
				tStruct.orientation.second = alignment.second;
				colliderObj.transform.position = alignment.first;
				colliderObj.transform.rotation = alignment.second;
				
				//rotate the aligned structure through every possible rotation
				foreach (float rotation in rotations)
				{
					//need to simulate g.transform.RotateAround(point, axis, angle) on structVert.data
					colliderObj.transform.RotateAround (connector.first, Vector3.up, rotation);
					tStruct.orientation.second = colliderObj.transform.rotation.eulerAngles;
					MeshCollider collider = MyMonoBehaviour.memoizer.GetMemoizedComponent<MeshCollider>(colliderObj);
					
					if (!HasGlobalCollision(collider) && Dispatch(target))
						return true;
				}
			}
		}
		
		GameObject.DestroyImmediate(colliderObj);
		return false;
	}
	
	bool HasGlobalCollision (MeshCollider C)
	{
		foreach (GameObject o in GameObject.FindGameObjectsWithTag("collider"))
		{
			if (C.bounds.Intersects(MyMonoBehaviour.memoizer.GetMemoizedComponent<MeshCollider>(o).bounds))
				return true;
		}
		
		return false;
	}
	
	public void DebugVisualize (V root)
	{
		DebugVisualize(root, levelStart);
	}
	
	void DebugVisualize (V root, Vector3 rootPos)
	{
		V nextNode = root;
		
		if (nextNode != null)
		{
			List<V> frontier = nextNode.Neighbors();
			
			foreach (V v in frontier)
			{
				Debug.DrawRay(rootPos, v.data.orientation.first - rootPos);
				DebugVisualize(v, v.data.orientation.first);
			}
		}
	}
	
	public void Visualize (V root)
	{
		V nextNode = root;
		GameObject lastBuilding = null;
		
		while (nextNode != null)
		{
			GameObject obj = buildingSpawner.GetRandBuildingExcept(nextNode.Neighbors.Count, lastBuilding);
			lastBuilding = obj;
			GameObject.Instantiate(obj, nextNode.data.orientation.first, Quaternion.AngleAxis(nextNode.data.orientation.second), Vector3.up);
		}
	}
	
	/* Loads all resources in "Prefabs/Buildings". */
	private class BuildingSpawner
	{
		/* Maps edges to lists of objects with that many edges. */
		public int maxConnectors;
		public Dictionary<int, List<GameObject>> dict;
		
		public BuildingSpawner ()
		{
			dict = new Dictionary<int, List<GameObject>>();
			int connectors;
			
			foreach (Object obj in Resources.LoadAll("Prefabs/Buildings"))
			{
				GameObject inst = (GameObject)GameObject.Instantiate(obj);
				CityObjectDescriptor descr = (CityObjectDescriptor)inst.GetComponent<CityObjectDescriptor>();
				connectors = descr.connections.Count;
				
				/* Also update the max number of connectors found so far. */
				if (connectors > maxConnectors)
				{
					maxConnectors = connectors;
				}
				
				dict[connectors].Add(inst);
			}
		}
		/* Returns building with at least edges number of connectors. */
		public GameObject GetRandBuilding (int edges)
		{
			int surplusEdges = Random.Range(edges, maxConnectors);
			List<GameObject> list = dict[surplusEdges];
			return dict[surplusEdges][Random.Range(0, list.Count)];
		}
		
		/* Could run forever if except is the only building with that many edges. */
		public GameObject GetRandBuildingExcept (int edges, GameObject except)
		{
			GameObject building = GetRandBuilding(edges);
			
			/* Reference comparison is fine. */
			while (building == except)
			{
				building = GetRandBuilding(edges);
			}
			
			return building;
		}
	}
}