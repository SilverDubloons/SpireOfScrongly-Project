using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class ButtonPlus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform rt;
	public bool isButton;
	[ShowIf("isButton")]
	public bool buttonEnabled = true;
	[ShowIf("isButton")]
	public bool playClickingSound = true;
	[ShowIf("isButton", "playClickingSound")]
	public AudioClip clickSound;
	[ShowIf("isButton", "playClickingSound")]
	public AudioSource soundSource;
	[ShowIf("isButton", "playClickingSound")]
	public float volumeFactor;
	private bool holdingDown = false;
	private bool mouseOverButton = false;
	[ShowIf("isButton")]
	public bool specialState = false;
	[ShowIf("isButton")]
	[SerializeField]
    private UnityEvent onClickEvent;
	[ShowIf("isButton")]
	[SerializeField]
    private UnityEvent onDoubleClickEvent;
	private float timeOfLastClick;
	private int clicksInARow = 0;
	
	[ShowIf("isButton")]
	public bool moveImageWhenClicked = true;
	private Vector2 buttonImageOrigin;
	[ShowIf("isButton", "moveImageWhenClicked")]
	public RectTransform buttonImageRT;
	[ShowIf("isButton", "moveImageWhenClicked")]
	public Vector2 buttonImageDestinationAdditive = new Vector2(0, -2f);
	[ShowIf("isButton", "moveImageWhenClicked")]
	public float moveImageDuration = 0.05f;
	private IEnumerator moveImageCoroutine;
	private bool movingImage = false;
	
	public bool expandEnabled;
	[ShowIf("expandEnabled")]
	public float expansionFactor = 1.05f;
	[ShowIf("expandEnabled")]
	public float expansionDuration = 0.1f;
	private IEnumerator scaleChangeCoroutine;
	private bool changingScale = false;
	
	public bool changeColorEnabled;
	[ShowIf("changeColorEnabled")]
	public bool colorChangeIsMultiplicative = true;
	[ShowIf("changeColorEnabled")]
	public Color baseColor = Color.blue;
	[ShowIf("changeColorEnabled")]
	public Color specialStateColor = Color.green;
	[ShowIf("changeColorEnabled")]
	public Color mouseOverColor = new Color(0.86f, 0.86f, 0.86f, 1f);
	[ShowIf("changeColorEnabled")]
	public Color disabledColor = new Color(0.2f, 0.2f, 0.2f, 1f);
	[ShowIf("changeColorEnabled")]
	public Image buttonImage;
	[ShowIf("changeColorEnabled")]
	public Image shadowImage;
	[ShowIf("changeColorEnabled")]
	public float changeColorDuration = 0.1f;
	private IEnumerator colorChangeCoroutine;
	private bool changingColor = false;
	
	private IEnumerator checkForGlobalMouseUpCoroutine;
	private bool checkingForGlobalMouseUp = false;
	
	public bool tickOnMouseOver = false;
	
	public Label buttonLabel;
	
	void Start()
	{
		if(isButton)
		{
			buttonImageOrigin = buttonImageRT.anchoredPosition;
			if(!buttonEnabled)
			{
				buttonImage.color = disabledColor;
			}
			else
			{
				if(specialState)
				{
					buttonImage.color = specialStateColor;
				}
				else
				{
					buttonImage.color = baseColor;
				}
			}
		}
	}
	
	public void ChangeButtonText(string newText)
	{
		buttonLabel.ChangeText(newText);
	}
	
	public void ChangeButtonEnabled(bool newEnabledState)
	{
		buttonEnabled = newEnabledState;
		if(changingScale)
		{
			StopCoroutine(scaleChangeCoroutine);
			changingScale = false;
		}
		if(changingColor)
		{
			StopCoroutine(colorChangeCoroutine);
			changingColor = false;
		}
		if(movingImage)
		{
			StopCoroutine(moveImageCoroutine);
			movingImage = false;
		}
		rt.localScale = Vector3.one;
		buttonImageRT.anchoredPosition = buttonImageOrigin;
		if(buttonEnabled && buttonImage != null)
		{
			buttonImage.color = baseColor;
			Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) * LocalInterface.instance.referenceResolution.x - LocalInterface.instance.referenceResolution.x / 2, (Input.mousePosition.y / Screen.height) * LocalInterface.instance.referenceResolution.y - LocalInterface.instance.referenceResolution.y / 2);
			List<RaycastResult> results = new List<RaycastResult>();
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			EventSystem.current.RaycastAll(pointerEventData, results);
			foreach(RaycastResult result in results)
			{
				if(result.gameObject == buttonImage.gameObject || result.gameObject == shadowImage.gameObject)
				{
					OnPointerEnter(pointerEventData);
					break;
				}
			}
		}
		else
		{
			buttonImage.color = disabledColor;
		}
	}
	
	public void ChangeSpecialState(bool isSpecial)
	{
		specialState = isSpecial;
		if(specialState)
		{
			buttonImage.color = specialStateColor;
		}
		else
		{
			buttonImage.color = baseColor;
		}
		if(mouseOverButton)
		{
			buttonImage.color = buttonImage.color * mouseOverColor;
		}
	}
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		if(tickOnMouseOver)
		{
			SoundManager.instance.PlayTickSound();
		}
		if(isButton && !buttonEnabled)
		{
			return;
		}
		if(expandEnabled)
		{
			if(changingScale)
			{
				StopCoroutine(scaleChangeCoroutine);
			}
			scaleChangeCoroutine = ChangeScale(new Vector3(expansionFactor, expansionFactor, 1f), expansionDuration);
			StartCoroutine(scaleChangeCoroutine);
		}
		if(changeColorEnabled)
		{
			if(changingColor)
			{
				StopCoroutine(colorChangeCoroutine);
			}
			if(colorChangeIsMultiplicative)
			{
				if(specialState)
				{
					colorChangeCoroutine = ChangeColor(specialStateColor * mouseOverColor, changeColorDuration);
				}
				else
				{
					colorChangeCoroutine = ChangeColor(baseColor * mouseOverColor, changeColorDuration);
				}
			}
			else
			{
				colorChangeCoroutine = ChangeColor(mouseOverColor, changeColorDuration);
			}
			StartCoroutine(colorChangeCoroutine);
		}
		if(isButton)
		{
			mouseOverButton = true;
			if(holdingDown)
			{
				if(movingImage)
				{
					StopCoroutine(moveImageCoroutine);
				}
				moveImageCoroutine = MoveImage(buttonImageOrigin + buttonImageDestinationAdditive, moveImageDuration);
				StartCoroutine(moveImageCoroutine);
				if(checkingForGlobalMouseUp)
				{
					StopCoroutine(checkForGlobalMouseUpCoroutine);
					checkingForGlobalMouseUp = false;
				}
			}
		}
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		if(isButton && !buttonEnabled)
		{
			return;
		}
		if(expandEnabled)
		{
			if(changingScale)
			{
				StopCoroutine(scaleChangeCoroutine);
			}
			scaleChangeCoroutine = ChangeScale(Vector3.one, expansionDuration);
			StartCoroutine(scaleChangeCoroutine);
		}
		if(changeColorEnabled)
		{
			if(changingColor)
			{
				StopCoroutine(colorChangeCoroutine);
			}
			if(specialState)
			{
				colorChangeCoroutine = ChangeColor(specialStateColor, changeColorDuration);
			}
			else
			{
				colorChangeCoroutine = ChangeColor(baseColor, changeColorDuration);
			}
			StartCoroutine(colorChangeCoroutine);
		}
		if(isButton)
		{
			mouseOverButton = false;
			if(holdingDown)
			{
				if(movingImage)
				{
					StopCoroutine(moveImageCoroutine);
				}
				moveImageCoroutine = MoveImage(buttonImageOrigin, moveImageDuration);
				StartCoroutine(moveImageCoroutine);
				checkForGlobalMouseUpCoroutine = CheckForGlobalMouseUp();
				StartCoroutine(checkForGlobalMouseUpCoroutine);
			}
		}
	}
	
	public void OnPointerDown(PointerEventData pointerEventData)
	{
		if(!isButton || !buttonEnabled)
		{
			return;
		}
		holdingDown = true;
		if(moveImageWhenClicked)
		{
			if(movingImage)
			{
				StopCoroutine(moveImageCoroutine);
			}
			moveImageCoroutine = MoveImage(buttonImageOrigin + buttonImageDestinationAdditive, moveImageDuration);
			StartCoroutine(moveImageCoroutine);
		}
	}
	
	public void OnPointerUp(PointerEventData pointerEventData)
	{
		if(!isButton || !buttonEnabled)
		{
			return;
		}
		if(onDoubleClickEvent.GetPersistentEventCount() > 0)
		{
			if(Time.time - timeOfLastClick > Preferences.instance.maxTimeBetweenDoubleClicks)
			{
				clicksInARow = 0;
			}
			timeOfLastClick = Time.time;
			clicksInARow++;
			if(mouseOverButton && holdingDown)
			{
				if(clicksInARow >= 2)
				{
					onDoubleClickEvent.Invoke();
				}
				else
				{
					onClickEvent.Invoke();
				}
			}
		}
		else
		{
			if(mouseOverButton && holdingDown)
			{
				onClickEvent.Invoke();
			}
		}
		if(playClickingSound && mouseOverButton && holdingDown)
		{
			if(Preferences.instance.soundOn && (Application.isFocused || (!Application.isFocused && !Preferences.instance.muteOnFocusLost)))
			{
				soundSource.PlayOneShot(clickSound, Preferences.instance.soundVolume * volumeFactor);
			}
		}
		if(moveImageWhenClicked)
		{
			if(movingImage)
			{
				StopCoroutine(moveImageCoroutine);
			}
			moveImageCoroutine = MoveImage(buttonImageOrigin, moveImageDuration);
			StartCoroutine(moveImageCoroutine);
		}
		holdingDown = false;
	}
	
	private IEnumerator ChangeScale(Vector3 destinationScale, float duration)
	{
		changingScale = true;
		Vector3 startingScale = rt.localScale;
		float t = 0;
		while(t < duration)
		{
			t += Time.deltaTime;
			rt.localScale = new Vector3(Mathf.Lerp(startingScale.x, destinationScale.x, t / duration), Mathf.Lerp(startingScale.y, destinationScale.y, t / duration), 1f);
			yield return null;
		}
		rt.localScale = destinationScale;
		changingScale = false;
	}
	
	private IEnumerator ChangeColor(Color destinationColor, float duration)
	{
		changingColor = true;
		Color originColor = buttonImage.color;
		float t = 0;
		while(t < duration)
		{
			t += Time.deltaTime;
			buttonImage.color = Color.Lerp(originColor, destinationColor, t / duration);
			yield return null;
		}
		buttonImage.color = destinationColor;
		changingColor = false;
	}
	
	private IEnumerator MoveImage(Vector2 destination, float duration)
	{
		movingImage = true;
		Vector2 origin = buttonImageRT.anchoredPosition;
		float t = 0;
		while(t < duration)
		{
			t += Time.deltaTime;
			buttonImageRT.anchoredPosition = Vector2.Lerp(origin, destination, t / duration);
			yield return null;
		}
		buttonImageRT.anchoredPosition = destination;
		movingImage = false;
	}
	
	private IEnumerator CheckForGlobalMouseUp()
	{
		checkingForGlobalMouseUp = true;
		while(Input.GetMouseButton(0))
		{
			yield return null;
		}
		holdingDown = false;
		checkingForGlobalMouseUp = false;
	}
}

