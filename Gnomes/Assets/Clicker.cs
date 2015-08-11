using UnityEngine;
using System.Collections;

public class Clicker : MyMonoBehaviour 
{
	Tuple<MovementCC, EntityDescriptor> selectedUnit;
	// Use this for initialization
	void Start () 
	{
		selectedUnit = new Tuple<MovementCC, EntityDescriptor>(null, null);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray);
			
			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.gameObject.tag == "Boss")
				{
					selectedUnit.first = null;
					selectedUnit.second = null;
					
					ClickHit clickHit = memoizer.GetMemoizedComponent<ClickHit>(hit.collider.gameObject);
					clickHit.GetClicked(hit.point);
					break;
				}
				else if (hit.collider.gameObject.tag == "Gnome")
				{
					selectedUnit.first = memoizer.GetMemoizedComponent<MovementCC>(hit.collider.gameObject);
					selectedUnit.second = memoizer.GetMemoizedComponent<EntityDescriptor>(hit.collider.gameObject);
				}
				else
				{
					selectedUnit.first = null;
					selectedUnit.second = null;
				}
			}
			
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 5.0f);
		}
	}
}
