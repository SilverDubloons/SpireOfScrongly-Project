using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public GameObject gameManagerPrefab;
	
    public void NewGameClicked()
	{
		LocalAnimations.instance.mo["MainMenu"].StartMove("OffScreen");
		LocalAnimations.instance.gameplayCanvas.SetActive(true);
		LocalAnimations.instance.mo["Map"].TeleportTo("OffScreen");
		LocalAnimations.instance.mo["Map"].StartMove("OnScreen", LocalInterface.instance.animationDuration);
		LocalAnimations.instance.mo["DrawPile"].TeleportTo("OffScreen");
		LocalAnimations.instance.mo["DrawPile"].StartMove("OnScreen", LocalInterface.instance.animationDuration);
		LocalAnimations.instance.mo["DiscardPile"].TeleportTo("OffScreen");
		LocalAnimations.instance.mo["DiscardPile"].StartMove("OnScreen", LocalInterface.instance.animationDuration);
		LocalAnimations.instance.mo["HandArea"].TeleportTo("OffScreen");
		// LocalAnimations.instance.mo["PlayArea"].TeleportTo("OffScreen");
		LocalAnimations.instance.mo["CombatArea"].TeleportTo("OffScreen");
		LocalAnimations.instance.mo["HandPower"].TeleportTo("OffScreen");
		LocalAnimations.instance.mo["Currency"].TeleportTo("OffScreen");
		LocalAnimations.instance.mo["Currency"].StartMove("OnScreen", LocalInterface.instance.animationDuration);
		
		GameObject newGameManagerGO = Instantiate(gameManagerPrefab, Vector3.zero, Quaternion.identity);
		GameManager newGameManager = newGameManagerGO.GetComponent<GameManager>();
		HandPower.instance.RearangeSuitOrder();
		newGameManager.StartNewGame("Standard", "Knight", UnityEngine.Random.Range(0, int.MaxValue));
	}
}
