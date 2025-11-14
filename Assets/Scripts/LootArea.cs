using UnityEngine;
using System.Collections.Generic;

public class LootArea : MonoBehaviour
{
	public static LootArea instance;
	public GameObject lootButtonPrefab;
	public Transform lootButtonParent;
    public ButtonPlus skipButton;
	public Vector2 lootButtonDimensions;
	public float distanceBetweenLootButtons;
	public float maxWidthOfLootButtons;
	public LootType[] lootTypesArray;
	public Dictionary<string, LootType> lootTypes = new Dictionary<string, LootType>();
	
	public int baseNumberOfBaubleChancesFromStandardFights;
	public float baseChanceForBaubleFromStandardFights;
	public int baseNumberOfBaubleChancesFromEliteFights;
	public float baseChanceForBaubleFromEliteFights;
	public int baseNumberOfBaubleChancesFromBossFights;
	public float baseChanceForBaubleFromBossFights;
	
	[System.Serializable]
	public struct LootType
	{
		public string lootType;
		public Sprite lootSprite;
		public Color lootSpriteColor;
		public int baseNumberOfChoices;
	}
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		for(int i = 0; i < lootTypesArray.Length; i++)
		{
			lootTypes.Add(lootTypesArray[i].lootType, lootTypesArray[i]);
		}
	}
	
	public void SetupLoot(Dictionary<string, float> lootDictionary)
	{
		int numberOfObjects = 0;
		foreach(KeyValuePair<string, float> entry in lootDictionary)
		{
			switch(entry.Key)
			{
				case "Card":
				case "StandardCard":
				case "SpecialCard":
				case "RandomBauble":
				case "CommonBauble":
				case "UncommonBauble":
				case "RareBauble":
				case "LegendaryBauble":
					numberOfObjects += Mathf.RoundToInt(entry.Value);
					break;
				default:
					numberOfObjects++;
					break;
			}
		}
		int index = 0;
		float squeezeDistance = (maxWidthOfLootButtons - lootButtonDimensions.x) / (numberOfObjects - 1);
		float distanceBetweenButtons = Mathf.Min((lootButtonDimensions.x +distanceBetweenLootButtons), squeezeDistance);
		foreach(KeyValuePair<string, float> entry in lootDictionary)
		{
			switch(entry.Key)
			{
				case "Card":
				case "StandardCard":
				case "SpecialCard":
					for(int i = 0; i < Mathf.RoundToInt(entry.Value); i++)
					{
						CreateLootButton(entry.Key, distanceBetweenButtons, index, numberOfObjects);
						index++;
					}
					break;
				case "RandomBauble":
				case "CommonBauble":
				case "UncommonBauble":
				case "RareBauble":
				case "LegendaryBauble":
					for(int i = 0; i < Mathf.RoundToInt(entry.Value); i++)
					{
						LootButton newLootButton = CreateLootButton(entry.Key, distanceBetweenButtons, index, numberOfObjects);
						newLootButton.baubleStars.gameObject.SetActive(true);
						index++;
					}
					break;
			}
		}
	}
	
	public LootButton CreateLootButton(string buttonType, float distanceBetweenButtons, int index, int numberOfObjects)
	{
		GameObject newLootButtonGO = Instantiate(lootButtonPrefab, Vector3.zero, Quaternion.identity, lootButtonParent);
		LootButton newLootButton = newLootButtonGO.GetComponent<LootButton>();
		float xPos = (numberOfObjects - 1) * (distanceBetweenButtons / 2f) - (numberOfObjects - index - 1) * distanceBetweenButtons;
		newLootButton.rt.anchoredPosition = new Vector2(xPos, 0);
		newLootButton.image.sprite = lootTypes[buttonType].lootSprite;
		newLootButton.imageRT.sizeDelta = new Vector2(lootTypes[buttonType].lootSprite.rect.width, lootTypes[buttonType].lootSprite.rect.height);
		newLootButton.image.color = lootTypes[buttonType].lootSpriteColor;
		return newLootButton;
	}
	
	public Dictionary<string, float> GenerateLoot(int tier, string type)
	{
		Dictionary<string, float> lootToReturn = new Dictionary<string, float>();
		
		switch(type)
		{
			case "StandardFight":
			
			break;
			case "EliteFight":
			
			break;
			case "BossFight":
			
			break;
		}
		return lootToReturn;
	}
}
