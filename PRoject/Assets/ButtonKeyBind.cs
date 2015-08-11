using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonKeyBind : MyMonoBehaviour
{
	public string key;
	public int abilityBoxIdx;
	Button button;
	
	public float fadeDelay;
	
	void Start () 
	{
		button = gameObject.GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		/* Update the icon of the button according with the ability it contains. */
		button.image.sprite = playerDescriptor.abilityBar.GetCurrAbilityInBox(abilityBoxIdx).icon;
		
		if (Input.GetKeyDown(key))
		{
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			PressButton(pointer);
			StartCoroutine(ReleaseButtonCoroutine(pointer));
		}
	}
	
	void PressButton (PointerEventData pointer)
	{
		ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
		ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerDownHandler);
		ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.submitHandler);
		//execute();
	}
	
	IEnumerator ReleaseButtonCoroutine (PointerEventData pointer)
	{
		yield return new WaitForSeconds(fadeDelay);
		ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerExitHandler);
		ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.pointerUpHandler);
	}
	
	public void execute ()
	{
		playerDescriptor.abilityBar.execute(abilityBoxIdx);
	}
}
