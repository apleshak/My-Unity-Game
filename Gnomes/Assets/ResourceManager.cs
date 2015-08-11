using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceManager : MyMonoBehaviour 
{
	public int count = 0;
	public Text textOutput;
	public Object resourcePrefab;
	Rigidbody selectedRigidbody;
	bool selected = false;
	float clickTime = 0.15f;
	float timeSinceClick = 0.0f;
	GameObject selectedObj;
	float distToMaintain;
	float fallForce = 200.0f;
	
	public void SpawnResource ()
	{
		Vector3 pos = new Vector3(transform.position.x, transform.position.y + Random.Range(1.0f, 5.0f), transform.position.z);
		GameObject.Instantiate(resourcePrefab, pos, Quaternion.identity);
	}
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//update text and timers
		textOutput.text = count.ToString();
		timeSinceClick += Time.deltaTime;
		
		//check for object destruction
		if (selected)
		{
			if (selectedObj == null)
			{
				selected = false;
				return;
			}
			else
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				selectedObj.transform.position = ray.GetPoint(distToMaintain);
			}
		}
		
		if (Input.GetMouseButton(0))
		{
			if (!selected)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hits = Physics.RaycastAll(ray);
				
				foreach (RaycastHit hit in hits)
				{
					if (hit.collider.gameObject.tag == "Resource")
					{
						selected = true;
						selectedObj = hit.collider.gameObject;
						selectedRigidbody = selectedObj.GetComponent<Rigidbody>();
						selectedRigidbody.useGravity = false;
						selectedRigidbody.AddTorque(selectedObj.transform.position.normalized * 7);
						distToMaintain = Vector3.Distance(selectedObj.transform.position, Camera.main.transform.position);
						
						break;
					}
				}
			}
		}
		else
		{
			if (selected)
			{
				selectedRigidbody.useGravity = true;
				selectedRigidbody.AddForce(Vector3.down * fallForce);
				
				if (timeSinceClick <= clickTime)
				{
					Destroy(selectedObj);
					count++;
				}
			}
			
			selected = false;
			timeSinceClick = 0.0f;
		}
	}
}
