using UnityEngine;
using System.Collections;

/* A spherical sector we can use to check if an ability can hit and LOS. */
public static class SphericalSector
{
	/* angleOffset is the angle between the direction vector and either edge of the vision cone. */
	public static bool inRange(Vector3 source, Vector3 destination, float angleOffset, 
	                           Vector3 direction, float radius)
	{
		/* Check distance first. */
		if (Vector3.Distance(source, destination) > radius)
		{
			return false;
		}
		
		/* Check angle. Hope this is correct. */
		Vector3 dir = (source - destination).normalized;
		float dot = Vector3.Dot(dir, direction);
		
		if (Mathf.Abs(Mathf.Acos(dot) * Mathf.Deg2Rad) > angleOffset)
		{
			return false;
		}
		
		return true;
	}
}

