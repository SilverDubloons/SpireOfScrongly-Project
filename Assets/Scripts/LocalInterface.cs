using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class LocalInterface : MonoBehaviour
{
	public Preferences preferences;
	public AnimationCurve animationCurve;
	public float animationDuration;
	public Image drawPileCardback;
	public Label drawPileLabel;
	public Image discardPileCardback;
	public Label discardPileLabel;
	public Vector2 referenceResolution;
	public Color[] suitColors;
	public string[] handNames;
	public float epsilon;
	public Color defaultTextColor;
	public int[] suitOrder; // 0 = spade, 1 = club, 2 = heart, 3 = diamond, 4 = rainbow
	public string[] suitNames;
	public Dictionary<string, int> suitOrderDictionary = new Dictionary<string, int>();
	public Dictionary<string, int> baseSuitOrderDictionary = new Dictionary<string, int>();
	public Dictionary<int, string> suitNamesByOrderDictionary = new Dictionary<int, string>();
	public Dictionary<string, Sprite> enemyActionSprites = new Dictionary<string, Sprite>();
	public EnemyActionSprite[] enemyActionSpritesToLoad;
	public GameObject enemyActionPrefab;
	public Vector2 cardSize;
	public GameObject statusEffectPrefab;
	public Label currencyLabel;
	public string gameName;
	public string localFilesDirectory;
	public GameObject baubleIconPrefab;
	
	[DllImport("__Internal")]
    private static extern void JS_FileSystem_Sync();
	
	[System.Serializable]
	public struct EnemyActionSprite
	{
		public string actionName;
		public Sprite actionSprite;
	}
	
    public static LocalInterface instance;
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		for(int i = 0; i < suitOrder.Length; i++)
		{
			suitOrderDictionary.Add(suitNames[i], suitOrder[i]);
			suitNamesByOrderDictionary.Add(suitOrder[i], suitNames[i]);
			baseSuitOrderDictionary.Add(suitNames[i], i); 
		}
		for(int i = 0; i < enemyActionSpritesToLoad.Length; i++)
		{
			enemyActionSprites.Add(enemyActionSpritesToLoad[i].actionName, enemyActionSpritesToLoad[i].actionSprite);
		}
		#if UNITY_WEBGL && !UNITY_EDITOR
			if(!Directory.Exists($"/idbfs/{gameName}"))
			{
				Directory.CreateDirectory("/idbfs/{gameName}");
			}
			localFilesDirectory = "/idbfs/{gameName}/";
		#else
			localFilesDirectory = $"{Application.persistentDataPath}/";
		#endif
	}
	
	public void FileUpdated()
	{
		#if UNITY_WEBGL && !UNITY_EDITOR
			JS_FileSystem_Sync();
		#endif
	}
	
	public void TwitterClicked()
	{
		Application.OpenURL("https://twitter.com/SilverDubloons");
	}
	
	public void KoFiClicked()
	{
		Application.OpenURL("https://ko-fi.com/silverdubloons");
	}
	
	public void DiscordClicked()
	{
		Application.OpenURL("https://discord.gg/TdJJBgbWTf");
	}
	
	public string ConvertFloatToString(float f)
	{
		string prefix = "";
		if(f < 0)
		{
			prefix = "-";
		}
		f = Mathf.Abs(f);
		string suffix = "";
		string formattedNumber = "";
		if(f >= 1000000000000000)
		{
			suffix = "e" + (Mathf.Floor(Mathf.Log10(f) / 3) * 3).ToString();
			float exponentNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3));
			if(exponentNumber > 100)
			{
				formattedNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3)).ToString("0");
			}
			else if(exponentNumber > 10)
			{
				formattedNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3)).ToString("0.#");
			}
			else
			{
				formattedNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3)).ToString("0.##");
			}
		}
		else if(f >= 100000000000000)
		{
			formattedNumber = (f/1000000000000f).ToString("0");
			suffix = "T";
		}
		else if(f >= 10000000000000)
		{
			formattedNumber = (f/1000000000000f).ToString("0.#");
			suffix = "T";
		}
		else if(f >= 1000000000000)
		{
			formattedNumber = (f/1000000000000f).ToString("0.##");
			suffix = "T";
		}
		else if(f >= 100000000000)
		{
			formattedNumber = (f/1000000000f).ToString("0");
			suffix = "B";
		}
		else if(f >= 10000000000)
		{
			formattedNumber = (f/1000000000f).ToString("0.#");
			suffix = "B";
		}
		else if(f >= 1000000000)
		{
			formattedNumber = (f/1000000000f).ToString("0.##");
			suffix = "B";
		}
		else if(f >= 100000000)
		{
			formattedNumber = (f/1000000f).ToString("0");
			suffix = "M";
		}
		else if(f >= 10000000)
		{
			formattedNumber = (f/1000000f).ToString("0.#");
			suffix = "M";
		}
		else if(f >= 1000000)
		{
			formattedNumber = (f/1000000f).ToString("0.##");
			suffix = "M";
		}
		else if(f >= 100000)
		{
			formattedNumber = (f/1000f).ToString("0");
			suffix = "K";
		}
		else if(f > 100)
		{
			formattedNumber = f.ToString("0");
		}
		else if(f > 10)
		{
			formattedNumber = f.ToString("0.#");
		}
		else if(f > 1)
		{
			formattedNumber = f.ToString("0.##");
		}
		else
		{
			formattedNumber = f.ToString("0.###");
		}
		return prefix + formattedNumber + "" + suffix; // at < e100 longest string output is 7 digits ex 3.14e18
	}
	
	public string ConvertRankAndSuitToString(int rank, int suit)
	{
		string cardString = string.Empty;
		switch(rank)
		{
			case 0:
				cardString += "2";
				break;
			case 1:
				cardString += "3";
				break;
			case 2:
				cardString += "4";
				break;
			case 3:
				cardString += "5";
				break;
			case 4:
				cardString += "6";
				break;
			case 5:
				cardString += "7";
				break;
			case 6:
				cardString += "8";
				break;
			case 7:
				cardString += "9";
				break;
			case 8:
				cardString += "T";
				break;
			case 9:
				cardString += "J";
				break;
			case 10:
				cardString += "Q";
				break;
			case 11:
				cardString += "K";
				break;
			case 12:
				cardString += "A";
				break;
			default:
				cardString += "?";
				break;
		}
		switch(suit)
		{
			case 0:
				cardString += "s";
				break;
			case 1:
				cardString += "c";
				break;
			case 2:
				cardString += "h";
				break;
			case 3:
				cardString += "d";
				break;
			case 4:
				cardString += "r";
				break;
			default:
				cardString += "?";
				break;
		}
		return cardString;
	}
}
