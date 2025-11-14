using UnityEngine;

public class Preferences : MonoBehaviour
{
    public bool soundOn;
	public bool musicOn;
	public float soundVolume;
	public float musicVolume;
	public bool muteOnFocusLost;
	public float maxTimeBetweenDoubleClicks;
	public bool cheatsOn;
	
	public static Preferences instance;
	
	void Awake()
	{
		instance = this;
	}
}
