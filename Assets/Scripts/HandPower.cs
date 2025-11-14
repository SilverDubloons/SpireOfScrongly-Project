using UnityEngine;
using static Deck;
using System.Collections.Generic;

public class HandPower : MonoBehaviour
{
    public Label[] baseValueLabels;
	public Label[] multiplierLabels;
	public RectTransform[] suitRTs;
	public Label handNameLabel;
	
	public static HandPower instance;
	
	void Awake()
	{
		instance = this;
	}
	
	public void UpdateHandPowerLabels(float[] baseValues, float[] multipliers)
	{
		for(int i = 0; i < 4; i++)
		{
			if(baseValues[i] < LocalInterface.instance.epsilon || multipliers[i] < LocalInterface.instance.epsilon)
			{
				baseValueLabels[i].ChangeText(string.Empty);
				multiplierLabels[i].ChangeText(string.Empty);
			}
			else
			{
				baseValueLabels[i].ChangeText(LocalInterface.instance.ConvertFloatToString(baseValues[i]));
				multiplierLabels[i].ChangeText(LocalInterface.instance.ConvertFloatToString(multipliers[i]));
			}
		}
	}
	
	public void ClearHandPowerLabels()
	{
		for(int i = 0; i < 4; i++)
		{
			baseValueLabels[i].ChangeText(string.Empty);
			multiplierLabels[i].ChangeText(string.Empty);
		}
	}
	
	public void ResetHandPowerLabelColors()
	{
		for(int i = 0; i < 4; i++)
		{
			baseValueLabels[i].ChangeColor(LocalInterface.instance.defaultTextColor);
			multiplierLabels[i].ChangeColor(LocalInterface.instance.defaultTextColor);
		}
	}
	
	public void RearangeSuitOrder()
	{
		for(int i = 0; i < suitRTs.Length; i++)
		{
			suitRTs[i].anchoredPosition = new Vector2(suitRTs[i].anchoredPosition.x, -13 -20 * LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[i]]);
		}
	}
	
	public void HandEvaluated(List<CardData> cardsUsed, bool evaluatingOnlyCardsUsed, bool[] handsContained, bool isRoyalFlush = false)
	{
		/* if(cardsUsed != null)
		{
			if(GetNumberOfStandardCards() > cardsUsed.Count && !evaluatingOnlyCardsUsed && Baubles.instance.GetBaubleImpactIntByTag("UseAllCardsInPlayArea") == 0)
			{
				HandEvaluation.instance.EvaluateHand(cardsUsed, true);
				return;
			}
		} */
		/* for(int i = 0; i < standardDropZones.Length; i++)
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

		HandPower.instance.UpdateHandPowerLabels(baseValue, multiplier);*/
	} 
}
