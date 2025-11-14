using UnityEngine;
using UnityEngine.UI;
using static Deck;
using System.Collections;

public class CardBackOnly : MonoBehaviour
{
    public RectTransform rt;
	public Image image;
	public CardData cardData;
	private bool moving;
	private IEnumerator moveCoroutine;
	
	public void StartMove(Vector2 destination, Vector3 destinationRotation, bool destroyAtEnd = false, bool discardAtEnd = false, bool addToDrawPileAtEnd = false)
	{
		if(moving)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = MoveCard(destination, destinationRotation, destroyAtEnd, discardAtEnd, addToDrawPileAtEnd);
		StartCoroutine(moveCoroutine);
	}
	
	public IEnumerator MoveCard(Vector2 destination, Vector3 destinationRotation, bool destroyAtEnd = false, bool discardAtEnd = false, bool addToDrawPileAtEnd = false)
	{
		moving = true;
		Quaternion originalRotationQ = rt.localRotation;
		Quaternion destinationRotationQ = Quaternion.Euler(destinationRotation);
		Vector2 originalPosition = rt.anchoredPosition;
		float t = 0;
		float moveTime = LocalInterface.instance.animationDuration / 5f;
		while(t < moveTime)
		{
			t += Time.deltaTime;
			rt.localRotation = Quaternion.Lerp(originalRotationQ, destinationRotationQ, t / moveTime);
			rt.anchoredPosition = Vector2.Lerp(originalPosition, destination, t / moveTime);
			yield return null;
		}
		rt.localRotation = destinationRotationQ;
		rt.anchoredPosition = destination;
		moving = false;
		if(discardAtEnd)
		{
			Deck.instance.discardPile.Add(cardData);
			Deck.instance.UpdateCardsInDiscardPile();
		}
		if(addToDrawPileAtEnd)
		{
			Deck.instance.drawPile.Add(cardData);
			Deck.instance.UpdateCardsInDrawPile();
		}
		if(destroyAtEnd)
		{
			Destroy(this.gameObject);
		}
	}
}
