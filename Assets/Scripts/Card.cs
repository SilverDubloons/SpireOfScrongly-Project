using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static Deck;

public class Card : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // public int rank;
	// public int suit;
	// public bool isSpecialCard;
	// public string specialCardName;
	// public float[] cardBaseValue;
	// public float[] cardMultiplier;
	public RectTransform rt;
	public Image rankImage;
	public RectTransform rankImageRT;
	public Image bigSuitImage;
	public RectTransform bigSuitImageRT;
	public Image detailImage;
	public RectTransform detailImageRT;
	public Image xImage;
	public Image front;
	public Image back;
	public bool canMove;
	public bool faceDown;
	public bool cardIsInShop;
	public int originalSiblingIndex;
	public DropZone dropZonePlacedIn;
	
	private IEnumerator moveCoroutine;
	public bool moving;
	public CardData cardData;
	
	private IEnumerator fadeCoroutine;
	private bool fading;
	
	public void StartMove(Vector2 destination, Vector3 destinationRotation, bool canMoveAtEnd = true, bool destroyAtEnd = false, bool discardAtEnd = false)
	{
		if(moving)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = MoveCard(destination, destinationRotation, canMoveAtEnd, destroyAtEnd, discardAtEnd);
		StartCoroutine(moveCoroutine);
	}
	
	public IEnumerator MoveCard(Vector2 destination, Vector3 destinationRotation, bool canMoveAtEnd = true, bool destroyAtEnd = false, bool discardAtEnd = false)
	{
		canMove = false;
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
		canMove = canMoveAtEnd;
		if(discardAtEnd)
		{
			Deck.instance.discardPile.Add(cardData);
			Deck.instance.UpdateCardsInDiscardPile();
			Deck.instance.cardsInHand.Remove(this);
		}
		if(destroyAtEnd)
		{
			Destroy(this.gameObject);
		}
	}
	
	public void ChangeFaceDown(bool isFaceDown)
	{
		faceDown = isFaceDown;
		front.gameObject.SetActive(!faceDown);
		back.gameObject.SetActive(faceDown);
	}
	
	public IEnumerator Flip(bool allowMovementAfterwards = true)
	{
		ChangeCanMove(false);
		Vector3 originalScale = rt.localScale;
		Vector3 destinationScale = rt.localScale;
		destinationScale.x = 0;
		float flipTime = LocalInterface.instance.animationDuration / 5f;
		float t = 0;
		while(t < flipTime)
		{
			t += Time.deltaTime;
			rt.localScale = Vector3.Lerp(originalScale, destinationScale, t / flipTime);
			yield return null;
		}
		ChangeFaceDown(!faceDown);
		t = 0;
		while(t < flipTime)
		{
			t += Time.deltaTime;
			rt.localScale = Vector3.Lerp(destinationScale, originalScale, t / flipTime);
			yield return null;
		}
		rt.localScale = Vector3.one;
		if(allowMovementAfterwards)
		{
			ChangeCanMove(true);
		}
	}
	
	public void ChangeCanMove(bool canCardMove)
	{
		if(!canCardMove)
		{
			rt.localScale = Vector3.one;
		}
		canMove = canCardMove;
	}
	
	public void OnBeginDrag(PointerEventData pointerEventData)
    {
		if(cardIsInShop || !canMove || HandArea.instance.handLockedIn)
		{
			return;
		}
		if(dropZonePlacedIn != null)
		{
			dropZonePlacedIn.CardRemoved();
			PlayArea.instance.HandUpdated();
			dropZonePlacedIn = null;
		}
		SoundManager.instance.PlayCardPickupSound();
		rt.rotation = Quaternion.identity;
		originalSiblingIndex = transform.GetSiblingIndex();
		transform.SetParent(HandArea.instance.looseCardParent);
	}
	
	public void OnDrag(PointerEventData pointerEventData)
    {
		if(cardIsInShop || !canMove || HandArea.instance.handLockedIn)
		{
			return;
		}
		Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) * LocalInterface.instance.referenceResolution.x - LocalInterface.instance.referenceResolution.x / 2, (Input.mousePosition.y / Screen.height) * LocalInterface.instance.referenceResolution.y - LocalInterface.instance.referenceResolution.y / 2);
		rt.anchoredPosition = mousePos;
		rt.rotation = Quaternion.identity;
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, results);
		foreach (RaycastResult result in results)
		{
			if(result.gameObject != null)
			{
				if(result.gameObject == HandArea.instance.handAreaCardZone)
				{
					int desiredIndex = 0;
					for(int i = 0; i < HandArea.instance.cardParent.childCount; i++)
					{
						if(rt.anchoredPosition.x < HandArea.instance.cardParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x)
						{
							desiredIndex = i;
							break;
						}
						if(i == HandArea.instance.cardParent.childCount - 1)
						{
							desiredIndex = i + 1;
						}
					}
					HandArea.instance.siblingIndexOfLooseCard = desiredIndex;
					HandArea.instance.ReorganizeHand();
					return;
				}
			}
		}
		HandArea.instance.siblingIndexOfLooseCard = -1;
		HandArea.instance.ReorganizeHand();
	}
	
	public void OnEndDrag(PointerEventData pointerEventData)
	{
		if(cardIsInShop || !canMove || HandArea.instance.handLockedIn)
		{
			return;
		}
		SoundManager.instance.PlayCardDropSound();
		Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) * LocalInterface.instance.referenceResolution.x - LocalInterface.instance.referenceResolution.x / 2, (Input.mousePosition.y / Screen.height) * LocalInterface.instance.referenceResolution.y - LocalInterface.instance.referenceResolution.y / 2);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, results);
		foreach (RaycastResult result in results)
		{
			if (result.gameObject != null)
			{
				if(result.gameObject == HandArea.instance.handAreaCardZone)
				{
					int desiredIndex = 0;
					for(int i = 0; i < HandArea.instance.cardParent.childCount; i++)
					{
						if(rt.anchoredPosition.x < HandArea.instance.cardParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x)
						{
							desiredIndex = i;
							break;
						}
						if(i == HandArea.instance.cardParent.childCount - 1)
						{
							desiredIndex = i + 1;
						}
					}
					HandArea.instance.siblingIndexOfLooseCard = desiredIndex;
					transform.SetParent(HandArea.instance.cardParent);
					transform.SetSiblingIndex(desiredIndex);
					HandArea.instance.siblingIndexOfLooseCard = -1;
					if(desiredIndex != originalSiblingIndex)
					{
						HandArea.instance.ChangeAlwaysSortType(0);
					}
					HandArea.instance.ReorganizeHand();
					return;
				}
				DropZone dropZone;
				if(result.gameObject.transform.TryGetComponent(out dropZone))
				{
					if(!dropZone.locked && !PlayArea.instance.locked)
					{
						if(!dropZone.cardPlaced || dropZone.cardPlaced && dropZone.placedCard.canMove)
						{
							if((!cardData.isSpecialCard && dropZone.specialCardsOnly) || (cardData.isSpecialCard && !dropZone.specialCardsOnly && Baubles.instance.GetBaubleImpactIntByTag("AllowSpecialCardsInStandardDropZones") == 0))
							{
								if(cardData.isSpecialCard)
								{
									Debug.Log("Can't place special cards into standard drop zones without the Finger Monster bauble");
								}
								else
								{
									Debug.Log("Standard cards can't go into special card drop zones");
								}
							}
							else
							{
								dropZonePlacedIn = dropZone;
								if(dropZone.cardPlaced)
								{
									dropZone.placedCard.dropZonePlacedIn = null;
									dropZone.placedCard.transform.SetParent(HandArea.instance.cardParent);
									dropZone.placedCard.transform.SetSiblingIndex(0);
									if(dropZone.placedCard.cardData.isSpecialCard)
									{
										dropZone.placedCard.RevertToOriginalImage();
									}
									HandArea.instance.ReorganizeHand();
								}
								transform.SetParent(dropZone.transform);
								rt.anchoredPosition = Vector2.zero;
								dropZone.CardPlaced(this);
								if(cardData.isSpecialCard)
								{
									UpdateToDropZoneImage();
								}
								PlayArea.instance.HandUpdated();
								return;
							}
						}
					}
				}
			}
		}
		transform.SetParent(HandArea.instance.cardParent);
		transform.SetSiblingIndex(originalSiblingIndex);
		HandArea.instance.ReorganizeHand();
	}
	
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		CheckCheatInput();
		if(cardIsInShop || !canMove || HandArea.instance.handLockedIn)
		{
			return;
		}
		if(HandArea.instance.selectedCards.Contains(this))
		{
			HandArea.instance.selectedCards.Remove(this);
			HandArea.instance.SelectedCardsUpdated();
			rt.anchoredPosition = rt.anchoredPosition - Vector2.up * HandArea.instance.distanceDifferenceOfSelectedCard;
			SoundManager.instance.PlayCardDropSound();
		}
		else
		{
			if(transform.parent == HandArea.instance.cardParent && canMove)
			{
				if((cardData.isSpecialCard && HandArea.instance.GetNumberOfCardsOfTypeInList(HandArea.instance.selectedCards, true) < GameManager.instance.baseNumberOfPlayableSpecialCards + Baubles.instance.GetBaubleImpactIntByTag("IncreasePlayableSpecialCardCount")) || (!cardData.isSpecialCard && HandArea.instance.GetNumberOfCardsOfTypeInList(HandArea.instance.selectedCards, false) < GameManager.instance.baseNumberOfPlayableStandardCards + Baubles.instance.GetBaubleImpactIntByTag("IncreasePlayableStandardCardCount")))
				{
					HandArea.instance.selectedCards.Add(this);
					HandArea.instance.SelectedCardsUpdated();
					rt.anchoredPosition = rt.anchoredPosition + Vector2.up * HandArea.instance.distanceDifferenceOfSelectedCard;
					SoundManager.instance.PlayCardPickupSound();
				}
				else
				{
					int playableCount = cardData.isSpecialCard ? GameManager.instance.baseNumberOfPlayableSpecialCards + Baubles.instance.GetBaubleImpactIntByTag("IncreasePlayableSpecialCardCount") : GameManager.instance.baseNumberOfPlayableStandardCards + Baubles.instance.GetBaubleImpactIntByTag("IncreasePlayableStandardCardCount");
					string specialOrStandard = cardData.isSpecialCard ? "special" : "standard";
					int IncreasePlayableStandardCardCountBaubleImpact = Baubles.instance.GetBaubleImpactIntByTag("IncreasePlayableStandardCardCount");
					Debug.Log($"You may only play at most {playableCount} {specialOrStandard} cards. GameManager.instance.baseNumberOfPlayableStandardCards = {GameManager.instance.baseNumberOfPlayableStandardCards}, IncreasePlayableStandardCardCountBaubleImpact = {IncreasePlayableStandardCardCountBaubleImpact}");
					// you may only play n number of (special / standard) cards at once!
				}
			}
		}
	}
	
	public void StartFade(float duration, float startingAlpha, float endingAlpha)
	{
		if(fading)
		{
			StopCoroutine(fadeCoroutine);
		}
		fadeCoroutine = Fade(duration, startingAlpha, endingAlpha);
		StartCoroutine(fadeCoroutine);
	}
	
	private IEnumerator Fade(float duration, float startingAlpha, float endingAlpha)
	{
		fading = true;
		float t = 0;
		Color rankImageStartingColor = rankImage.color;
		Color bigSuitImageStartingColor = bigSuitImage.color;
		Color detailImageStartingColor = detailImage.color;
		Color frontStartingColor = front.color;
		Color backStartingColor = back.color;
		Color rankImageEndingColor = new Color(rankImageStartingColor.r, rankImageStartingColor.g, rankImageStartingColor.b, endingAlpha);
		Color bigSuitImageEndingColor = new Color(bigSuitImageStartingColor.r, bigSuitImageStartingColor.g, bigSuitImageStartingColor.b, endingAlpha);
		Color detailImageEndingColor = new Color(detailImageStartingColor.r, detailImageStartingColor.g, detailImageStartingColor.b, endingAlpha);
		Color frontEndingColor = new Color(frontStartingColor.r, frontStartingColor.g, frontStartingColor.b, endingAlpha);
		Color backEndingColor = new Color(backStartingColor.r, backStartingColor.g, backStartingColor.b, endingAlpha);
		while(t < duration)
		{
			t += Time.deltaTime;
			rankImage.color = Color.Lerp(rankImageStartingColor, rankImageEndingColor, t / duration);
			bigSuitImage.color = Color.Lerp(bigSuitImageStartingColor, bigSuitImageEndingColor, t / duration);
			detailImage.color = Color.Lerp(detailImageStartingColor, detailImageEndingColor, t / duration);
			front.color = Color.Lerp(frontStartingColor, frontEndingColor, t / duration);
			back.color = Color.Lerp(backStartingColor, backEndingColor, t / duration);
			yield return null;
		}
		fading = false;
	}
	
	public void RevertToOriginalImage()
	{
		
	}
	
	public void UpdateToDropZoneImage()
	{
		
	}
	
	public void UpdateGraphics()
	{
		name = LocalInterface.instance.ConvertRankAndSuitToString(cardData.rank, cardData.suit);
		if(cardData.suit < 4)
		{
			rankImage.color = LocalInterface.instance.suitColors[cardData.suit];
			bigSuitImage.color = LocalInterface.instance.suitColors[cardData.suit];
			if(cardData.rank <= 8 || cardData.rank == 12)
			{
				detailImage.color = LocalInterface.instance.suitColors[cardData.suit];
			}
			rankImage.sprite = HandArea.instance.rankSprites[cardData.rank];
		}
		else
		{
			rankImage.sprite = HandArea.instance.rankSprites[cardData.rank + 13];
			rankImageRT.anchoredPosition = rankImageRT.anchoredPosition;
		}
		rankImageRT.sizeDelta = new Vector2(HandArea.instance.rankSprites[cardData.rank].rect.width, HandArea.instance.rankSprites[cardData.rank].rect.height);
		bigSuitImage.sprite = HandArea.instance.bigSuitSprites[cardData.suit];
		bigSuitImageRT.sizeDelta = new Vector2(HandArea.instance.bigSuitSprites[cardData.suit].rect.width, HandArea.instance.bigSuitSprites[cardData.suit].rect.height);
		int cardNumber = cardData.suit * 13 + cardData.rank;
		detailImage.sprite = HandArea.instance.cardDetails[cardNumber];
		detailImageRT.sizeDelta = new Vector2(HandArea.instance.cardDetails[cardNumber].rect.width, HandArea.instance.cardDetails[cardNumber].rect.height);
	}
	
	public void ChangeSuit(int newSuit)
	{
		cardData.baseValue[newSuit] = cardData.baseValue[cardData.suit];
		cardData.baseValue[cardData.suit] = 0;
		cardData.suit = newSuit;
		UpdateGraphics();
	}
	
	public void ChangeRank(int newRank)
	{
		float rankValueChange = GetBaseValueByRank(newRank) - GetBaseValueByRank(cardData.rank);
		cardData.baseValue[cardData.suit] += rankValueChange;
		cardData.rank = newRank;
		UpdateGraphics();
	}
	
	public void CheckCheatInput()
	{
		if(Preferences.instance.cheatsOn)
		{
			if(Input.GetKey(KeyCode.S))
			{
				ChangeSuit(0);
			}
			if(Input.GetKey(KeyCode.C))
			{
				ChangeSuit(1);
			}
			if(Input.GetKey(KeyCode.H))
			{
				ChangeSuit(2);
			}
			if(Input.GetKey(KeyCode.D))
			{
				ChangeSuit(3);
			}
			if(Input.GetKey(KeyCode.R))
			{
				ChangeSuit(4);
			}
			if(Input.GetKey(KeyCode.Alpha2))
			{
				ChangeRank(0);
			}
			if(Input.GetKey(KeyCode.Alpha3))
			{
				ChangeRank(1);
			}
			if(Input.GetKey(KeyCode.Alpha4))
			{
				ChangeRank(2);
			}
			if(Input.GetKey(KeyCode.Alpha5))
			{
				ChangeRank(3);
			}
			if(Input.GetKey(KeyCode.Alpha6))
			{
				ChangeRank(4);
			}
			if(Input.GetKey(KeyCode.Alpha7))
			{
				ChangeRank(5);
			}
			if(Input.GetKey(KeyCode.Alpha8))
			{
				ChangeRank(6);
			}
			if(Input.GetKey(KeyCode.Alpha9))
			{
				ChangeRank(7);
			}
			if(Input.GetKey(KeyCode.Alpha0))
			{
				ChangeRank(8);
			}
			if(Input.GetKey(KeyCode.J))
			{
				ChangeRank(9);
			}
			if(Input.GetKey(KeyCode.Q))
			{
				ChangeRank(10);
			}
			if(Input.GetKey(KeyCode.K))
			{
				ChangeRank(11);
			}
			if(Input.GetKey(KeyCode.A))
			{
				ChangeRank(12);
			}
			if(Input.GetKey(KeyCode.I))
			{
				print("SiblingIndex= " + transform.GetSiblingIndex());
			}
		}
	}
}