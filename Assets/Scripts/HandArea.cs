using UnityEngine;
using static Deck;
using System.Collections.Generic;
using System.Collections;

public class HandArea : MonoBehaviour
{
	public GameObject cardPrefab;
	public GameObject cardBackOnlyPrefab;
	public RectTransform cardParent;
	public RectTransform looseCardParent;
	public RectTransform discardedCardsParent;
	public RectTransform playedCardsParent;
	public RectTransform shufflingCardsParent;
	public Sprite[] rankSprites;
	public Sprite[] bigSuitSprites;
	public Sprite[] cardDetails;
	public Sprite[] nonStandardCardDetails;
	
	public ButtonPlus sortByRankButton;
	public ButtonPlus sortBySuitButton;
	public ButtonPlus selectAllButton;
	public ButtonPlus playSelectedButton;
	public ButtonPlus discardButton;
	public ButtonPlus recallButton;
	public int alwaysSortType; // 0 = no sorting, 1 = rank, 2 = suit
	public AnimationCurve handParabola;
	public AnimationCurve handRotationParabola;
	
	public Vector2 deckLocation;
	public Vector2 discardPileLocation;
	
	public List<Card> selectedCards;
	public int siblingIndexOfLooseCard = -1;
	public GameObject handAreaCardZone;
	public Label discardsRemainingLabel;
	
	public static HandArea instance;
	public float distanceDifferenceOfSelectedCard;
	public float maxWidthOfPlayedHand;
	public float idealDistanceBetweenCardsInPlayedHand;
	public float yPositionOfPlayedHand;
	
	public bool handLockedIn;
	public List<Card> lockedInHand = new List<Card>();
	public float[] lockedInHandBaseValues;
	public float[] lockedInHandMultipliers;
	
	private bool shufflingDiscardPileIntoDrawPile;
	
	void Awake()
	{
		instance = this;
	}
	
	public void StartDrawCards(int numberOfCardsToDraw, float delay = 0)
	{
		StartCoroutine(DrawCardsCoroutine(numberOfCardsToDraw, delay));
	}
	
