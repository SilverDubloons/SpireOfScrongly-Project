using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MovingObject : MonoBehaviour
{
	public string referenceName;
	public RectTransform rt;
	[System.Serializable]
	public class Location
	{
		public string locationName;
		public Vector3 locationVector3;
		public ButtonPlus[] buttonsToImmediatelyDisable;
		public ButtonPlus[] buttonsToEnableOnFinish;
	}
	public Location[] locations;
	private Dictionary<string, Location> locationsDictionary = new Dictionary<string, Location>();
	private IEnumerator MoveCoroutine;
	private bool moving;
	
	void Awake()
	{
		for(int i = 0; i < locations.Length; i++)
		{
			locationsDictionary.Add(locations[i].locationName, locations[i]);
		}
	}
	
	public void StartMove(string destinationName, float delay = 0f)
	{
		if(moving)
		{
			StopCoroutine(MoveCoroutine);
		}
		MoveCoroutine = MoveObject(locationsDictionary[destinationName], delay);
		StartCoroutine(MoveCoroutine);
	}
	
	public void TeleportTo(string destinationName)
	{
		if(moving)
		{
			StopCoroutine(MoveCoroutine);
		}
		rt.anchoredPosition = locationsDictionary[destinationName].locationVector3;
	}
	
	public IEnumerator MoveObject(Location destination, float delay = 0f)
	{
		moving = true;
		while(delay > 0f)
		{
			delay -= Time.deltaTime;
			yield return null;
		}
		for(int i = 0; i < destination.buttonsToImmediatelyDisable.Length; i++)
		{
			destination.buttonsToImmediatelyDisable[i].ChangeButtonEnabled(false);
		}
		Vector3 origin = rt.anchoredPosition;
		float t = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime;
			rt.anchoredPosition = Vector3.Lerp(origin, destination.locationVector3, LocalInterface.instance.animationCurve.Evaluate(t / LocalInterface.instance.animationDuration));
			yield return null;
		}
		rt.anchoredPosition = destination.locationVector3;
		for(int i = 0; i < destination.buttonsToEnableOnFinish.Length; i++)
		{
			destination.buttonsToEnableOnFinish[i].ChangeButtonEnabled(true);
		}
		moving = false;
	}
}
