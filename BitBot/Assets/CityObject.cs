using UnityEngine;
using System.Collections.Generic;

public class CityObject
{	
	public Graph<Dummy, int> alleyGraph = new Graph<List<CityObject>, int>();
	
	
	public CityObject ()
	{
		Graph<Dummy, int>.GraphVertex vert0 = alleyGraph.addVertex(new Dummy(0));
		Graph<Dummy, int>.GraphVertex vert1 = alleyGraph.addVertex(new Dummy(1));
		Graph<Dummy, int>.GraphVertex vert2 = alleyGraph.addVertex(new Dummy(2));
		Graph<Dummy, int>.GraphVertex vert3 = alleyGraph.addVertex(new Dummy(3));
		Graph<Dummy, int>.GraphVertex vert4 = alleyGraph.addVertex(new Dummy(4));
		
	}
	
	public static class RectangularBuilding
	{
		
	}
	
	public static class ConcaveSmallBuilding
	{
		
	}
	
	public static class ConcaveLargeBuilding
	{
		
	}
	
	public static class ConvexSmallBuilding
	{
		
	}
	
	public static class ConvexLargeBuilding
	{
		
	}
	
	public static class SquareBuilding
	{
	
	}
	
	public static class Fork
	{
		
	}
	
	public static class Bridge
	{
	
	}
	
	public static class Alley
	{
		
	}
	
	public static class Plaza
	{
		
	}
	
	public static class ForkedBridge
	{
	
	}
	
	public static class LongAlley
	{
	
	}
	
	public static class SquarePlaza
	{
	
	}
	
	public static class CircularPlaza
	{
	
	}
}

