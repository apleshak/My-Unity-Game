using UnityEngine;
using System.Collections;

public class ClickHit : MonoBehaviour 
{
	EntityDescriptor descriptor;
	Animator animator;
	public Object hitPrefab;
	
	// Use this for initialization
	void Start () 
	{
		descriptor = GetComponent<EntityDescriptor>();
		animator = GetComponent<Animator>();
	}
	
	public void GetClicked (Vector3 location)
	{
		descriptor.TakeDamage(1.0f);
		animator.SetTrigger("Clicked");
		Instantiate(hitPrefab, location, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
