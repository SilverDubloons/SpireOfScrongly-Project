using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BaubleViewer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public RectTransform backdropRT;
	public RectTransform contentRT;
	
	public static BaubleViewer instance;
	public Dictionary<string, BaubleIcon> baubleIcons = new Dictionary<string, BaubleIcon>();
	
	void Awake()
	{
		instance = this;
	}
	
	public void AddBauble(string tag)
	{
		if(baubleIcons.ContainsKey(tag))
		{
			baubleIcons[tag].IncrementBaubleIcon();
			return;
		}
		GameObject newBaubleIconGO = Instantiate(LocalInterface.instance.baubleIconPrefab, Vector3.zero, Quaternion.identity, contentRT);
		BaubleIcon newBaubleIcon = newBaubleIconGO.GetComponent<BaubleIcon>();
		newBaubleIcon.rt.anchorMin = new Vector2(0, 1f);
		newBaubleIcon.rt.anchorMax = new Vector2(0, 1f);
		newBaubleIcon.SetupBaubleIcon(tag);
		float xPos = 26f + (baubleIcons.Count % 2) * 48f;
		int rows = (baubleIcons.Count - 1) / 2 + 1;
		float yPos = -26f - (rows - 1) * 48f;
		newBaubleIcon.rt.anchoredPosition = new Vector2(xPos, yPos);
		contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, 4f + 48f * rows - 1);
		backdropRT.sizeDelta = new Vector2(backdropRT.sizeDelta.x, Mathf.Min(12f + 48f * rows, 360f));
	}
	
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
		
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		
	}
}
