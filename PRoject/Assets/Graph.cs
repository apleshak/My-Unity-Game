using System;
using System.Collections;
using System.Collections.Generic;
using V = Graph<Structure>.Vertex;
using System.Linq;
/* 
	T is the data
	K is the edge description/weight
*/
public class Graph<T, K> where T : class
{
	HashSet<GraphVertex> vertices;
	public int count;
	
	public Graph ()
	{
		vertices = new HashSet<GraphVertex>();
		count = 0;
	}
	
	public GraphVertex getVertex (T data)
	{
		foreach (GraphVertex vertex in vertices)
		{
			if (vertex.data == data)
			{
				return vertex;
			}
		}
		
		return null;
	}
	
	public Dictionary<GraphVertex, K> getNeighbors (GraphVertex vertex)
	{
		return vertex.edges;
	}
	
	public GraphVertex addVertex (T data)
	{
		GraphVertex newVert = new GraphVertex(data);
		vertices.Add(newVert);
		count += 1;
		return newVert;
	}
	
	public bool containsVertex (T data)
	{
		foreach (GraphVertex vertex in vertices)
		{
			if (vertex.data == data)
			{
				return true;
			}
		}
		
		return false;
	}
	
	/* Directed. */
	public void addEdge (GraphVertex from, GraphVertex to, K weight)
	{
		from.edges[to] = weight;
	}
	
	public class GraphVertex
	{
		public T data;
		public Dictionary<GraphVertex, K> edges;
		
		public GraphVertex (T Data)
		{
			data = Data;
			edges = new Dictionary<GraphVertex, K>();
		}
		
		public K getEdge (GraphVertex toVertex)
		{
			return edges[toVertex];
		}
		
		public void setEdge (GraphVertex toVertex, K edge)
		{
			edges[toVertex] = edge;
		}
	}
}

//TODO: make sure linking vertices from other graphs works
//TODO: figure out
//Dictionary mapping vertices to sets of neighbors 
public class Graph<T>
{
	protected Dictionary<Vertex, HashSet<Vertex>> vertexMap;
	public Vertex firstVertex;
	
	public Graph ()
	{
		vertexMap = new Dictionary<Vertex, HashSet<Vertex>>();
	}
	
	public static Graph<K> Map<K> (Func<T, K> f)
	{
		Graph<K> g = new Graph<K>();
		
		foreach (Vertex from in vertexMap.Keys)
		{
			Graph<K>.Vertex newFrom = g.AddVertex(f(from));
			
			foreach (Vertex to in vertexMap[from])
			{
				Graph<K>.Vertex newTo = g.AddVertex(f(to));
				g.AddEdge(newFrom, newTo);
			}
		}
		
		return g;
	}
	
	public HashSet<Vertex> GetNeighbors (Vertex v)
	{
		return vertexMap[v];
	}
	
	public Vertex GetVertex (T ID)
	{
		foreach (Vertex v in vertexMap.Keys)
		{
			if (v.data.Equals(ID))
				return v;
		}
		
		return null;
	}
	
	//make sure that if item is already present we don't wipe it out
	public Vertex AddVertex (T item)
	{
		Vertex newV = new Vertex(item);
		
		if (firstVertex == null)
			firstVertex = newV;
			
		vertexMap[newV] = new HashSet<Vertex>();
		return newV;
	}
	
	public void AddEdge (T from, T to)
	{
		Vertex fromV = GetVertex(from);
		Vertex toV = GetVertex(to);
		AddEdge(fromV, toV);
	}
	
	public void AddEdge (Vertex from, Vertex to)
	{
		vertexMap[from].Add(to);
	}
	
	public void RemoveVertex (T item)
	{
		Vertex itemV = GetVertex(item);
		RemoveVertex(itemV);
	}
	
	public void RemoveVertex (Vertex vertex)
	{
		vertexMap.Remove(vertex);
	}
	
	public void RemoveEdge (T from, T to)
	{
		Vertex fromV = GetVertex(from);
		Vertex toV = GetVertex(to);
		RemoveEdge(fromV, toV);
	}
	
	public void RemoveEdge (Vertex from, Vertex to)
	{
		vertexMap[from].Remove(to);
	}
	
	public Tuple<Vertex, Vertex> MinMaxBetweenness ()
	{
		return new Tuple<Vertex, Vertex>(null, null);
	}
	
	public Tuple<Vertex, Vertex> MinMaxCloseness ()
	{
		return new Tuple<Vertex, Vertex>(null, null);
	}
	
	public Tuple<Vertex, Vertex> MinMaxEigenvector ()
	{
		return new Tuple<Vertex, Vertex>(null, null);
	}
	
	public Tuple<Vertex, Vertex> MinMaxDegree ()
	{
		return new Tuple<Vertex, Vertex>(null, null);
	}
	
	public Tuple<Vertex, Vertex> MinMaxHarmonic ()
	{
		return new Tuple<Vertex, Vertex>(null, null);
	}
	
	public Tuple<Vertex, Vertex> MinMaxKatz ()
	{
		return new Tuple<Vertex, Vertex>(null, null);
	}
	
	public class Vertex
	{
		public T data;
		
		public Vertex (T item)
		{
			data = item;
		}
		
		//TODO: how do I get the instance of the base class?
		public HashSet<Vertex> Neighbors ()
		{
			return new HashSet<Vertex>();
		}
	}
}