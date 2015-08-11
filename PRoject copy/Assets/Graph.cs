using System.Collections;
using System.Collections.Generic;

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