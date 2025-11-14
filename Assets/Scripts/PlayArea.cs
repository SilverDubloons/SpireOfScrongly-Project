using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static Deck;

public class PlayArea : MonoBehaviour, IPointerClickHandler
{
    public RectTransform rt;
	public RectTransform specialCardDropZonesRT;
	public RectTransform standardCardDropZonesRT;
	public RectTransform sideCircleShadowRT;
	public DropZone[] standardDropZones;
	public DropZone[] specialCardDropZones;
	public ButtonPlus lockButton;
	public Image lockButtonGraphic;
	public Sprite lockedSprite;
	public Sprite unlockedSprite;
	public Label handNameLabel;
	public bool locked;
	
	public static PlayArea instance;
	
	void Awake()
	{
		instance = this;
	}
	
	public void ResizePlayZone()
	{
		/* int numberOfStandardDropZones = GameManager.instance.baseNumberOfPlayableStandardCards + Baubles.instance.GetBaubleImpactIntByTag("IncreaseStandardCardDropZoneCount");
		int numberOfSpecialDropZones = GameManager.instance.baseNumberOfPlayableSpecialCards + Baubles.instance.GetBaubleImpactIntByTag("IncreaseSpecialCardDropZoneCount");
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(i < numberOfStandardDropZones)
			{
				standardDropZones[i].gameObject.SetActive(true);
			}
			else
			{
				standardDropZones[i].gameObject.SetActive(false);
			}
		}
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(i < numberOfSpecialDropZones)
			{
				specialCardDropZones[i].gameObject.SetActive(true);
			}
			else
			{
				specialCardDropZones[i].gameObject.SetActive(false);
			}
		}
		if(numberOfSpecialDropZones <= 0)
		{
			specialCardDropZonesRT.gameObject.SetActive(false);
		}
		else
		{
			specialCardDropZonesRT.gameObject.SetActive(true);
			specialCardDropZonesRT.sizeDelta = new Vector2(4 + 48 * numberOfSpecialDropZones, specialCardDropZonesRT.sizeDelta.y);
		}
		standardCardDropZonesRT.sizeDelta = new Vector2(4 + 48 * numberOfStandardDropZones, standardCardDropZonesRT.sizeDelta.y);
		sideCircleShadowRT.anchoredPosition = new Vector2(-208 + 24 * numberOfStandardDropZones, sideCircleShadowRT.anchoredPosition.y); */
	}
	
	public int GetNumberOfEmptyStandardDropZones()
	{
		int emptyStandardDropZones = 0;
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && !standardDropZones[i].cardPlaced)
			{
				emptyStandardDropZones++;
			}
		}
		return emptyStandardDropZones;
	}
	
	public int GetNumberOfEmptySpecialCardDropZones()
	{
		int emptySpecialCardDropZones = 0;
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].gameObject.activeSelf && !specialCardDropZones[i].cardPlaced)
			{
				emptySpecialCardDropZones++;
			}
		}
		return emptySpecialCardDropZones;
	}
	
	public void HandUpdated()
	{
		List<Card> cardsInPlayZone = new List<Card>();
		bool standardCardInPlayArea = false;
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].cardPlaced)
			{
				cardsInPlayZone.Add(standardDropZones[i].placedCard);
				if(!standardDropZones[i].placedCard.cardData.isSpecialCard)
				{
					standardCardInPlayArea = true;
				}
			}
		}
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].cardPlaced)
			{
				cardsInPlayZone.Add(specialCardDropZones[i].placedCard);
			}
		}
		if(cardsInPlayZone.Count > 0)
		{
			if(standardCardInPlayArea)
			{
				lockButton.ChangeButtonEnabled(true);
			}
			else
			{
				lockButton.ChangeButtonEnabled(false);
			}
			HandArea.instance.recallButton.ChangeButtonEnabled(true);
			List<CardData> standardCardsInHand = new List<CardData>();
			for(int i = 0; i < cardsInPlayZone.Count; i++)
			{
				if(!cardsInPlayZone[i].cardData.isSpecialCard)
				{
					standardCardsInHand.Add(cardsInPlayZone[i].cardData);
				}
			}
			HandEvaluation.instance.EvaluateHand(standardCardsInHand, false);
		}
		else
		{
			lockButton.ChangeButtonEnabled(false);
			HandArea.instance.recallButton.ChangeButtonEnabled(false);
			handNameLabel.ChangeText(string.Empty);
			HandPower.instance.ClearHandPowerLabels();lockButton.ChangeButtonEnabled(false);
			HandArea.instance.recallButton.ChangeButtonEnabled(false);
			handNameLabel.ChangeText(string.Empty);
			HandPower.instance.ClearHandPowerLabels();
		}
	}
	
	public int GetNumberOfStandardCards()
	{
		int numberOfStandardCards = 0;
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].cardPlaced)
			{
				if(!standardDropZones[i].placedCard.cardData.isSpecialCard)
				{
					numberOfStandardCards++;
				}
			}
		}
		return numberOfStandardCards;
	}
	
	public List<DropZone> GetEmptyDropZones(DropZone[] dropzoneArray)
	{
		List<DropZone> emptyDropZones = new List<DropZone>();
		for(int i = 0; i < dropzoneArray.Length; i++)
		{
			if(!dropzoneArray[i].cardPlaced)
			{
				emptyDropZones.Add(dropzoneArray[i]);
			}
		}
		return emptyDropZones;
	}
	
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		HandArea.instance.PlaySelectedClicked();
	}
	
	public void HandEvaluated(List<CardData> cardsUsed, bool evaluatingOnlyCardsUsed, bool[] handsContained, bool isRoyalFlush = false)
	{
		if(cardsUsed != null)
		{
			if(GetNumberOfStandardCards() > cardsUsed.Count && !evaluatingOnlyCardsUsed && Baubles.instance.GetBaubleImpactIntByTag("UseAllCardsInPlayArea") == 0)
			{
				HandEvaluation.instance.EvaluateHand(cardsUsed, true);
				return;
			}
		}
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			standardDropZones[i].xImage.gameObject.SetActive(true);
			if(standardDropZones[i].cardPlaced)
			{
				if(!standardDropZones[i].placedCard.cardData.isSpecialCard)
				{
					for(int j = 0; j < cardsUsed.Count; j++)
					{
						if(standardDropZones[i].placedCard.cardData == cardsUsed[j])
						{
							standardDropZones[i].xImage.gameObject.SetActive(false);
							break;
						}
					}
				}
				else
				{
					standardDropZones[i].xImage.gameObject.SetActive(false);
				}
			}
			else
			{
				standardDropZones[i].xImage.gameObject.SetActive(false);
			}
		}
		int handTier = -1;
		for(int i = 17; i >= 0; i--)
		{
			if(handsContained[i])
			{
				handTier = i;
				handNameLabel.ChangeText(LocalInterface.instance.handNames[i]);
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
	}
}
