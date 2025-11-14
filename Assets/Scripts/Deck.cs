using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
	public List<CardData> drawPile = new List<CardData>();
	public List<Card> cardsInHand = new List<Card>();
	public List<CardData> discardPile = new List<CardData>();
	
	public static Deck instance;
	
	void Awake()
	{
		instance = this;
	}
	
    public class CardData
	{
		public int rank;	// 0 - 12, duece through ace
		public int suit;	// 0 = spade, 1 = club, 2 = heart, 3 = diamond, 4 = rainbow
		public float[] baseValue = new float[4];
		public float[] multiplier = new float[4];
		public bool isSpecialCard;
		public string specialCardName; // if empty, is standard card
		public CardData(int r, int s, string scn = "")
		{
			rank = r;
			suit = s;
			specialCardName = scn;
			if(scn == "")
			{
				isSpecialCard = false;
			}
			else
			{
				isSpecialCard = true;
			}
			baseValue[s] = GetBaseValueByRank(r);
		}
	}
	
	public static float GetBaseValueByRank(int r)
	{
		if(r < 8)
		{
			return r + 2;
		}
		else if(r < 12)
		{
			return 10;
		}
		else if(r == 12)
		{
			return 15;
		}
		else
		{
			Debug.LogError("GetBaseValueByRank called with value > 12");
			return 0;
		}
	}
	
	public void CreateDeck(string deckName = "Standard")
	{
		switch(deckName)
		{
			case "Standard":
				for(int i = 0; i < 52; i++)
				{
					drawPile.Add(new CardData(i % 13, i / 13));
				}
				break;
			default:
				Debug.LogError("CreateDeck called with unsupported deck name");
				break;
		}
		UpdateCardsInDrawPile();
		UpdateCardsInDiscardPile();
		ShuffleDrawPile();
	}
	
	public void ShuffleDrawPile()
	{
		for(int i = 0; i < drawPile.Count; i++)
		{
			int r = GameManager.instance.shuffleRNG.Range(0, i + 1);
			CardData temp = drawPile[i];
			drawPile[i] = drawPile[r];
			drawPile[r] = temp;
		}
	}
	
	public void ShuffleCardIntoDeck(CardData cardData)	// does so to a random location in the deck
	{
		drawPile.Insert(GameManager.instance.shuffleRNG.Range(0, drawPile.Count + 1), cardData);
	}
	
	public void UpdateCardsInDrawPile()
	{
		LocalInterface.instance.drawPileLabel.ChangeText(drawPile.Count.ToString());
		if(drawPile.Count == 0)
		{
			if(LocalInterface.instance.drawPileCardback.gameObject.activeSelf)
			{
				LocalInterface.instance.drawPileCardback.gameObject.SetActive(false);
			}
		}
		else
		{
			if(!LocalInterface.instance.drawPileCardback.gameObject.activeSelf)
			{
				LocalInterface.instance.drawPileCardback.gameObject.SetActive(true);
			}
		}
	}
	
	public void UpdateCardsInDiscardPile()
	{
		LocalInterface.instance.discardPileLabel.ChangeText(discardPile.Count.ToString());
		if(discardPile.Count == 0)
		{
			if(LocalInterface.instance.discardPileCardback.gameObject.activeSelf)
			{
				LocalInterface.instance.discardPileCardback.gameObject.SetActive(false);
			}
		}
		else
		{
			if(!LocalInterface.instance.discardPileCardback.gameObject.activeSelf)
			{
				LocalInterface.instance.discardPileCardback.gameObject.SetActive(true);
			}
		}
	}
}
