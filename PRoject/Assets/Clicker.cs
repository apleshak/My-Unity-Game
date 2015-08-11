using UnityEngine;
using System.Collections;

public class Clicker : MyMonoBehaviour 
{
	float distance;
	
	// Use this for initialization
	void Start () 
	{
	
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
					ClickHit clickHit = memoizer.GetMemoizedComponent<ClickHit>(hit.collider.gameObject);
					clickHit.GetClicked(hit.point);
					break;
				}
			}
			
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 5.0f);
		}
	}
}
