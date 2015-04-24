using UnityEngine;
using System.Collections.Generic;

/* Has connectors that have forward directions that need varying. */
public class CityObjectDescriptor : MonoBehaviour
{
	/* Position, rotation, connected to. */
	public List<GameObject> positions;
	public List<CityObjectDescriptor> connections;
	
	public class Attachment
	{
		public Object prefab;
		public Vector3 position;
	}
}


