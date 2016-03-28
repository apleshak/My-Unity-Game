using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Orientation = Tuple<UnityEngine.Vector3, UnityEngine.Vector3>;

//This is a structure that holds info necessary to spawn prefabs of structures
//Collision detection happens through the mesh collider component (hopefully it
//is possible to make these calls in 1 frame without relying on the builtin event handlers like
// OnCollisionEnter)
//
//One instance of every structure (and building) that will appear in the level must be spawned to
//retrieve the data that this class will hold. This data is stored in the BuildingSpawner
//class and can be accessed as a regular database lookup.
//
//Every structure has:
//	1) Position (x, y, z)
//	2) Rotation around z axis
//	3) A collection of attachment points with:
//		- Position
//		- Rotation around z axis indicating which direction the point faces
//			from the center of the structure when the structure's rotation is 0.
//
public class Structure
{
	static float offset = 0.1f;
	public Orientation orientation;
	public string colliderName;
	public List<Orientation> attachPoints;
	
	public Structure (Vector3 pos, Vector3 forward, ICollection<Orientation> AttachPoints)
	{
		orientation = new Orientation(pos, forward);
		attachPoints = AttachPoints;
	}
	
	//Returns a list of orientations fro this structure that align each of the attch points of this structure to a target
	//attach point on another structure. The resulting alignments ensure there are no collisions at the 
	//attachment point. They do NOT guarantee a lack of collisions with other structures.
	public List<Orientation> GetAlignments (Orientation alignTo)
	{
		//Map attach points to orientations of this structure
		return attachPoints.Select<Orientation, Orientation>((p) => {return GetAlignment(p, alignTo);});
	}
	
	//TODO: this is not correct in terms of rotation
	Orientation GetAlignment (Orientation alignThis, Orientation toThis)
	{
		//Calculate the distance between the structure center and the local attach point.
		float centerToMyAttach = Vector3.Distance(orientation.first, alignThis.first);
		
		//Move the structure to the target attach point's position, then slide it along that point's forward
		//vector for centerToMyAttach units.
		return new Orientation(toThis.first + (toThis.second * (centerToMyAttach + offset)), -toThis.second);
	}
	
	//TODO: some kind of collision detection
	public bool HasCollision ()
	{
		return false;
	}
}