using UnityEngine;
using UnityEngine.UI;

public class EnemyAction : MonoBehaviour
{
	public RectTransform rt;
	public Label actionLabel;
	public Image actionImage;
	public RectTransform actionImageRT;
	
	public void UpdateAction(float actionImpact = 0f, string actionType = "")
	{
		if(actionType != "")
		{
			actionImage.sprite = LocalInterface.instance.enemyActionSprites[actionType];
			actionImageRT.sizeDelta = new Vector2(LocalInterface.instance.enemyActionSprites[actionType].rect.width, LocalInterface.instance.enemyActionSprites[actionType].rect.height);
		}
		else
		{
			actionImage.gameObject.SetActive(false);
		}
		if(!Mathf.Approximately(actionImpact, 0f))
		{
			actionLabel.ChangeText(LocalInterface.instance.ConvertFloatToString(actionImpact));
		}
		else
		{
			actionLabel.ChangeText("");
		}
	}
}