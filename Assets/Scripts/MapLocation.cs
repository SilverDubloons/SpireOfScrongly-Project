using UnityEngine;

public class MapLocation : MonoBehaviour
{
    public string eventType;
	public string eventEncounter; // for Battle, expect e.g. "Flying Eye,Goblin,Skeleton"
	
	public void LocationSelected()
	{
		switch(eventType)
		{
			case "Battle":
				LocalAnimations.instance.mo["Map"].StartMove("OffScreen");
				LocalAnimations.instance.mo["DrawPile"].StartMove("OnScreen");
				LocalAnimations.instance.mo["DiscardPile"].StartMove("OnScreen");
				LocalAnimations.instance.mo["HandArea"].StartMove("OnScreen");
				LocalAnimations.instance.mo["CombatArea"].StartMove("OnScreen");
				LocalAnimations.instance.mo["HandPower"].StartMove("OnScreen");
				HandArea.instance.StartDrawCards(GameManager.instance.baseHandSize + Baubles.instance.GetBaubleImpactIntByTag("IncreaseHandSize"), LocalInterface.instance.animationDuration);
				HandArea.instance.SelectedCardsUpdated();
				string[] enemies = eventEncounter.Split(',');
				CombatArea.instance.StartCombat(GameManager.instance.characterName, enemies);
				
				break;
		}
	}
}
