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

/*
   Make damn sure that uninstantiated objects can have their scripts accessed
   and that they can be treated as GameObjects
*/
public class LevelBuilder
{	
	public static float[] dirs = 
		{
			0.0f, 22.5f, 45.0f, 67.5f, 
			90.0f, 112.5f, 135.0f, 157.5f, 
			180.0f, 202.5f, 225.0f, 247.5f,
			270.0f, 292.5f, 315.0f, 337.5f,
		};
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
		generateGraph(Nodes, MaxEdges);
		configureDirections();
	}
	
	public void generateGraph (int Nodes, int MaxEdges)
	{
		graph = new Graph<Dummy, Tuple<Vector3, Vector3>>();
		Dummy nextID = new Dummy(0);
		start = graph.addVertex(nextID);
		growGraph(nodes, nextID, maxEdges, start);
	}
	
	public Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex growGraph (int Nodes, Dummy nextID, int MaxEdges,
		   Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex source)
	{
		for (int i = 0; i < Nodes; i++)
		{
			nextID = nextNodeID(nextID);
			Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex newEdgeNode = null;
			
			for (int edges = Random.Range(0, MaxEdges); edges >= 0; edges--)
			{
				nextID = nextNodeID(nextID);
				newEdgeNode = graph.addVertex(nextID);
				Tuple<Vector3, Vector3> edge = new Tuple<Vector3, Vector3>(new Vector3(0,0,0), new Vector3(0,0,0));
				graph.addEdge(source, newEdgeNode, edge);
				
				/* Recursing here only if it's not the last edge to add. */
				if (edges > 0)
				{
					growGraph(Nodes/3, nextID, MaxEdges, newEdgeNode);
				}
			}
			
			/* Select the only edge node that wasn't recursed on. */
			source = newEdgeNode;
		}
		
		return source;
	}
	
	public Dummy nextNodeID (Dummy lastID)
	{
		return new Dummy(42);
	}
	
	public void configureDirections ()
	{
		return;
	}
	
	public void visualize ()
	{
		Graph<Dummy, Tuple<Vector3, Vector3>>.GraphVertex nextNode = start;
		GameObject lastBuilding = null;
		
		while (nextNode != null)
		{
			GameObject obj = buildingSpawner.getRandBuilding(nextNode.edges.Count, lastBuilding);
			GameObject.Instantiate(obj);
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
			
			foreach (GameObject obj in Resources.LoadAll("Prefabs/Buildings"))
			{
				CityObjectDescriptor descr = (CityObjectDescriptor)obj.GetComponent<CityObjectDescriptor>();
				connectors = descr.connections.Count;
				
				if (connectors > maxConnectors)
				{
					maxConnectors = connectors;
				}
				
				dict[connectors].Add(obj);
			}
		}
		
		public GameObject getRandBuilding (int edges)
		{
			int surplusEdges = Random.Range(edges, maxConnectors);
			List<GameObject> list = dict[surplusEdges];
			return dict[surplusEdges][Random.Range(0, list.Count)];
		}
		
		public GameObject getRandBuilding (int edges, GameObject except)
		{
			GameObject building = getRandBuilding(edges);
			
			while (building == except)
			{
				building = getRandBuilding(edges);
			}
			
			return building;
		}
	}
}