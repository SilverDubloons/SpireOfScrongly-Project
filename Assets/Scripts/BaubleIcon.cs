using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BaubleIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public RectTransform rt;
    public Image image;
	public Label label;
	private int quantityOwned;
	public string baubleTag;
	
	public void SetupBaubleIcon(string baubleTag)
	{
		this.baubleTag = baubleTag;
		image.sprite = Baubles.instance.baubles[baubleTag].sprite;
		quantityOwned = 1;
	}
	
	public void IncrementBaubleIcon()
	{
		quantityOwned++;
		if(!label.gameObject.activeSelf)
		{
			label.gameObject.SetActive(true);
		}
		label.ChangeText(quantityOwned.ToString());
	}
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		
	}
}
