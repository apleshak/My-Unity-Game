using UnityEngine;
using System.Collections.Generic;

public class Dummy
{
	int edges;
	
	public Dummy (int Edges)
	{
		edges = Edges;
	}
}

public class LevelBuilder
{	
	public static float[] dirs = 
		{
			0.0f, 22.5f, 45.0f, 67.5f, 
			90.0f, 112.5f, 135.0f, 157.5f, 
			180.0f, 202.5f, 225.0f, 247.5f,
			270.0f, 292.5f, 315.0f, 337.5f,
		};
	public Vector3 levelStart;
	public Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f); 
	public int nodes;
	public int maxEdges;
	public Graph<Dummy, Tuple<Vector3, Vector3>> graph;
	public Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex start;
	BuildingSpawner buildingSpawner;
	
	public LevelBuilder (int Nodes, int MaxEdges)
	{
		nodes = Nodes;
		maxEdges = MaxEdges;
		buildingSpawner = new BuildingSpawner();
		generateGraph(nodes, MaxEdges);
		configureDirections(start, origin);
	}
	
	void generateGraph (int Nodes, int MaxEdges)
	{
		graph = new Graph<Dummy, Tuple<Vector3, Vector3>>();
		Dummy nextID = new Dummy(0);
		start = graph.addVertex(nextID);
		growGraph(nodes, nextID, maxEdges, start);
	}
	
	/* Depth-first, recursive graph creation. Nodes is the length of the main path. */
	Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex growGraph (int Nodes, Dummy nextID, int MaxEdges,
												Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex source)
	{
		Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex newEdgeNode = null;

		for (int i = 0; i < Nodes; i++)
		{
			nextID = nextNodeID(nextID);
			
			//TODO
			/* Edges should be a function of the graph topology with request memory.*/
			/* Always runs at least once. */
			int edges = Random.Range(0, Mathf.Min(MaxEdges, Nodes - i));
			i += edges;
			for (; edges > 0; edges--)
			{
				/* Decide the next ID, vertex and edge. */
				nextID = nextNodeID(nextID);
				newEdgeNode = graph.addVertex(nextID);
				Tuple<Vector3, Vector3> edge = new Tuple<Vector3, Vector3>(new Vector3(0,0,0), new Vector3(0,0,0));
				graph.addEdge(source, newEdgeNode, edge);
				
				/* Recurse only if it's not the last edge to add. */
				if (edges > 0)
				{
					growGraph(Nodes/2, nextID, MaxEdges, newEdgeNode);
				}
			}
			
			/* Start at last vertex spawned (and not recursed upon). */
			if (newEdgeNode != null)
			{
				source = newEdgeNode;
			}
		}
		
		return source;
	}
	
	Dummy nextNodeID (Dummy lastID)
	{
		return new Dummy(42);
	}
	
	/* After the number of branches in each node is decided we need to rotate them to connect properly. */
	void configureDirections (Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex root, Vector3 rootPos)
	{
		Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex nextNode = root;
		
		if (nextNode != null)
		{
			Dictionary<Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex, Tuple<Vector3, Vector3>> frontier = graph.getNeighbors(nextNode);
			
			foreach (KeyValuePair<Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex, Tuple<Vector3, Vector3>> kvp in frontier)
			{
				float deltaX = Random.Range(0.0f, 2.0f);
				rootPos.x += deltaX;
				rootPos.z += 2.0f - deltaX;
				kvp.Value.first = rootPos;
				configureDirections(graph.getVertex(kvp.Key.data), rootPos);
			}
		}
	}
	
	public void debugVisualize()
	{
		debugVisualize(start, origin);
	}
	
	void debugVisualize (Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex root, Vector3 rootPos)
	{
		Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex nextNode = root;
		
		if (nextNode != null)
		{
			Dictionary<Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex, Tuple<Vector3, Vector3>> frontier = graph.getNeighbors(nextNode);
			
			foreach (KeyValuePair<Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex, Tuple<Vector3, Vector3>> kvp in frontier)
			{
				Debug.DrawRay(rootPos, kvp.Value.first - rootPos);
				debugVisualize(graph.getVertex(kvp.Key.data), kvp.Value.first);
			}
		}
	}
	
	public void visualize ()
	{
		Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex nextNode = start;
		GameObject lastBuilding = null;
		
		while (nextNode != null)
		{
			GameObject obj = buildingSpawner.getRandBuildingExcept(nextNode.edges.Count, lastBuilding);
			lastBuilding = obj;
			GameObject.Instantiate(obj);
		}
	}
	
	public void printGraph ()
	{
		
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
		public GameObject getRandBuilding (int edges)
		{
			int surplusEdges = Random.Range(edges, maxConnectors);
			List<GameObject> list = dict[surplusEdges];
			return dict[surplusEdges][Random.Range(0, list.Count)];
		}
		
		/* Could run forever if except is the only building with that many edges. */
		public GameObject getRandBuildingExcept (int edges, GameObject except)
		{
			GameObject building = getRandBuilding(edges);
			
			/* Reference comparison is fine. */
			while (building == except)
			{
				building = getRandBuilding(edges);
			}
			
			return building;
		}
	}
}