using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class Baubles : MonoBehaviour
{
    public TextAsset baubleSpreadsheet;
	public Sprite[] baubleImages;
	public string unlockedBaublesFileName;
	public string unlockedBaublesFileVersion;
	
	public static Baubles instance;
	
	public List<Bauble> availableCommonBaubles = new List<Bauble>();
	public List<Bauble> availableUncommonBaubles = new List<Bauble>();
	public List<Bauble> availableRareBaubles = new List<Bauble>();
	public List<Bauble> availableLegendaryBaubles = new List<Bauble>();
	public List<Bauble> availableZodiacs = new List<Bauble>();
	public List<string> unlockedBaubles;
	
	void Awake()
	{
		instance = this;
	}
	
	[System.Serializable]
	public struct Bauble
	{
		public string baubleName;
		public string description;
		public int quantityOwned;
		public int maxQuantity;
		public int baseCost;
		public int costScalingAdditive;
		public float impact1;
		public float impact2;
		public string category;
		public Sprite sprite;
		public int id;
		public Bauble(string baubleName, string description, int maxQuantity, int baseCost, int costScalingAdditive, float impact1, float impact2, string category, Sprite sprite, int id)
		{
			this.baubleName = baubleName;
			this.description = description;
			this.quantityOwned = 0;
			this.maxQuantity = maxQuantity;
			this.baseCost = baseCost;
			this.costScalingAdditive = costScalingAdditive;
			this.impact1 = impact1;
			this.impact2 = impact2;
			this.category = category;
			this.sprite = sprite;
			this.id = id;
		}
	}
	
	public Dictionary<string, Bauble> baubles = new Dictionary<string, Bauble>();
	
	public int GetNumberOfBaublesOwnedByTag(string tag)
	{
		return baubles[tag].quantityOwned;
	}
	
	public float GetBaubleImpactFloatByTag(string tag)
	{
		return baubles[tag].quantityOwned * baubles[tag].impact1;
	}
	
	public int GetBaubleImpactIntByTag(string tag)
	{
		return Mathf.RoundToInt(baubles[tag].quantityOwned * baubles[tag].impact1);
	}
	
	public float GetBaubleImpact2FloatByTag(string tag)
	{
		return baubles[tag].quantityOwned * baubles[tag].impact2;
	}
	
	public int GetBaubleImpact2IntByTag(string tag)
	{
		return Mathf.RoundToInt(baubles[tag].quantityOwned * baubles[tag].impact2);
	}
	
	public Sprite GetSpriteFromCoordinates(string coords)
	{
		string rowString = coords.Substring(0, 1).ToUpper();
		char rowChar = char.Parse(rowString);
		int rowInt = rowChar - 'A';
		string columnString = coords.Substring(1);
		int columnInt = int.Parse(columnString);
		int imageIndex = rowInt * 16 + columnInt;
		return baubleImages[imageIndex];
	}
	
	public void ImportBaublesFromSpreadsheet()
	{
		PopulateUnlockedBaubles();
		string[] rows = baubleSpreadsheet.text.Split('\n');
		for(int i = 1; i < rows.Length; i++)
		{
			string[] columns = rows[i].Split(',');
			string tag = columns [0];
			string baubleName = columns[1];
			string description = columns [2].Replace("COMMA", ",");
			int maxQuantity = int.Parse(columns[3]);
			int baseCost = int.Parse(columns[4]);
			int costScalingAdditive = int.Parse(columns[5]);
			float impact1 = float.Parse(columns[6]);
			float impact2 = float.Parse(columns[7]);
			string category = columns[8];
			Sprite sprite = GetSpriteFromCoordinates(columns[9]);
			bool startsAvailable = bool.Parse(columns[10]);
			bool mustBeUnlocked = bool.Parse(columns[11]);
			int id = i - 1;
			baubles.Add(tag, new Bauble(baubleName, description, maxQuantity, baseCost, costScalingAdditive, impact1, impact2, category, sprite, id));
			if(startsAvailable && (!mustBeUnlocked || (mustBeUnlocked && IsBaubleIsUnlocked(tag))))
			{
				AddBaubleToAvailableBaubles(tag);
			}
		}
	}
	
	public void AddBaubleToAvailableBaubles(string tag)
	{
		switch(baubles[tag].category)
		{
			case "Common":
				availableCommonBaubles.Add(baubles[tag]);
			break;
			case "Uncommon":
				availableUncommonBaubles.Add(baubles[tag]);
			break;
			case "Rare":
				availableRareBaubles.Add(baubles[tag]);
			break;
			case "Legendary":
				availableLegendaryBaubles.Add(baubles[tag]);
			break;
			case "Zodiac":
				availableZodiacs.Add(baubles[tag]);
			break;
		}
	}
	
	public void PopulateUnlockedBaubles()
	{
		string unlockedBaublesFilePath = $"{LocalInterface.instance.localFilesDirectory}{unlockedBaublesFileName}.txt";
		if(File.Exists(unlockedBaublesFilePath))
		{
			string[] lines;
			using(StreamReader reader = new StreamReader(unlockedBaublesFilePath))
			{
				string unlockedBaublesData = reader.ReadToEnd();
				lines = unlockedBaublesData.Split('n');
			}
			if(lines[0].Trim() != unlockedBaublesFileVersion)
			{
				Debug.LogError($"Version mismatch in unlocked Baubles file. unlockedBaublesFilePath = {unlockedBaublesFilePath} File version is {lines[0].Trim()}, current version is {unlockedBaublesFileVersion}");
				return;
			}
			if(lines.Length > 1)
			{
				for(int i = 1; i < lines.Length; i++)
				{
					unlockedBaubles.Add(lines[i].Trim());
				}
			}
		}
		else
		{
			File.WriteAllText(unlockedBaublesFilePath, "");
			StreamWriter writer = new StreamWriter(unlockedBaublesFilePath, true);
			writer.Write(unlockedBaublesFileVersion);
			writer.Close();
			LocalInterface.instance.FileUpdated();
		}
	}
	
	public void UnlockBauble(string tag)
	{
		unlockedBaubles.Add(tag);	// Should new baubles be unlocked in the current run? I say, why not?
		AddBaubleToAvailableBaubles(tag);
		string unlockedBaublesFilePath = $"{LocalInterface.instance.localFilesDirectory}{unlockedBaublesFileName}.txt";
		StreamWriter writer = new StreamWriter(unlockedBaublesFilePath, true);
		writer.Write($"\n{tag}");
		writer.Close();
		LocalInterface.instance.FileUpdated();
	}
	
	public bool IsBaubleIsUnlocked(string tag)
	{
		if(unlockedBaubles.Contains(tag))
		{
			return true;
		}
		return false;
	}
}
