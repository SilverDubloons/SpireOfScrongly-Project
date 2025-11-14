using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
	public RandomNumbers shuffleRNG;
	public RandomNumbers mapRNG;
	public RandomNumbers rewardsRNG;
	public RandomNumbers enemyActionsRNG;
	public Deck deck;
	public Baubles baubles;
	public int baseNumberOfPlayableStandardCards;
	public int baseNumberOfPlayableSpecialCards;
	public int baseMaxCardsDiscardedAtOnce;
	public int baseDiscardsPerHand;
	public int discardsRemaining;
	public float currentHealth;
	public float currentShield;
	public float startingCurrency;
	public float currentCurrency;
	public string characterName;
	public string deckName;
	
	public int baseHandSize;
	public int baseCardsNeededToMakeAFlush;
	public int baseCardsNeededToMakeAStraight;
	public int baseCardsNeededToMakeAStraightFlush;
	public float[] baseValuesOfHands;
	public float[] baseMultipliersOfHands;
	
	void Awake()
	{
		instance = this;
	}
	
	public void StartNewGame(string deckN, string characterN, int seed)
	{
		deckName = deckN;
		characterName = characterN;
		shuffleRNG.ChangeSeed(seed);
		mapRNG.ChangeSeed(seed);
		rewardsRNG.ChangeSeed(seed);
		enemyActionsRNG.ChangeSeed(seed);
		deck.CreateDeck(deckName);
		baubles.PopulateUnlockedBaubles();
		baubles.ImportBaublesFromSpreadsheet();
		// bsaeNumberOfPlayableStandardCards = 5;
		// bsaeNumberOfPlayableSpecialCards = 1; // 0 if tutorial, then animate 1 entering once player obtains a special card?
		// PlayArea.instance.ResizePlayZone();
		// discardsRemaining = 1;
		SetDiscardsRemaining(baseDiscardsPerHand);
		currentHealth = 300f;
		ModifyCurrentCurrency(startingCurrency);
		HandPower.instance.ClearHandPowerLabels();
	}
	
	public void SetDiscardsRemaining(int newDiscardsRemaining)
	{
		discardsRemaining = newDiscardsRemaining;
		HandArea.instance.discardsRemainingLabel.ChangeText(discardsRemaining.ToString());
	}
	
	public void PlayerTakesDamage(float damageTaken)
	{
		float shieldDamageTaken = Mathf.Min(damageTaken, currentShield);
		currentShield -= shieldDamageTaken;
		float healthDamageTaken = damageTaken - shieldDamageTaken;
		currentHealth -= healthDamageTaken;
		if(CombatArea.instance.playerCharacter != null)
		{
			Character playerCharacter = CombatArea.instance.playerCharacter;
			playerCharacter.currentShield = currentShield;
			playerCharacter.currentHealth = currentHealth;
			playerCharacter.UpdateCharacterStats();
			if(healthDamageTaken > LocalInterface.instance.epsilon)
			{
				playerCharacter.StartAnimation("GetHit");
			}
			if(currentHealth <= LocalInterface.instance.epsilon)
			{
				playerCharacter.StartAnimationAfterDelay("Death", playerCharacter.GetDurationOfAnimation("GetHit"), false, false);
				playerCharacter.healthBackdropRT.gameObject.SetActive(false);
				playerCharacter.shieldBackdropRT.gameObject.SetActive(false);
				playerCharacter.statusBackdropRT.gameObject.SetActive(false);
			}
		}
	}
	
	public void ModifyCurrentHealth(float change)
	{
		currentHealth += change;
		if(CombatArea.instance.playerCharacter != null)
		{
			CombatArea.instance.playerCharacter.currentHealth = currentHealth;
			CombatArea.instance.playerCharacter.UpdateCharacterStats();
		}
	}
	
	public void ModifyCurrentShield(float change)
	{
		currentShield += change;
		if(CombatArea.instance.playerCharacter != null)
		{
			CombatArea.instance.playerCharacter.currentShield = currentShield;
			CombatArea.instance.playerCharacter.UpdateCharacterStats();
		}
	}
	
	public void ModifyCurrentCurrency(float change)
	{
		currentCurrency += change;
		LocalInterface.instance.currencyLabel.ChangeText(LocalInterface.instance.ConvertFloatToString(currentCurrency));
	}
}