#if UNITY_EDITOR

public class ShowIfAttribute : PropertyAttribute
{
    public string[] ConditionFieldNames { get; private set; }
    public bool RequiredValue { get; private set; } = true;
    
    public ShowIfAttribute(params string[] conditionFieldNames)
    {
        ConditionFieldNames = conditionFieldNames;
    }

    public ShowIfAttribute(bool requiredValue, params string[] conditionFieldNames)
    {
        ConditionFieldNames = conditionFieldNames;
        RequiredValue = requiredValue;
    }
}

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        bool shouldShow = true;

        foreach (string conditionFieldName in showIf.ConditionFieldNames)
        {
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionFieldName);
            
            if (conditionProperty == null) continue;
            
            bool conditionMet = conditionProperty.propertyType switch
            {
                SerializedPropertyType.Boolean => conditionProperty.boolValue == showIf.RequiredValue,
                _ => true // Default to showing if we don't know how to evaluate
            };

            shouldShow &= conditionMet;
        }

        if (shouldShow)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        bool shouldShow = true;

        foreach (string conditionFieldName in showIf.ConditionFieldNames)
        {
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionFieldName);
            
            if (conditionProperty == null) continue;
            
            bool conditionMet = conditionProperty.propertyType switch
            {
                SerializedPropertyType.Boolean => conditionProperty.boolValue == showIf.RequiredValue,
                _ => true
            };

            shouldShow &= conditionMet;
        }

        return shouldShow ? EditorGUI.GetPropertyHeight(property, label) : -EditorGUIUtility.standardVerticalSpacing;
    }
}

#endif