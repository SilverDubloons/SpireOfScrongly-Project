using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour, IPointerClickHandler
{
	public RectTransform rt;
	public Animation[] animations;
	public Image image;
	public RectTransform imageRT;
	
	private Dictionary<string, Animation> animationsDictionary = new Dictionary<string, Animation>();
	private IEnumerator animationCoroutine;
	private bool animating;
	private bool moving;
	private IEnumerator moveCoroutine;
	private bool jumping;
	private IEnumerator jumpCoroutine;
	private bool moveAttacking;
	private IEnumerator moveAttackCoroutine;
	
	public AnimationCurve jumpCurve;
	public float startingY;
	public float combatX;
	public float spriteWidth;
	public float startingHealth;
	public float currentHealth;
	public float startingShield;
	public float currentShield;
	public Vector2 combatOrigin;
	// public float startingEnemyActionY;
	public Vector2 targetReticleAdjust;
	
	public Image healthBackdrop;
	public RectTransform healthBackdropRT;
	public Label healthLabel;
	public Image shieldBackdrop;
	public RectTransform shieldBackdropRT;
	public Label shieldLabel;
	public Image statusBackdrop;
	public RectTransform statusBackdropRT;
	public RectTransform upcomingActionsRT;
	
	public bool randomizeTurnOrder;
	public Turn[] turns;
	public int currentTurn = 0;
	public Dictionary<string, CurrentStatusEffect> currentStatusEffects = new Dictionary<string, CurrentStatusEffect>();
	
	[System.Serializable]
	public struct CurrentStatusEffect
	{
		public int statusEffectImpact;
		public StatusEffect statusEffect;
		
		public CurrentStatusEffect(int impact, StatusEffect se)
		{
			statusEffectImpact = impact;
			statusEffect = se;
		}
	}
	
	
	[System.Serializable]
	public struct Action
	{
		public string actionType;
		public float actionImpact;
	}
	
	[System.Serializable]
	public struct Turn
	{
		public Action[] actions;
	}
	
	[System.Serializable]
    public class Animation
	{
		public string animationName;
		public Sprite[] animationSprites;
	}
	
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) * LocalInterface.instance.referenceResolution.x - LocalInterface.instance.referenceResolution.x / 2, (Input.mousePosition.y / Screen.height) * LocalInterface.instance.referenceResolution.y - LocalInterface.instance.referenceResolution.y / 2);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, results);
		foreach (RaycastResult result in results)
		{
			if (result.gameObject != null)
			{
				if(result.gameObject.name == "CharacterTargetClick")
				{
					Character targetCharacter = result.gameObject.transform.parent.GetComponent<Character>();
					if(CombatArea.instance.enemiesInCombat.Contains(targetCharacter))
					{
						CombatArea.instance.ChangeTargetedEnemy(targetCharacter);
					}
				}
			}
		}
	}
	
	public void PopulateAnimationDictionary()
	{
		for(int i = 0; i < animations.Length; i++)
		{
			animationsDictionary.Add(animations[i].animationName, animations[i]);
		}
	}
	
	public void SetupEnemyForCombat()
	{
		currentTurn = 0;
		if(randomizeTurnOrder)
		{
			for(int i = 0; i < turns.Length; i++)
			{
				int r = GameManager.instance.enemyActionsRNG.Range(0, i + 1);
				Turn temp = turns[i];
				turns[i] = turns[r];
				turns[r] = temp;
			}
		}
		currentHealth = startingHealth;
		healthBackdropRT.gameObject.SetActive(false);
		shieldBackdropRT.gameObject.SetActive(false);
		statusBackdropRT.gameObject.SetActive(false);
	}
	
	public void UpdateCharacterStats()
	{
		if(currentHealth > LocalInterface.instance.epsilon)
		{
			if(!healthBackdropRT.gameObject.activeSelf)
			{
				healthBackdropRT.gameObject.SetActive(true);
			}
			healthLabel.ChangeText(LocalInterface.instance.ConvertFloatToString(currentHealth));
		}
		else
		{
			if(healthBackdropRT.gameObject.activeSelf)
			{
				healthBackdropRT.gameObject.SetActive(false);
			}
		}
		if(currentShield > LocalInterface.instance.epsilon)
		{
			if(!shieldBackdropRT.gameObject.activeSelf)
			{
				shieldBackdropRT.gameObject.SetActive(true);
			}
			shieldLabel.ChangeText(LocalInterface.instance.ConvertFloatToString(currentShield));
		}
		else
		{
			if(shieldBackdropRT.gameObject.activeSelf)
			{
				shieldBackdropRT.gameObject.SetActive(false);
			}
		}
	}
	
	public void AddStatusEffect(string statusEffectName, int statusEffectQuantity)
	{
		if(currentStatusEffects.ContainsKey(statusEffectName))
		{
			CurrentStatusEffect effect = currentStatusEffects[statusEffectName];
			effect.statusEffectImpact += Mathf.RoundToInt(statusEffectQuantity);
			currentStatusEffects[statusEffectName] = effect;
		}
		else
		{
			CurrentStatusEffect newCurrentStatusEffect = new CurrentStatusEffect(statusEffectQuantity, null);
			currentStatusEffects.Add(statusEffectName, newCurrentStatusEffect);
		}
		UpdateStatusEffects();
	}
	
	public void UpdateStatusEffects()
	{
		int index = 0;
		int numberOfStatusEffects = GetNumberOfStatusEffects();
		if(currentStatusEffects.Count > 0)
		{
			statusBackdropRT.gameObject.SetActive(true);
		}
		foreach(KeyValuePair<string, CurrentStatusEffect> entry in currentStatusEffects)
		{
			if(entry.Value.statusEffect == null)
			{
				GameObject newStatusEffectGO = Instantiate(LocalInterface.instance.statusEffectPrefab, Vector3.zero, Quaternion.identity, statusBackdrop.transform);
				StatusEffect newStatusEffect = newStatusEffectGO.GetComponent<StatusEffect>();
				newStatusEffect.statusImage.sprite = LocalInterface.instance.enemyActionSprites[entry.Key];
				newStatusEffect.statusLabel.ChangeText(entry.Value.statusEffectImpact.ToString());
				if(numberOfStatusEffects == 1)
				{
					newStatusEffect.rt.anchoredPosition = Vector2.zero;
				}
				else
				{
					newStatusEffect.rt.anchoredPosition = new Vector2(-12 + 24 * index, 0);
				}
				index++;
			}
			else if(entry.Value.statusEffectImpact == 0)
			{
				Destroy(entry.Value.statusEffect.gameObject);
				currentStatusEffects.Remove(entry.Key);		// will this cause problems?
			}
			else
			{
				entry.Value.statusEffect.rt.anchoredPosition = new Vector2(-12 + 24 * index, 0);
				index++;
			}
		}
		if(numberOfStatusEffects == 0)
		{
			statusBackdropRT.gameObject.SetActive(false);
		}
	}
	
	public int GetNumberOfStatusEffects()
	{
		int statusEffects = 0;
		foreach(KeyValuePair<string, CurrentStatusEffect> entry in currentStatusEffects)
		{
			if(entry.Value.statusEffectImpact != 0)
			{
				statusEffects++;
			}
		}
		return statusEffects;
	}
	
	public void EnemyTakesDamage(float damageTaken)
	{
		float shieldDamageTaken = Mathf.Min(damageTaken, currentShield);
		currentShield -= shieldDamageTaken;
		float healthDamageTaken = damageTaken - shieldDamageTaken;
		currentHealth -= healthDamageTaken;
		UpdateCharacterStats();
		if(healthDamageTaken > LocalInterface.instance.epsilon)
		{
			StartAnimation("GetHit");
		}
		if(currentHealth <= LocalInterface.instance.epsilon)
		{
			StartAnimationAfterDelay("Death", GetDurationOfAnimation("GetHit"), false, false);
			StartEnemyDeath(GetDurationOfAnimation("GetHit") + GetDurationOfAnimation("Death"));
			healthBackdropRT.gameObject.SetActive(false);
			shieldBackdropRT.gameObject.SetActive(false);
			statusBackdropRT.gameObject.SetActive(false);
			upcomingActionsRT.gameObject.SetActive(false);
			CombatArea.instance.enemiesInCombat.Remove(this);
			if(CombatArea.instance.targetedEnemy == this)
			{
				if(CombatArea.instance.enemiesInCombat.Count > 0)
				{
					CombatArea.instance.enemiesInCombat.Sort((x,y) => 
					{
						int xComparison = Mathf.RoundToInt(x.rt.anchoredPosition.x - y.rt.anchoredPosition.x);
						return xComparison;
					});
					CombatArea.instance.ChangeTargetedEnemy(CombatArea.instance.enemiesInCombat[0]);
				}
				else
				{
					
				}
			}
		}
	}
	
	public float GetDurationOfAnimation(string animationName)
	{
		return LocalAnimations.instance.timeBetweenCharacterFrames * animationsDictionary[animationName].animationSprites.Length;
	}
	
	public void StartEnemyDeath(float delay)
	{
		StartCoroutine(EnemyDeath(delay));
	}
	
	private IEnumerator EnemyDeath(float delay)
	{
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime;
			yield return null;
		}
		t = 0;
		Color startingColor = image.color;
		Color endingColor = image.color;
		endingColor.a = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime;
			image.color = Color.Lerp(startingColor, endingColor, t / LocalInterface.instance.animationDuration);
			yield return null;
		}
		Destroy(this.gameObject);
	}
	
	public void EnemyJoinedBattle()
	{
		UpdateCharacterStats();
		UpdateEnemyTurn();
	}
	
	public void CycleTurnForward()
	{
		currentTurn++;
		if(currentTurn >= turns.Length)
		{
			currentTurn = 0;
		}
	}
	
	public void DeleteOldTurns()
	{
		for(int i = upcomingActionsRT.childCount - 1; i >= 0; i--)
		{
			Destroy(upcomingActionsRT.GetChild(i).gameObject);
		}
	}
	
	public void UpdateEnemyTurn()
	{
		for(int i = 0; i < turns[currentTurn].actions.Length; i++)
		{
			GameObject newEnemyActionGO = Instantiate(LocalInterface.instance.enemyActionPrefab, Vector2.zero, Quaternion.identity, upcomingActionsRT);
			EnemyAction newEnemyAction = newEnemyActionGO.GetComponent<EnemyAction>();
			newEnemyAction.rt.anchoredPosition = new Vector2(0, 20 * i);
			float actionImpactAfterStatusEffects = turns[currentTurn].actions[i].actionImpact;
			if(turns[currentTurn].actions[i].actionType == "Attack")
			{
				actionImpactAfterStatusEffects = GetAttackValueAfterStatusEffects(turns[currentTurn].actions[i].actionImpact);
			}
			newEnemyAction.UpdateAction(actionImpactAfterStatusEffects, turns[currentTurn].actions[i].actionType);
		}
	}
	
	public float GetAttackValueAfterStatusEffects(float baseAttack)
	{
		float modifiedAttack = baseAttack;
		if(currentStatusEffects.ContainsKey("IncreaseStrength"))
		{
			float attackFactor = 1f + currentStatusEffects["IncreaseStrength"].statusEffectImpact * 0.1f;
			modifiedAttack = modifiedAttack * attackFactor;
		}
		return modifiedAttack;
	}
	
	public void StartAnimationAfterDelay(string animationName, float delay, bool idleAfter = true, bool loop = false)
	{
		StartCoroutine(StartAnimationAfterDelayCoroutine(animationName, delay, idleAfter, loop));
	}
	
	public IEnumerator StartAnimationAfterDelayCoroutine(string animationName, float delay, bool idleAfter = true, bool loop = false)
	{
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime;
			yield return null;
		}
		StartAnimation(animationName, idleAfter, loop);
	}
	
	public void StartAnimation(string animationName, bool idleAfter = true, bool loop = false)
	{
		if(animating)
		{
			StopCoroutine(animationCoroutine);
		}
		animationCoroutine = PlayAnimation(animationName, LocalAnimations.instance.timeBetweenCharacterFrames, idleAfter, loop);
		StartCoroutine(animationCoroutine);
	}
	
	private IEnumerator PlayAnimation(string animationName, float timeBetweenFrames, bool idleAfter = true, bool loop = false)
	{
		animating = true;
		int spriteIndex = 0;
		Sprite[] animationSprites = animationsDictionary[animationName].animationSprites;
		while(spriteIndex < animationSprites.Length)
		{
			image.sprite = animationSprites[spriteIndex];
			// imageRT.sizeDelta = new Vector2(animationSprites[spriteIndex].rect.width, animationSprites[spriteIndex].rect.height);
			float t = 0;
			while(t < timeBetweenFrames)
			{
				t += Time.deltaTime;
				yield return null;
			}
			t -= timeBetweenFrames;
			spriteIndex++;
		}
		animating = false;
		if(idleAfter)
		{
			StartAnimation("Idle");
		}
		else if(loop)
		{
			StartAnimation(animationName, false, true);
		}
	}
	
	public void StartMoveAttackJumpBack(Vector2 destination)
	{
		if(moveAttacking)
		{
			StopCoroutine(moveAttackCoroutine);
		}
		if(jumping)
		{
			StopCoroutine(jumpCoroutine);
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, startingY);
		}
		if(animationsDictionary.ContainsKey("Jump"))
		{
			moveAttackCoroutine = MoveAttackJumpBack(destination);
		}
		else
		{
			moveAttackCoroutine = MoveAttackRunBack(destination);
		}
		StartCoroutine(moveAttackCoroutine);
	}
	
	public IEnumerator MoveAttackRunBack(Vector2 destination)
	{
		moveAttacking = true;
		StartAnimation("Run", false, true);
		StartMove(destination);
		float t = 0;
		while(t < LocalInterface.instance.animationDuration - LocalAnimations.instance.timeBetweenCharacterFrames * animationsDictionary["Attack"].animationSprites.Length)
		{
			t += Time.deltaTime;
			yield return null;
		}
		StartAnimation("Attack");
		t = 0;
		while(t < LocalAnimations.instance.timeBetweenCharacterFrames * animationsDictionary["Attack"].animationSprites.Length)
		{
			t += Time.deltaTime;
			yield return null;
		}
		StartAnimation("Run", false, true);
		StartMove(combatOrigin);
		imageRT.localScale = new Vector3(-imageRT.localScale.x, imageRT.localScale.y, imageRT.localScale.z);
		t = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime;
			yield return null;
		}
		StartAnimation("Idle");
		imageRT.localScale = new Vector3(-imageRT.localScale.x, imageRT.localScale.y, imageRT.localScale.z);
		moveAttacking = false;
	}
	
	public IEnumerator MoveAttackJumpBack(Vector2 destination)
	{
		moveAttacking = true;
		StartAnimation("Run", false, true);
		StartMove(destination);
		float t = 0;
		int r = UnityEngine.Random.Range(1, 4);
		string attackAnimation = $"Attack{r}";
		while(t < LocalInterface.instance.animationDuration - LocalAnimations.instance.timeBetweenCharacterFrames * animationsDictionary[attackAnimation].animationSprites.Length)
		{
			t += Time.deltaTime;
			yield return null;
		}
		StartAnimation(attackAnimation);
		t = 0;
		while(t < LocalAnimations.instance.timeBetweenCharacterFrames * animationsDictionary[attackAnimation].animationSprites.Length)
		{
			t += Time.deltaTime;
			yield return null;
		}
		StartAnimation("Jump", false, true);
		StartMove(combatOrigin);
		StartJump(20f, LocalInterface.instance.animationDuration, jumpCurve);
		t = 0;
		while(t < LocalInterface.instance.animationDuration * 0.25)
		{
			t += Time.deltaTime;
			yield return null;
		}
		StartAnimation("Fall", false, true);
		t = 0;
		while(t < LocalInterface.instance.animationDuration * 0.75)
		{
			t += Time.deltaTime;
			yield return null;
		}
		StartAnimation("Idle");
		moveAttacking = false;
	}
	
	public void StartJump(float heightGain, float duration, AnimationCurve heightCurve)
	{
		if(jumping)
		{
			StopCoroutine(jumpCoroutine);
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, startingY);
		}
		jumpCoroutine = Jump(heightGain, duration, heightCurve);
		StartCoroutine(jumpCoroutine);
	}
	
	public IEnumerator Jump(float heightGain, float duration, AnimationCurve heightCurve)
	{
		jumping = true;
		float heightOrigin = rt.anchoredPosition.y;
		float t = 0;
		while(t < duration)
		{
			t += Time.deltaTime;
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.Lerp(heightOrigin, heightOrigin + heightGain, heightCurve.Evaluate(t / duration)));
			yield return null;
		}
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, heightOrigin);
		jumping = false;
	}
	
	public void StartMove(Vector2 destination, float delay = 0f)
	{
		if(moving)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = Move(destination, delay);
		StartCoroutine(moveCoroutine);
}
	
	public IEnumerator Move(Vector2 destination, float delay = 0f)
	{
		moving = true;
		while(delay > 0f)
		{
			delay -= Time.deltaTime;
			yield return null;
		}
		Vector2 origin = rt.anchoredPosition;
		float t = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime;
			rt.anchoredPosition = Vector2.Lerp(origin, destination, LocalInterface.instance.animationCurve.Evaluate(t / LocalInterface.instance.animationDuration));
			yield return null;
		}
		rt.anchoredPosition = destination;
		moving = false;
	}
}
