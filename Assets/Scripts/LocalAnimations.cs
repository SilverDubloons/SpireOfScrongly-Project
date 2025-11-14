using UnityEngine;
using System.Collections.Generic;

public class LocalAnimations : MonoBehaviour
{
	public MovingObject[] movingObjects;
	public Dictionary<string, MovingObject> mo = new Dictionary<string, MovingObject>();
	
	public static LocalAnimations instance;
	public GameObject mainMenuCanvas;
	public GameObject gameplayCanvas;
	public float timeBetweenCharacterFrames;
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		mainMenuCanvas.SetActive(true);
		for(int i = 0; i < movingObjects.Length; i++)
		{
			mo.Add(movingObjects[i].referenceName, movingObjects[i]);
		}
		gameplayCanvas.SetActive(false);
		mo["MainMenu"].TeleportTo("OffScreen");
		mo["MainMenu"].StartMove("OnScreen");
	}
}