	public IEnumerator DrawCardsCoroutine(int numberOfCardsToDraw, float delay = 0)
	{
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime;
			yield return null;
		}
		for(int i = 0; i < numberOfCardsToDraw; i++)
		{
			if(Deck.instance.drawPile.Count == 0)
			{
				ReorganizeHand();
				StartCoroutine(ShuffleDiscardPileIntoDrawPile());
				while(shufflingDiscardPileIntoDrawPile)
				{
					yield return null;
				}
				yield return new WaitForSeconds(LocalInterface.instance.animationDuration / 2);
				Deck.instance.ShuffleDrawPile();
			}
			Card newCard = SpawnCard(Deck.instance.drawPile[0], deckLocation, cardParent);
			if(newCard.cardData.isSpecialCard)
			{
				i--;
			}
			Deck.instance.cardsInHand.Add(newCard);
			Deck.instance.drawPile.RemoveAt(0);
			ReorganizeHand();
			Deck.instance.UpdateCardsInDrawPile();
			yield return new WaitForSeconds(LocalInterface.instance.animationDuration / 5);
		}
	}
	
	public IEnumerator ShuffleDiscardPileIntoDrawPile(float delay = 0f)
	{
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime;
			yield return null;
		}
		int cardsInDiscardPile = Deck.instance.discardPile.Count;
		shufflingDiscardPileIntoDrawPile = true;
		SoundManager.instance.PlayCardShuffleSound();
		while(Deck.instance.discardPile.Count > 0)
		{
			GameObject newCardBackOnlyGO = Instantiate(cardBackOnlyPrefab, Vector3.zero, Quaternion.identity, shufflingCardsParent);
			CardBackOnly newCardBackOnly = newCardBackOnlyGO.GetComponent<CardBackOnly>();
			newCardBackOnly.rt.anchoredPosition = discardPileLocation;
			newCardBackOnly.cardData = Deck.instance.discardPile[Deck.instance.discardPile.Count - 1];
			Deck.instance.discardPile.RemoveAt(Deck.instance.discardPile.Count - 1);
			Deck.instance.UpdateCardsInDiscardPile();
			newCardBackOnly.StartMove(deckLocation, Vector3.zero, true, false, true);
			yield return new WaitForSeconds(LocalInterface.instance.animationDuration / cardsInDiscardPile);
		}
		shufflingDiscardPileIntoDrawPile = false;
	}
	
	public void SortHandClicked(int sortType)
	{
		if(sortType == 1)
		{
			if(alwaysSortType == 1)
			{
				ChangeAlwaysSortType(0);
			}
			else
			{
				ChangeAlwaysSortType(1);
			}
		}
		else if(sortType == 2)
		{
			if(alwaysSortType == 2)
			{
				ChangeAlwaysSortType(0);
			}
			else
			{
				ChangeAlwaysSortType(2);
			}
		}
		SortHand(sortType);
		ReorganizeHand();
	}

	public void ChangeAlwaysSortType(int sortType)
	{
		alwaysSortType = sortType;
		sortByRankButton.ChangeSpecialState(sortType == 1 ? true : false);
		sortBySuitButton.ChangeSpecialState(sortType == 2 ? true : false);
	}
	
	public void SortHand(int sortType)
	{
		if(sortType != alwaysSortType)
		{
			ChangeAlwaysSortType(0); 
		}
		List<Card> cardsInHand = GetAllCardsInHand(false);
		if(sortType == 1)
		{
			cardsInHand.Sort((x,y) => 
			{
				int rankComparison = x.cardData.rank - y.cardData.rank;
				if(rankComparison != 0)
				{
					return rankComparison;
				}
				else
				{
					// return x.cardData.suit - y.cardData.suit;
					return LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[x.cardData.suit]] - LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[y.cardData.suit]];
				}
			});
		}
		else
		{
			cardsInHand.Sort((x,y) => 
			{
				// int suitComparison = x.cardData.suit - y.cardData.suit;
				int suitComparison = LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[x.cardData.suit]] - LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[y.cardData.suit]];
				if(suitComparison != 0)
				{
					return suitComparison;
				}
				else
				{
					return x.cardData.rank - y.cardData.rank;
				}
			});
		}
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			cardsInHand[i].transform.SetSiblingIndex(i);
		}
	}
	
	public void ReorganizeHand()
	{
		if(alwaysSortType != 0)
		{
			SortHand(alwaysSortType);
		}
		float distanceBetweenCards = 50f;
		int visualCardsInHand = cardParent.childCount;
		if(siblingIndexOfLooseCard >= 0)
		{
			visualCardsInHand++;
		}
		if(visualCardsInHand > 8)
		{
			distanceBetweenCards = 400f / cardParent.childCount;
		}
		for(int i = 0; i < visualCardsInHand; i++)
		{
			if(i != siblingIndexOfLooseCard)
			{
				float xDestination = (visualCardsInHand - 1) * (distanceBetweenCards / 2f) - (visualCardsInHand - i - 1) * distanceBetweenCards; // -200 to 200
				float yDestination = handParabola.Evaluate((xDestination + 200f) / 400f) * 30 - 100;
				Vector2 destination = new Vector2(xDestination, yDestination);
				Vector3 destinationRotation = new Vector3(0, 0, handRotationParabola.Evaluate((xDestination + 200f) / 400f) * 30f);
				cardParent.GetChild((i < siblingIndexOfLooseCard || siblingIndexOfLooseCard < 0) ? i : i - 1).GetComponent<Card>().StartMove(destination, destinationRotation);
			}
		}
		selectedCards.Clear();
		SelectedCardsUpdated();
	}
	
    public Card SpawnCard(CardData cardData, Vector2 spawnLocation, Transform parent, bool spawnFaceDown = true)
	{
		SoundManager.instance.PlayCardPickupSound();
		GameObject newCardGO = Instantiate(cardPrefab, spawnLocation, Quaternion.identity, parent);
		newCardGO.transform.localPosition = new Vector3(newCardGO.transform.localPosition.x, newCardGO.transform.localPosition.y, 0);
		newCardGO.transform.SetSiblingIndex(0);
		newCardGO.name = LocalInterface.instance.ConvertRankAndSuitToString(cardData.rank, cardData.suit);
		Card newCard = newCardGO.GetComponent<Card>();
		newCard.rt.anchoredPosition = spawnLocation;
		newCard.cardData = cardData;
		if(cardData.suit < 4)
		{
			newCard.rankImage.color = LocalInterface.instance.suitColors[cardData.suit];
			newCard.bigSuitImage.color = LocalInterface.instance.suitColors[cardData.suit];
			if(cardData.rank <= 8 || cardData.rank == 12)
			{
				newCard.detailImage.color = LocalInterface.instance.suitColors[cardData.suit];
			}
			newCard.rankImage.sprite = rankSprites[cardData.rank];
		}
		else
		{
			newCard.rankImage.sprite = rankSprites[cardData.rank + 13];
			newCard.rankImageRT.anchoredPosition = newCard.rankImageRT.anchoredPosition;
		}
		newCard.rankImageRT.sizeDelta = new Vector2(rankSprites[cardData.rank].rect.width, rankSprites[cardData.rank].rect.height);
		newCard.bigSuitImage.sprite = bigSuitSprites[cardData.suit];
		newCard.bigSuitImageRT.sizeDelta = new Vector2(bigSuitSprites[cardData.suit].rect.width, bigSuitSprites[cardData.suit].rect.height);
		int cardNumber = cardData.suit * 13 + cardData.rank;
		newCard.detailImage.sprite = cardDetails[cardNumber];
		newCard.detailImageRT.sizeDelta = new Vector2(cardDetails[cardNumber].rect.width, cardDetails[cardNumber].rect.height);
		// newCard.rank = cardData.rank;
		// newCard.suit = cardData.suit;
		// newCard.specialCardName = cardData.specialCardName;
		// newCard.cardBaseValue = new float[4];
		// newCard.cardMultiplier = new float[4];
		/* for(int i = 0; i < 4; i++)
		{
			newCard.cardBaseValue[i] = cardData.baseValue[i];
			newCard.cardMultiplier[i] = cardData.multiplier[i];
		} */
		if(spawnFaceDown)
		{
			newCard.ChangeFaceDown(true);
			newCard.StartCoroutine(newCard.Flip());
		}
		else
		{
			newCard.ChangeFaceDown(false);
		}
		return newCard;
	}
	
	public List<Card> GetAllCardsInHand(bool includeLooseCards = true)
	{
		List<Card> cardsInHand = new List<Card>();
		for(int i = 0; i < cardParent.childCount; i++)
		{
			if(cardParent.GetChild(i).GetComponent<Card>() != null)
			{
				cardsInHand.Add(cardParent.GetChild(i).GetComponent<Card>());
			}
		}
		if(includeLooseCards)
		{
			for(int i = 0; i < looseCardParent.childCount; i++)
			{
				if(looseCardParent.GetChild(i).GetComponent<Card>() != null)
				{
					cardsInHand.Add(looseCardParent.GetChild(i).GetComponent<Card>());
				}
			}
		}
		return cardsInHand;
	}
	
	public void SelectedCardsUpdated()
	{
		if(handLockedIn) 
		{
			return;
		}
		int maxCardsDiscardedAtOnce = GameManager.instance.baseMaxCardsDiscardedAtOnce + Baubles.instance.GetBaubleImpactIntByTag("AllowDiscardingMore");
		if(GameManager.instance.discardsRemaining > 0)
		{
			if(!discardButton.buttonEnabled && selectedCards.Count > 0 && selectedCards.Count <= maxCardsDiscardedAtOnce && !handLockedIn)
			{
				discardButton.ChangeButtonEnabled(true);
			}
			else if((discardButton.buttonEnabled && selectedCards.Count == 0 || selectedCards.Count > maxCardsDiscardedAtOnce) || handLockedIn)
			{
				discardButton.ChangeButtonEnabled(false);
			}
		}
		for(int i = 0; i < Deck.instance.cardsInHand.Count; i++)
		{
			if(!selectedCards.Contains(Deck.instance.cardsInHand[i]))
			{
				if(Deck.instance.cardsInHand[i].xImage.gameObject.activeSelf)
				{
					Deck.instance.cardsInHand[i].xImage.gameObject.SetActive(false);
				}
			}
		}
		int numberOfStandardCardsSelected = GetNumberOfCardsOfTypeInList(selectedCards, false);
		if(selectedCards.Count > 0 && numberOfStandardCardsSelected > 0 && numberOfStandardCardsSelected <= GameManager.instance.baseNumberOfPlayableStandardCards + Baubles.instance.GetBaubleImpactIntByTag("IncreasePlayableStandardCardCount"))
		{
			List<CardData> cardDataOfCardsInHand = new List<CardData>();
			for(int i = 0; i < selectedCards.Count; i++)
			{
				if(!selectedCards[i].cardData.isSpecialCard)
				{
					cardDataOfCardsInHand.Add(selectedCards[i].cardData);
				}
			}
			HandEvaluation.instance.EvaluateHand(cardDataOfCardsInHand, false);
			playSelectedButton.ChangeButtonEnabled(true);
		}
		else
		{
			playSelectedButton.ChangeButtonEnabled(false);
			HandPower.instance.handNameLabel.ChangeText(string.Empty);
			HandPower.instance.ClearHandPowerLabels();
		}
		
		/* int numberOfStandardCardsSelected = GetNumberOfCardsOfTypeInList(selectedCards, false);
		int numberOfSpecialCardsSelected = GetNumberOfCardsOfTypeInList(selectedCards, true);
		int numberOfEmptyStandardDropZones = PlayArea.instance.GetNumberOfEmptyStandardDropZones();
		int numberOfEmptySpecialCardDropZones = PlayArea.instance.GetNumberOfEmptySpecialCardDropZones();
		
		if(selectedCards.Count > 0 && !PlayArea.instance.locked)
		{
			if(Baubles.instance.GetBaubleImpactIntByTag("AllowSpecialCardsInStandardDropZones") == 0)
			{
				if(numberOfStandardCardsSelected <= numberOfEmptyStandardDropZones && numberOfSpecialCardsSelected <= numberOfEmptySpecialCardDropZones)
				{
					playSelectedButton.ChangeButtonEnabled(true);
				}
				else
				{
					playSelectedButton.ChangeButtonEnabled(false);
				}
			}
			else
			{
				if(numberOfStandardCardsSelected <= numberOfEmptyStandardDropZones && numberOfStandardCardsSelected + numberOfSpecialCardsSelected < numberOfEmptyStandardDropZones + numberOfEmptySpecialCardDropZones)
				{
					playSelectedButton.ChangeButtonEnabled(true);
				}
				else
				{
					playSelectedButton.ChangeButtonEnabled(false);
				}
			}
		}
		else
		{
			playSelectedButton.ChangeButtonEnabled(false);
		} */
	}
	
	public int GetNumberOfCardsOfTypeInList(List<Card> cards, bool special)
	{
		int cardsOfType = 0;
		for(int i = 0; i < cards.Count; i++)
		{
			if(cards[i].cardData.isSpecialCard == special)
			{
				cardsOfType++;
			}
		}
		return cardsOfType;
	}
	
	public void SelectAllClicked()
	{
		List<Card> cardsInHand = GetAllCardsInHand();
		int cardsSelected = 0;
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			if(!selectedCards.Contains(cardsInHand[i]))
			{
				selectedCards.Add(cardsInHand[i]);
				cardsInHand[i].rt.anchoredPosition = cardsInHand[i].rt.anchoredPosition + Vector2.up * distanceDifferenceOfSelectedCard;
				cardsSelected++;
			}
		}
		if(cardsSelected > 0)
		{
			SoundManager.instance.PlayCardPickupSound();
		}
		else
		{
			for(int i = 0; i < cardsInHand.Count; i++)
			{
				if(selectedCards.Contains(cardsInHand[i]))
				{
					selectedCards.Remove(cardsInHand[i]);
					cardsInHand[i].rt.anchoredPosition = cardsInHand[i].rt.anchoredPosition - Vector2.up * distanceDifferenceOfSelectedCard;
					cardsSelected++;
				}
			}
			if(cardsSelected > 0)
			{
				SoundManager.instance.PlayCardDropSound();
			}
		}
		SelectedCardsUpdated();
	}
	
	public void RecallClicked()
	{
		for(int i = PlayArea.instance.standardDropZones.Length - 1; i >= 0; i--)
		{
			if(PlayArea.instance.standardDropZones[i].cardPlaced)
			{
				PlayArea.instance.standardDropZones[i].placedCard.dropZonePlacedIn = null;
				PlayArea.instance.standardDropZones[i].placedCard.RevertToOriginalImage();
				PlayArea.instance.standardDropZones[i].placedCard.transform.SetParent(cardParent);
				PlayArea.instance.standardDropZones[i].placedCard.transform.SetSiblingIndex(0);
				PlayArea.instance.standardDropZones[i].CardRemoved();
			}
		}
		for(int i = PlayArea.instance.specialCardDropZones.Length - 1; i >= 0; i--)
		{
			if(PlayArea.instance.specialCardDropZones[i].cardPlaced)
			{
				PlayArea.instance.specialCardDropZones[i].placedCard.dropZonePlacedIn = null;
				PlayArea.instance.specialCardDropZones[i].placedCard.RevertToOriginalImage();
				PlayArea.instance.specialCardDropZones[i].placedCard.transform.SetParent(cardParent);
				PlayArea.instance.specialCardDropZones[i].placedCard.transform.SetSiblingIndex(0);
				PlayArea.instance.specialCardDropZones[i].CardRemoved();
			}
		}
		PlayArea.instance.HandUpdated();
		SoundManager.instance.PlayCardSlideSound();
		recallButton.ChangeButtonEnabled(false);
		ReorganizeHand();
	}
	
	public void PlaySelectedClicked()
	{
		if(CombatArea.instance.combatAreaLocked || selectedCards.Count == 0)
		{
			return;
		}
		handLockedIn = true;
		discardButton.ChangeButtonEnabled(false);
		sortByRankButton.ChangeButtonEnabled(false);
		sortBySuitButton.ChangeButtonEnabled(false);
		playSelectedButton.ChangeButtonEnabled(false);
		List<Card> cardsInHand = GetAllCardsInHand();
		cardsInHand.Sort((x,y) => 
		{
			int xAnchorComparison = Mathf.RoundToInt(x.rt.anchoredPosition.x - y.rt.anchoredPosition.x);
			return xAnchorComparison;
		});
		int selectedCardIndex = 0;
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			cardsInHand[i].canMove = false;
			if(selectedCards.Contains(cardsInHand[i]))
			{
				lockedInHand.Add(cardsInHand[i]);
				cardsInHand[i].rt.SetParent(playedCardsParent);
				float squeezeDistance = (maxWidthOfPlayedHand - LocalInterface.instance.cardSize.x) / (selectedCards.Count - 1);
				float practicalDistanceBetweenCards = Mathf.Min((LocalInterface.instance.cardSize.x + idealDistanceBetweenCardsInPlayedHand), squeezeDistance);
				Vector2 destination = new Vector2((selectedCards.Count - 1) * (practicalDistanceBetweenCards / 2) - (selectedCards.Count - selectedCardIndex - 1) * practicalDistanceBetweenCards, yPositionOfPlayedHand);
				cardsInHand[i].StartMove(destination, Vector3.zero, false);
				selectedCardIndex++;
			}
			else
			{
				cardsInHand[i].StartFade(LocalInterface.instance.animationDuration, 1f, 0.2f);
			}
		}
		ReorganizeHand();
		CombatArea.instance.StartCombatTurn(LocalInterface.instance.animationDuration);
	}
	
	public void DiscardClicked()
	{
		SoundManager.instance.PlayCardSlideSound();
		DiscardCards(selectedCards);
		List<Card> cardsInHandAfterDiscarding = Deck.instance.cardsInHand;
		for(int i = 0; i < selectedCards.Count; i++)
		{
			cardsInHandAfterDiscarding.Remove(selectedCards[i]);
		}
		int numberOfStandardCardsInHand = GetNumberOfCardsOfTypeInList(cardsInHandAfterDiscarding, false);
		selectedCards.Clear();
		SelectedCardsUpdated();
		GameManager.instance.SetDiscardsRemaining(GameManager.instance.discardsRemaining - 1);
		StartDrawCards(GameManager.instance.baseHandSize + Baubles.instance.GetBaubleImpactIntByTag("IncreaseHandSize") - numberOfStandardCardsInHand);
	}
	
	public void DiscardCards(List<Card> cardsToDiscard)
	{
		int numberOfAces = 0;
		List<Vector2> locationsOfAces = new List<Vector2>();
		for(int i = 0; i < cardsToDiscard.Count; i++)
		{
			if(cardsToDiscard[i].cardData.rank == 12 && Baubles.instance.GetBaubleImpactIntByTag("DiscardAcesForMoney") > 0)
			{
				numberOfAces++;
				locationsOfAces.Add(cardsToDiscard[i].rt.anchoredPosition);
			}
			cardsToDiscard[i].transform.SetParent(discardedCardsParent);
			
			cardsToDiscard[i].StartMove(discardPileLocation, Vector3.zero, false, true, true);
			if(!cardsToDiscard[i].faceDown)
			{
				cardsToDiscard[i].StartCoroutine(cardsToDiscard[i].Flip(false));
			}
		}
	}
	
	public void HandEvaluated(List<CardData> cardsUsed, bool evaluatingOnlyCardsUsed, bool[] handsContained, bool isRoyalFlush = false)
	{
		if(cardsUsed != null)
		{
			if(GetNumberOfCardsOfTypeInList(selectedCards, false) > cardsUsed.Count && !evaluatingOnlyCardsUsed && Baubles.instance.GetBaubleImpactIntByTag("UseAllCardsInPlayArea") == 0)
			{
				HandEvaluation.instance.EvaluateHand(cardsUsed, true);
				return;
			}
		}
		for(int i = 0; i < selectedCards.Count; i++)
		{
			if(!selectedCards[i].cardData.isSpecialCard)
			{
				selectedCards[i].xImage.gameObject.SetActive(true);
				for(int j = 0; j < cardsUsed.Count; j++)
				{
					if(selectedCards[i].cardData == cardsUsed[j])
					{
						selectedCards[i].xImage.gameObject.SetActive(false);
						break;
					}
				}
			}
		}
		int handTier = -1;
		for(int i = 17; i >= 0; i--)
		{
			if(handsContained[i])
			{
				handTier = i;
				if(i == 8 && isRoyalFlush)
				{
					HandPower.instance.handNameLabel.ChangeText("Royal Flush");
				}
				else
				{
					HandPower.instance.handNameLabel.ChangeText(LocalInterface.instance.handNames[i]);
				}
				break;
			}
		}
		float[] baseValue = new float[4];
		float[] multiplier = new float[4];
		if(handTier >= 0)
		{
			for(int i = 0; i < cardsUsed.Count; i++)
			{
				for(int j = 0; j < 4; j++)
				{
					baseValue[j] += cardsUsed[i].baseValue[j];
					multiplier[j] += cardsUsed[i].multiplier[j];
				}
			}
			string handTierString = handTier.ToString();
			if(handTier < 10)
			{
				handTierString = $"0{handTier.ToString()}"; 
			}
			for(int i = 0; i < handsContained.Length; i++)
			{
				if(handsContained[i])
				{
					for(int j = 0; j < 4; j++)
					{
						baseValue[j] += GameManager.instance.baseValuesOfHands[i];
						baseValue[j] += Baubles.instance.GetBaubleImpactIntByTag($"Hand{handTierString}Power");
						multiplier[j] += GameManager.instance.baseMultipliersOfHands[i];
						multiplier[j] += Baubles.instance.GetBaubleImpactIntByTag($"Hand{handTierString}Power");
					}
				}
			}
		}
		// Debug.Log($"HandEvaluated playerName = {player.playerName} LocalInterface.instance.fusionInterface.localPlayerRef = {LocalInterface.instance.fusionInterface.localPlayerRef.ToString()}");
		HandPower.instance.UpdateHandPowerLabels(baseValue, multiplier);
		lockedInHandBaseValues = baseValue;
		lockedInHandMultipliers = multiplier;
	}
	
	public void HandOver(bool allEnemiesDefeated)
	{
		GameManager.instance.ModifyCurrentShield(-GameManager.instance.currentShield);
		handLockedIn = false;
		HandPower.instance.ClearHandPowerLabels();
		GameManager.instance.SetDiscardsRemaining(GameManager.instance.baseDiscardsPerHand);
		List<Card> cardsInHandNotLockedIn = new List <Card>();
		for(int i = 0; i < Deck.instance.cardsInHand.Count; i++)
		{
			if(!lockedInHand.Contains(Deck.instance.cardsInHand[i]))
			{
				Deck.instance.cardsInHand[i].StartFade(LocalInterface.instance.animationDuration, 0.2f, 1f);
				cardsInHandNotLockedIn.Add(Deck.instance.cardsInHand[i]);
			}
		}
		List<Card> standardCardsInHand = new List<Card>();
		List<Card> specialCardsInHand = new List<Card>();
		for(int i = 0; i < lockedInHand.Count; i++)
		{
			if(lockedInHand[i].cardData.isSpecialCard)
			{
				specialCardsInHand.Add(lockedInHand[i]);
			}
			else
			{
				standardCardsInHand.Add(lockedInHand[i]);
			}
		}
		lockedInHand.Clear();
		DiscardCards(standardCardsInHand);
		selectedCards.Clear();
		SelectedCardsUpdated();
		sortByRankButton.ChangeButtonEnabled(true);
		sortBySuitButton.ChangeButtonEnabled(true);
		if(allEnemiesDefeated)
		{
			DiscardCards(cardsInHandNotLockedIn);
			ShuffleDiscardPileIntoDrawPile(LocalInterface.instance.animationDuration);
		}
		else
		{
			StartDrawCards(standardCardsInHand.Count);
		}
	}
}
