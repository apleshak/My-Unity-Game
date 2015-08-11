using UnityEngine;
using System.Collections;

public class ClickMove : MyMonoBehaviour 
{
	/* To display the drag animation*/
	public GameObject unit;
	Animator animator;
	MovementCC movement;
	ChaseResource chaseResource;
	bool selected;
	
	// Use this for initialization
	void Start () 
	{
		animator = memoizer.GetMemoizedComponent<Animator>(unit);
		movement = memoizer.GetMemoizedComponent<MovementCC>(gameObject);
		chaseResource = memoizer.GetMemoizedComponent<ChaseResource>(gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray);
			
			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.gameObject.tag == "Gnome" && hit.collider.gameObject == unit)
				{
					animator.SetBool("Dragged", true);
					selected = true;
					chaseResource.enabled = false;
					break;
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			animator.SetBool("Dragged", false);
			selected = false;
			chaseResource.enabled = true;
		}
		
		if (selected)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray);
			
			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.gameObject.tag == "Walkable")
				{
					movement.Approach(hit.point);
					break;
				}
			}
			
			Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 5.0f);
		}
	}
}
