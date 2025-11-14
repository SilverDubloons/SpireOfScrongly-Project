using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections;

public class Label : MonoBehaviour
{
	public RectTransform rt;
    public TMP_Text labelShadow;
	public TMP_Text label;
	public RectTransform labelShadowRT;
	public RectTransform labelRT;
	
	private bool expandRetracting;
	private IEnumerator expandRetractCoroutine;
	
	public void ChangeText(string newText, bool filterRichText = false)
	{
		if(filterRichText)
		{
			labelShadow.text = RemoveRichTextTags(newText);
		}
		else
		{
			labelShadow.text = newText;
		}
		label.text = newText;
	}
	
	public string RemoveRichTextTags(string input)
	{
		string pattern = @"<.*?>";
		return Regex.Replace(input, pattern, string.Empty);
	}
	
	public void ForceMeshUpdate()
	{
		labelShadow.ForceMeshUpdate(true, true);
		label.ForceMeshUpdate(true, true);
	}
	
	public void ChangeColor(Color color)
	{
		label.color = color;
	}
	
	public void StartExpandRetract(float duration, float expandFactor)
	{
		if(expandRetracting)
		{
			StopCoroutine(expandRetractCoroutine);
		}
		expandRetractCoroutine = ExpandRetract(duration, expandFactor);
		StartCoroutine(expandRetractCoroutine);
	}
	
	private IEnumerator ExpandRetract(float duration, float expandFactor)
	{
		expandRetracting = true;
		float t = 0;
		Vector2 expansionScale = new Vector2(expandFactor, expandFactor);
		while(t < duration / 2)
		{
			t += Time.deltaTime;
			labelShadowRT.localScale = Vector2.Lerp(Vector2.one, expansionScale, t / (duration / 2));
			labelRT.localScale = Vector2.Lerp(Vector2.one, expansionScale, t / (duration / 2));
			yield return null;
		}
		t = 0;
		while(t < duration / 2)
		{
			t += Time.deltaTime;
			labelShadowRT.localScale = Vector2.Lerp(expansionScale, Vector2.one, t / (duration / 2));
			labelRT.localScale = Vector2.Lerp(expansionScale, Vector2.one, t / (duration / 2));
			yield return null;
		}
		labelShadowRT.localScale = Vector2.one;
		labelRT.localScale = Vector2.one;
		expandRetracting = false;
	}
}
