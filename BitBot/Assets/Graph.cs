using System.Collections.Generic;

/* 
	T is the data
	K is the edge escription/weight
*/
public class Graph<T, K> where T : class
{
	HashSet<GraphVertex> vertices;
	
	public Graph ()
	{
		vertices = new HashSet<GraphVertex>();
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
	
	public GraphVertex addVertex (T data)
	{
		GraphVertex newVert = new GraphVertex(data);
		vertices.Add(newVert);
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
	
	/* 
	   L is the data
	   M is the edge escription/weight
	*/
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