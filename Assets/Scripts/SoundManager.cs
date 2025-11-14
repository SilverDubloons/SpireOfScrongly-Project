using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	public AudioClip[] tickSounds;
	public AudioClip[] cardPickupSounds;
	public AudioClip[] cardDropSounds;
	public AudioClip[] cardSlideSounds;
	public AudioClip[] cardShuffleSounds;
	
	public AudioSource soundSource;
	public AudioSource tickSource;
	
	public static SoundManager instance;
	void Awake()
	{
		instance = this;
	}
	
	public void PlaySound(AudioClip sound, float volumeFactor = 1f)
	{
		if(Preferences.instance.soundOn && (Application.isFocused || (!Application.isFocused && !Preferences.instance.muteOnFocusLost)))
		{
			soundSource.PlayOneShot(sound, Preferences.instance.soundVolume * volumeFactor);
		}
	}
	
	public void PlayCardPickupSound()
	{
		PlaySound(cardPickupSounds[Random.Range(0, cardPickupSounds.Length)], 0.5f);
	}
	
	public void PlayCardDropSound()
	{
		PlaySound(cardDropSounds[Random.Range(0, cardDropSounds.Length)], 0.5f);
	}
	
	public void PlayCardSlideSound()
	{
		PlaySound(cardSlideSounds[Random.Range(0, cardSlideSounds.Length)], 0.5f);
	}
	
	public void PlayCardShuffleSound()
	{
		PlaySound(cardShuffleSounds[Random.Range(0, cardShuffleSounds.Length)], 0.5f);
	}
	
	private float lastTickSoundTime = 0;
	private int tickSoundIndex = 0;
	
	public void PlayTickSound()
	{
		if(Preferences.instance.soundOn && (Application.isFocused || (!Application.isFocused && !Preferences.instance.muteOnFocusLost)))
		{
			if(Time.time - lastTickSoundTime > 0.2f)
			{
				tickSoundIndex = 0;
			}
			lastTickSoundTime = Time.time;
			tickSource.pitch = 1f + 0.05f * tickSoundIndex;
			tickSource.PlayOneShot(tickSounds[Random.Range(0,tickSounds.Length)], Preferences.instance.soundVolume * 0.5f);
			tickSoundIndex++;
		}
	}
}
