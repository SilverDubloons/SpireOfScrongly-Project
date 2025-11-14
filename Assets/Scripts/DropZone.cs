using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
	public bool locked;
	public RectTransform rt;
	public bool cardPlaced;
	public Card placedCard;
	public int dropZoneNumber;
	public bool specialCardsOnly;
	public Image xImage;
	
	public void CardPlaced(Card card)
	{
		cardPlaced = true;
		placedCard = card;
		xImage.rectTransform.SetSiblingIndex(1);
	}
	
	public void CardRemoved()
	{
		cardPlaced = false;
		placedCard = null;
		xImage.gameObject.SetActive(false);
	}
}
