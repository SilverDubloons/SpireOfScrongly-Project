using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static Deck;

public class HandEvaluation : MonoBehaviour
{	
	public static HandEvaluation instance;
	
	void Awake()
	{
		instance = this;
	}

    public void EvaluateHand(List<CardData> hand, bool evaluatingOnlyCardsUsed)
	{
		hand.Sort((x, y) =>
		{
			int rankSort = x.rank.CompareTo(y.rank);
			if(rankSort == 0)
			{
				return x.suit.CompareTo(y.suit);
			}
			else
			{
				return rankSort;
			}
		});
		int[] numberOfEachSuit = new int[5];
		for(int i = 0; i < hand.Count; i++)
		{
			numberOfEachSuit[hand[i].suit]++;
		}
		bool sevenOfAKind = false;
		bool hugeHouse = false;
		bool wideHouse = false;
		bool guestHouse = false;
		if(hand.Count >= 7)
		{
			if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
			{
				sevenOfAKind = true;
			}
			else
			{
				if((hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank != hand[5].rank && hand[5].rank == hand[6].rank) || (hand[0].rank == hand[1].rank && hand[1].rank != hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank))
				{
					hugeHouse = true;
				}
				else
				{
					if((hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank != hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank) || (hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank != hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank))
					{
						wideHouse = true;
					}
					else
					{
						if((hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank != hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank != hand[5].rank && hand[5].rank == hand[6].rank) || (hand[0].rank == hand[1].rank && hand[1].rank != hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank != hand[5].rank && hand[5].rank == hand[6].rank) || (hand[0].rank == hand[1].rank && hand[1].rank != hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank != hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank))
						{
							guestHouse = true;
						}
					}
				}
			}
		}
		bool sixOfAKind = false;
		bool stuffedHouse = false;
		bool doubleTriple = false;
		bool tripleDouble = false;
		
		List<CardData> sixHand = new List<CardData>();
		
		if(hand.Count >= 6)
		{
			if(sevenOfAKind)
			{
				sixOfAKind = true;
			}
			else if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank)
			{
				sixOfAKind = true;
				for(int i = 0; i < 6; i++)
				{
					sixHand.Add(hand[i]);
				}
			}
			else
			{
				if(hand.Count >= 7)
				{
					if(hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
					{
						sixOfAKind = true;
						for(int i = 1; i < 7; i++)
						{
							sixHand.Add(hand[i]);
						}
					}
				}
				if(!sixOfAKind)
				{
					if(hugeHouse || wideHouse || guestHouse)
					{
						if(hugeHouse || wideHouse)
						{
							stuffedHouse = true;
						}
						if(wideHouse)
						{
							doubleTriple = true;
						}
						if(guestHouse)
						{
							tripleDouble = true;
						}
					}
					else
					{
						if((hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank != hand[4].rank && hand[4].rank == hand[5].rank) || (hand[0].rank == hand[1].rank && hand[1].rank != hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank))
						{
							stuffedHouse = true;
							for(int i = 0; i < 6; i++)
							{
								sixHand.Add(hand[i]);
							}
						}
						else if(hand.Count >= 7)
						{
							if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[5].rank == hand[6].rank)
							{
								stuffedHouse = true;
								for(int i = 0; i < 7; i++)
								{
									if(i != 4)
									{
										sixHand.Add(hand[i]);
									}
								}
							}
							else if(hand[0].rank == hand[1].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
							{
								stuffedHouse = true;
								for(int i = 0; i < 7; i++)
								{
									if(i != 2)
									{
										sixHand.Add(hand[i]);
									}
								}
							}
							else if((hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[5].rank == hand[6].rank) || (hand[1].rank == hand[2].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank))
							{
								stuffedHouse = true;
								for(int i = 1; i < 7; i++)
								{
									sixHand.Add(hand[i]);
								}
							}
						}
						if(!stuffedHouse)
						{
							if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank)
							{
								doubleTriple = true;
								for(int i = 0; i < 6; i++)
								{
									sixHand.Add(hand[i]);
								}
							}
							else
							{
								if(hand.Count >= 7)
								{
									if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
									{
										doubleTriple = true;
										for(int i = 0; i < 7; i++)
										{
											if(i != 3)
											{
												sixHand.Add(hand[i]);
											}
										}
									}
									else if(hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
									{
										doubleTriple = true;
										for(int i = 1; i < 7; i++)
										{
											sixHand.Add(hand[i]);
										}
									}
								}
							}
							if(!doubleTriple)
							{
								if(hand[0].rank == hand[1].rank && hand[2].rank == hand[3].rank && hand[4].rank == hand[5].rank)
								{
									tripleDouble = true;
									for(int i = 0; i < 6; i++)
									{
										sixHand.Add(hand[i]);
									}
								}
								else
								{
									if(hand.Count >= 7)
									{
										if(hand[0].rank == hand[1].rank && hand[2].rank == hand[3].rank && hand[5].rank == hand[6].rank)
										{
											tripleDouble = true;
											for(int i = 0; i < 7; i++)
											{
												if(i != 4)
												{
													sixHand.Add(hand[i]);
												}
											}
										}
										else if(hand[0].rank == hand[1].rank && hand[3].rank == hand[4].rank && hand[5].rank == hand[6].rank)
										{
											tripleDouble = true;
											for(int i = 0; i < 7; i++)
											{
												if(i != 2)
												{
													sixHand.Add(hand[i]);
												}
											}
										}
										else if(hand[1].rank == hand[2].rank && hand[3].rank == hand[4].rank && hand[5].rank == hand[6].rank)
										{
											tripleDouble = true;
											for(int i = 1; i < 7; i++)
											{
												sixHand.Add(hand[i]);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		bool fiveOfAKind = false;
		List<CardData> fiveOfAKindCards = new List<CardData>();
		if(hand.Count >= 5)
		{
			if(sixOfAKind)
			{
				fiveOfAKind = true;
			}
			else if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank)
			{
				fiveOfAKind = true;
				for(int i = 0; i < 5; i++)
				{
					fiveOfAKindCards.Add(hand[i]);
				}
				/* if(HandIsFlush(fiveOfAKindCards, 5))
				{
					flushFive = true;
				} */
			}
			else if(hand.Count >= 6)
			{
				if(hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank)
				{
					fiveOfAKind = true;
					for(int i = 1; i < 6; i++)
					{
						fiveOfAKindCards.Add(hand[i]);
					}
				}
				else if(hand.Count >= 7)
				{
					if(hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
					{
						fiveOfAKind = true;
						for(int i = 2; i < 7; i++)
						{
							fiveOfAKindCards.Add(hand[i]);
						}
					}
				}
			}
		}
		bool fourOfAKind = false;
		List<CardData> fourOfAKindCards = new List<CardData>();
		if(hand.Count >= 4)
		{
			if(fiveOfAKind)
			{
				fourOfAKind = true;
			}
			else if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank)
			{
				fourOfAKind = true;
				for(int i = 0; i < 4; i++)
				{
					fourOfAKindCards.Add(hand[i]);
				}
			}
			else if(hand.Count >= 5)
			{
				if(hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank)
				{
					fourOfAKind = true;
					for(int i = 1; i < 5; i++)
					{
						fourOfAKindCards.Add(hand[i]);
					}
				}
				else if(hand.Count >= 6)
				{
					if(hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank)
					{
						fourOfAKind = true;
						for(int i = 2; i < 6; i++)
						{
							fourOfAKindCards.Add(hand[i]);
						}
					}
					else if(hand.Count >= 7)
					{
						if(hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
						{
							fourOfAKind = true;
							for(int i = 3; i < 7; i++)
							{
								fourOfAKindCards.Add(hand[i]);
							}
						}
					}
				}
			}
		}
		bool fullHouse = false;
		List<CardData> fullHouseCards = new List<CardData>();
		if(hand.Count >= 5)
		{
			if(hugeHouse || wideHouse || guestHouse || stuffedHouse || doubleTriple)
			{
				fullHouse = true;
			}
			else if(!fourOfAKind) 	// if you have four of a kind (or five of a kind, etc, which means you have four of a kind),
			{						// then you can't have a full house without also having one of the bigger housees
				if((hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[3].rank == hand[4].rank) || (hand[0].rank == hand[1].rank && hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank))
				{
					fullHouse = true;
					for(int i = 0; i < 5; i++)
					{
						fullHouseCards.Add(hand[i]);
					}
				}
				else if(hand.Count >= 6)
				{
					if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[4].rank == hand[5].rank)
					{
						fullHouse = true;
						for(int i = 0; i < 6; i++)
						{
							if(i != 3)
							{
								fullHouseCards.Add(hand[i]);
							}
						}
					}
					else if(hand[0].rank == hand[1].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank)
					{
						fullHouse = true;
						for(int i = 0; i < 6; i++)
						{
							if(i != 2)
							{
								fullHouseCards.Add(hand[i]);
							}
						}
					}
					else if((hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[4].rank == hand[5].rank) || (hand[1].rank == hand[2].rank && hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank))
					{
						fullHouse = true;
						for(int i = 1; i < 6; i++)
						{
							fullHouseCards.Add(hand[i]);
						}
					}
					else if(hand.Count >= 7)
					{
						if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank && hand[5].rank == hand[6].rank)
						{
							fullHouse = true;
							for(int i = 0; i < 7; i++)
							{
								if(i != 3 && i != 4)
								{
									fullHouseCards.Add(hand[i]);
								}
							}
						}
						else if(hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank && hand[5].rank == hand[6].rank)
						{
							fullHouse = true;
							for(int i = 1; i < 7; i++)
							{
								if(i != 4)
								{
									fullHouseCards.Add(hand[i]);
								}
							}
						}
						else if((hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank && hand[5].rank == hand[6].rank) || (hand[2].rank == hand[3].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank))
						{
							fullHouse = true;
							for(int i = 2; i < 7; i++)
							{
								fullHouseCards.Add(hand[i]);
							}
						}
						else if(hand[0].rank == hand[1].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
						{
							fullHouse = true;
							for(int i = 0; i < 7; i++)
							{
								if(i != 2 && i != 3)
								{
									fullHouseCards.Add(hand[i]);
								}
							}
						}
						else if(hand[1].rank == hand[2].rank && hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
						{
							fullHouse = true;
							for(int i = 1; i < 7; i++)
							{
								if(i != 3)
								{
									fullHouseCards.Add(hand[i]);
								}
							}
						}
					}
				}
			}
		}
		bool flush = false;
		bool straightFlush = false;
		bool royalFlush = false;
		List<CardData> flushCards = new List<CardData>();
		List<CardData> straightFlushCards = new List<CardData>();
		if(hand.Count >= GameManager.instance.baseCardsNeededToMakeAFlush - Baubles.instance.GetBaubleImpactIntByTag("DecreaseCardsNeededForFlush") || hand.Count >= GameManager.instance.baseCardsNeededToMakeAStraightFlush - Baubles.instance.GetBaubleImpactIntByTag("DecreaseCardsNeededForStraightFlush"))
		{
			List<int> foundFlushes = new List<int>();
			List<int> foundPossibleStraightFlushes = new List<int>();
			for(int i = 0; i < 4; i++)
			{
				if(numberOfEachSuit[i] + numberOfEachSuit[4] >= GameManager.instance.baseCardsNeededToMakeAFlush - Baubles.instance.GetBaubleImpactIntByTag("DecreaseCardsNeededForFlush"))
				{
					flush = true;
					foundFlushes.Add(i);
				}
				if(numberOfEachSuit[i] + numberOfEachSuit[4] >= GameManager.instance.baseCardsNeededToMakeAStraightFlush - Baubles.instance.GetBaubleImpactIntByTag("DecreaseCardsNeededForStraightFlush"))
				{
					foundPossibleStraightFlushes.Add(i);
				}
			}
			if(flush)
			{
				List<List<CardData>> flushes = new List<List<CardData>>();
				for(int i = 0; i < foundFlushes.Count; i++)
				{
					List<CardData> flushHand = GetAllCardsOfSuit(hand, foundFlushes[i], false, true);
					flushes.Add(flushHand);
				}
				if(flushes.Count >= 1)
				{
					flushes.Sort(new HighCardComparer());
					flushCards = flushes[0];
				}
			}
			List<List<CardData>> straightFlushes = new List<List<CardData>>();
			for(int i = 0; i < foundPossibleStraightFlushes.Count; i++)
			{
				List<CardData> flushToCheck = new List<CardData>();
				for(int j = 0; j < hand.Count; j++)
				{
					if(hand[j].suit == foundPossibleStraightFlushes[i] || hand[j].suit == 4)
					{
						flushToCheck.Add(hand[j]);
					}
				}
				List<CardData> straightFlushCardsToCheck = DoesHandContainStraight(flushToCheck, Baubles.instance.GetBaubleImpactIntByTag("IncreaseMaxGapInStraights"), GameManager.instance.baseCardsNeededToMakeAStraightFlush - Baubles.instance.GetBaubleImpactIntByTag("DecreaseCardsNeededForStraightFlush"), Baubles.instance.GetBaubleImpactIntByTag("StraightsCanWrap") > 0 ? true : false);
				if(straightFlushCardsToCheck != null)
				{
					straightFlush = true;
					straightFlushes.Add(straightFlushCardsToCheck);
				}
			}
			if(straightFlush)
			{
				straightFlushes.Sort(new StraightComparer());
				if(straightFlushes[0][straightFlushes[0].Count - 1].rank == 12)
				{
					royalFlush = true;
				}
				straightFlushCards = straightFlushes[0];
			}
		}
		bool straight = false;
		List<CardData> straightCards = DoesHandContainStraight(hand, Baubles.instance.GetBaubleImpactIntByTag("IncreaseMaxGapInStraights"), GameManager.instance.baseCardsNeededToMakeAStraight - Baubles.instance.GetBaubleImpactIntByTag("DecreaseCardsNeededForStraight"), Baubles.instance.GetBaubleImpactIntByTag("StraightsCanWrap") > 0 ? true : false);
		if(straightCards != null)
		{
			straight = true;
		}
		bool threeOfAKind = false;
		List<CardData> threeOfAKindCards = new List<CardData>();
		if(hand.Count >= 3)
		{
			if(fourOfAKind)
			{
				threeOfAKind = true;
			}
			else if(hand[0].rank == hand[1].rank && hand[1].rank == hand[2].rank)
			{
				threeOfAKind = true;
				for(int i = 0; i < 3; i++)
				{
					threeOfAKindCards.Add(hand[i]);
				}
			}
			else if(hand.Count >= 4)
			{
				if(hand[1].rank == hand[2].rank && hand[2].rank == hand[3].rank)
				{
					threeOfAKind = true;
					for(int i = 1; i < 4; i++)
					{
						threeOfAKindCards.Add(hand[i]);
					}
				}
				else if(hand.Count >= 5)
				{
					if(hand[2].rank == hand[3].rank && hand[3].rank == hand[4].rank)
					{
						threeOfAKind = true;
						for(int i = 2; i < 5; i++)
						{
							threeOfAKindCards.Add(hand[i]);
						}
					}
					else if(hand.Count >= 6)
					{
						if(hand[3].rank == hand[4].rank && hand[4].rank == hand[5].rank)
						{
							threeOfAKind = true;
							for(int i = 3; i < 6; i++)
							{
								threeOfAKindCards.Add(hand[i]);
							}
						}
						else if(hand.Count >= 7)
						{
							if(hand[4].rank == hand[5].rank && hand[5].rank == hand[6].rank)
							{
								threeOfAKind = true;
								for(int i = 4; i < 7; i++)
								{
									threeOfAKindCards.Add(hand[i]);
								}
							}
						}
					}
				}
			}
		}
		
		bool twoPair = false;
		List<CardData> twoPairCards = new List<CardData>();
		if(hand.Count >= 4)
		{
			if(fullHouse || tripleDouble || doubleTriple || stuffedHouse || guestHouse || wideHouse || hugeHouse)
			{
				twoPair = true;
			}
			else if(!threeOfAKind)	// if you have three+ of a kind, then you can't have two pair without having one of the
			{						// bigger houses
				if(hand[0].rank == hand[1].rank && hand[2].rank == hand[3].rank)
				{
					twoPair = true;
					for(int i = 0; i < 4; i++)
					{
						twoPairCards.Add(hand[i]);
					}
				}
				else if(hand.Count >= 5)
				{
					if(hand[0].rank == hand[1].rank && hand[3].rank == hand[4].rank)
					{
						twoPair = true;
						for(int i = 0; i < 5; i++)
						{
							if(i != 2)
							{
								twoPairCards.Add(hand[i]);
							}
						}
					}
					else if(hand[1].rank == hand[2].rank && hand[3].rank == hand[4].rank)
					{
						twoPair = true;
						for(int i = 1; i < 5; i++)
						{
							twoPairCards.Add(hand[i]);
						}
					}
					else if(hand.Count >= 6)
					{
						if(hand[0].rank == hand[1].rank && hand[4].rank == hand[5].rank)
						{
							twoPair = true;
							for(int i = 0; i < 6; i++)
							{
								if(i != 2 && i != 3)
								{
									twoPairCards.Add(hand[i]);
								}
							}
						}
						else if(hand[1].rank == hand[2].rank && hand[4].rank == hand[5].rank)
						{
							twoPair = true;
							for(int i = 1; i < 6; i++)
							{
								if(i != 3)
								{
									twoPairCards.Add(hand[i]);
								}
							}
						}
						else if(hand[2].rank == hand[3].rank && hand[4].rank == hand[5].rank)
						{
							twoPair = true;
							for(int i = 2; i < 6; i++)
							{
								twoPairCards.Add(hand[i]);
							}
						}
						else if(hand.Count >= 7)
						{
							if(hand[0].rank == hand[1].rank && hand[5].rank == hand[6].rank)
							{
								twoPair = true;
								for(int i = 0; i < 7; i++)
								{
									if(i != 2 && i != 3 && i != 4)
									{
										twoPairCards.Add(hand[i]);
									}
								}
							}
							else if(hand[1].rank == hand[2].rank && hand[5].rank == hand[6].rank)
							{
								twoPair = true;
								for(int i = 1; i < 7; i++)
								{
									if(i != 3 && i != 4)
									{
										twoPairCards.Add(hand[i]);
									}
								}
							}
							else if(hand[2].rank == hand[3].rank && hand[5].rank == hand[6].rank)
							{
								twoPair = true;
								for(int i = 2; i < 7; i++)
								{
									if(i != 4)
									{
										twoPairCards.Add(hand[i]);
									}
								}
							}
							else if(hand[3].rank == hand[4].rank && hand[5].rank == hand[6].rank)
							{
								twoPair = true;
								for(int i = 3; i < 7; i++)
								{
									twoPairCards.Add(hand[i]);
								}
							}
						}
					}
				}
			}
		}
		bool onePair = false;
		List<CardData> onePairCards = new List<CardData>();
		if(hand.Count >= 2)
		{
			if(twoPair || threeOfAKind || fullHouse || tripleDouble || doubleTriple || stuffedHouse || guestHouse || wideHouse || hugeHouse)
			{
				onePair = true;
			}
			else
			{
				if(hand[0].rank == hand[1].rank)
				{
					onePair = true;
					onePairCards.Add(hand[0]);
					onePairCards.Add(hand[1]);
				}
				else if(hand.Count >= 3)
				{
					if(hand[1].rank == hand[2].rank)
					{
						onePair = true;
						onePairCards.Add(hand[1]);
						onePairCards.Add(hand[2]);
					}
					else if(hand.Count >= 4)
					{
						if(hand[2].rank == hand[3].rank)
						{
							onePair = true;
							onePairCards.Add(hand[2]);
							onePairCards.Add(hand[3]);
						}
						else if(hand.Count >= 5)
						{
							if(hand[3].rank == hand[4].rank)
							{
								onePair = true;
								onePairCards.Add(hand[3]);
								onePairCards.Add(hand[4]);
							}
							else if(hand.Count >= 6)
							{
								if(hand[4].rank == hand[5].rank)
								{
									onePair = true;
									onePairCards.Add(hand[4]);
									onePairCards.Add(hand[5]);
								}
								else if(hand.Count >= 7)
								{
									if(hand[5].rank == hand[6].rank)
									{
										onePair = true;
										onePairCards.Add(hand[5]);
										onePairCards.Add(hand[6]);
									}
								}
							}
						}
					}
				}
			}
		}
		bool highCard = false;
		List<CardData> highCardCards = new List<CardData>();
		if(hand.Count >= 1)
		{
			highCard = true;
			highCardCards.Add(hand[hand.Count - 1]);
		}
		bool[] handsContained = new bool[18];
		handsContained[0] = highCard;
		handsContained[1] = onePair;
		handsContained[2] = twoPair;
		handsContained[3] = threeOfAKind;
		handsContained[4] = straight;
		handsContained[5] = flush;
		handsContained[6] = fullHouse;
		handsContained[7] = fourOfAKind;
		handsContained[8] = straightFlush;
		handsContained[9] = fiveOfAKind;
		handsContained[10] = tripleDouble;
		handsContained[11] = doubleTriple;
		handsContained[12] = stuffedHouse;
		handsContained[13] = sixOfAKind;
		handsContained[14] = guestHouse;
		handsContained[15] = wideHouse;
		handsContained[16] = hugeHouse;
		handsContained[17] = sevenOfAKind;
		
		for(int i = 17; i >= 0; i--)
		{
			if(handsContained[i])
			{
				if(i >= 14)
				{
					HandArea.instance.HandEvaluated(hand, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i >= 10)
				{
					HandArea.instance.HandEvaluated(sixHand, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 9)
				{
					HandArea.instance.HandEvaluated(fiveOfAKindCards, evaluatingOnlyCardsUsed, handsContained);
				}
				if(i == 8)
				{
					if(royalFlush)
					{
						HandArea.instance.HandEvaluated(straightFlushCards, evaluatingOnlyCardsUsed, handsContained, true);
					}
					else
					{
						HandArea.instance.HandEvaluated(straightFlushCards, evaluatingOnlyCardsUsed, handsContained);
					}
				}
				else if(i == 7)
				{
					HandArea.instance.HandEvaluated(fourOfAKindCards, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 6)
				{
					HandArea.instance.HandEvaluated(fullHouseCards, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 5)
				{
					HandArea.instance.HandEvaluated(flushCards, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 4)
				{
					HandArea.instance.HandEvaluated(straightCards, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 3)
				{
					HandArea.instance.HandEvaluated(threeOfAKindCards, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 2)
				{
					HandArea.instance.HandEvaluated(twoPairCards, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 1)
				{
					HandArea.instance.HandEvaluated(onePairCards, evaluatingOnlyCardsUsed, handsContained);
				}
				else if(i == 0)
				{
					HandArea.instance.HandEvaluated(highCardCards, evaluatingOnlyCardsUsed, handsContained);
				}
				return;
			}
		}
		Debug.LogWarning("EvaluateHand reached endpoint with no hand evaluated");
	}
	
	public class HighCardComparer : IComparer<List<CardData>>
	{
		public int Compare(List<CardData> hand1, List<CardData> hand2)
		{
			int rank1 = hand1.Count > 0 ? hand1[0].rank : -1;
			int rank2 = hand2.Count > 0 ? hand2[0].rank : -1;
			
			if(rank1 != rank2)
			{
				return rank2.CompareTo(rank1);
			}
			
			for(int i = 1; i < Mathf.Min(hand1.Count, hand2.Count); i++)
			{
				rank1 = hand1[i].rank;
				rank2 = hand2[i].rank;
				if(rank1 != rank2)
				{
					return rank2.CompareTo(rank1);
				}
			}
			return hand2.Count.CompareTo(hand1.Count);
		}
	}
	
	public List<CardData> GetAllCardsOfSuit(List<CardData> hand, int suit, bool uniqueRanksOnly, bool highestFirst)
	{
		List<CardData> cardsOfDesiredSuit = new List<CardData>();
		for(int i = 0; i < hand.Count; i++)
		{
			if(hand[i].suit == suit || hand[i].suit == 4)
			{
				if(uniqueRanksOnly)
				{
					bool rankIsUnique = true;
					for(int j = 0; j < cardsOfDesiredSuit.Count; j++)
					{
						if(cardsOfDesiredSuit[j].rank == hand[i].rank)
						{
							rankIsUnique = false;
							break;
						}
					}
					if(rankIsUnique)
					{
						cardsOfDesiredSuit.Add(hand[i]);
					}
				}
				else
				{
					cardsOfDesiredSuit.Add(hand[i]);
				}
			}
		}
		if(highestFirst)
		{
			cardsOfDesiredSuit.Sort((a, b) => b.rank.CompareTo(a.rank));
		}
		else
		{
			cardsOfDesiredSuit.Sort((a, b) => a.rank.CompareTo(b.rank));
		}
		return cardsOfDesiredSuit;
	}
	
	public class StraightComparer : IComparer<List<CardData>>
	{
		public int Compare(List<CardData> straight1, List<CardData> straight2)
		{
			int highCard1 = straight1.Count > 0 ? straight1[straight1.Count - 1].rank : -1;
			int highCard2 = straight2.Count > 0 ? straight2[straight2.Count - 1].rank : -1;
			if(highCard1 != highCard2)
			{
				return highCard2.CompareTo(highCard1);
			}
			int i = -2;
			while(straight1.Count + i >= 0 && straight2.Count + i >= 0)
			{
				highCard1 = straight1[straight1.Count + i].rank;
				highCard2 = straight2[straight2.Count + i].rank;
				if(highCard1 != highCard2)
				{
					return highCard2.CompareTo(highCard1);
				}
				i--;
			}
			return straight2.Count.CompareTo(straight1.Count);
		}
	}
	
	public List<CardData> DoesHandContainStraight(List<CardData> hand, int maxGaps, int minLength, bool canWrap)
	{
		if(hand.Count >= minLength)
		{
			List<CardData> uniqueRanks = new List<CardData>();
			for(int card = 0; card < hand.Count; card++)
			{
				bool addToUniqueRanks = true;
				for(int uniqueRank = 0; uniqueRank < uniqueRanks.Count; uniqueRank++)
				{
					if(uniqueRanks[uniqueRank].rank == hand[card].rank)
					{
						addToUniqueRanks = false;
						break;
					}
				}
				if(addToUniqueRanks)
				{
					uniqueRanks.Add(hand[card]);
				}
			}
			int uniqueRanksInHand = uniqueRanks.Count;
			if(uniqueRanksInHand >= minLength)
			{
				List<CardData> cardsUsedInStraight = new List<CardData>();
				List<List<CardData>> straights = new List<List<CardData>>();
				if(minLength <= 1)
				{
					List<CardData> oneCardStraight = new List<CardData>();
					oneCardStraight.Add(hand[hand.Count - 1]);
					straights.Add(oneCardStraight);
				}
				int cardsInARow = 1;
				if(uniqueRanks[uniqueRanksInHand - 1].rank == 12 && !canWrap) // if there is an ace in hand
				{
					if(uniqueRanks[0].rank <= maxGaps)
					{
						cardsInARow++;
						cardsUsedInStraight.Add(uniqueRanks[uniqueRanksInHand - 1]);
						cardsUsedInStraight.Add(uniqueRanks[0]);
					}
				}
				for(int card = 0; card < uniqueRanksInHand - 1; card++)
				{
					if(uniqueRanks[card + 1].rank - uniqueRanks[card].rank <= 1 + maxGaps && uniqueRanks[card + 1].rank - uniqueRanks[card].rank > 0)
					{
						cardsInARow++;
						if(!cardsUsedInStraight.Contains(uniqueRanks[card]))
						{
							cardsUsedInStraight.Add(uniqueRanks[card]);
						}
						cardsUsedInStraight.Add(uniqueRanks[card + 1]);
					}
					else
					{
						if(cardsUsedInStraight.Count >= minLength)
						{
							straights.Add(cardsUsedInStraight.ToList());
						}
						cardsInARow = 1;
						cardsUsedInStraight.Clear();
					}
				}
				if(cardsUsedInStraight.Count >= minLength)	//		this here's the bit that makes it so wraps suck, but it is logical. Best straight  
				{											//		is best straight. Like with JQKA2 and straight length of 4, JQKA is an ace high
					straights.Add(cardsUsedInStraight.ToList());//	straight whereas JQKA2 is a 2 high straight, according to prescedent set by wheel.
				}
				if(canWrap && cardsInARow < uniqueRanksInHand)
				{
					if(uniqueRanks[uniqueRanksInHand - 1].rank - uniqueRanks[0].rank >= (12 - maxGaps))
					{
						cardsInARow++;
						if(!cardsUsedInStraight.Contains(uniqueRanks[uniqueRanksInHand - 1]))
						{
							cardsUsedInStraight.Add(uniqueRanks[uniqueRanksInHand - 1]);
						}
						if(!cardsUsedInStraight.Contains(uniqueRanks[0]))
						{
							cardsUsedInStraight.Add(uniqueRanks[0]);
						}
					}
					else
					{
						if(cardsUsedInStraight.Count >= minLength)
						{
							straights.Add(cardsUsedInStraight.ToList());
						}
						cardsInARow = 1;
						cardsUsedInStraight.Clear();
					}
					for(int card = 0; card < uniqueRanksInHand - 2; card++)
					{
						if(uniqueRanks[card + 1].rank - uniqueRanks[card].rank <= 1 + maxGaps && uniqueRanks[card + 1].rank - uniqueRanks[card].rank > 0)
						{
							cardsInARow++;
							if(!cardsUsedInStraight.Contains(uniqueRanks[card]))
							{
								cardsUsedInStraight.Add(uniqueRanks[card]);
							}
							if(!cardsUsedInStraight.Contains(uniqueRanks[card + 1]))
							{
								cardsUsedInStraight.Add(uniqueRanks[card + 1]);
							}
						}
						else
						{
							if(cardsUsedInStraight.Count >= minLength)
							{
								straights.Add(cardsUsedInStraight.ToList());
							}
							cardsInARow = 1;
							cardsUsedInStraight.Clear();
						}
					}
					if(cardsUsedInStraight.Count >= minLength)
					{
						straights.Add(cardsUsedInStraight.ToList());
					}
				}
				else
				{
					if(cardsUsedInStraight.Count >= minLength)
					{
						straights.Add(cardsUsedInStraight.ToList());
					}
				}
				if(straights.Count > 0)
				{
					straights.Sort(new StraightComparer());
					return straights[0];
				}
			}
			else
			{
				return null;
			}
		}
		else
		{
			return null;
		}
		return null;
	}
	
	public List<CardData> RemoveOneListOfCardsFromAnother(List<CardData> baseList, List<CardData> cardsToRemove)
	{
		List<CardData> cardsRemaining = baseList;
		for(int i = 0; i < cardsToRemove.Count; i++)
		{
			cardsRemaining.Remove(cardsToRemove[i]);
		}
		return cardsRemaining;
	}
}
