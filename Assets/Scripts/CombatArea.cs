using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CombatArea : MonoBehaviour
{
    public static CombatArea instance;
	public GameObject[] charactersArray;
	public Dictionary<string, GameObject> characters = new Dictionary<string, GameObject>();
	public float playerSpawnX;
	public float enemySpawnX;
	public Transform characterParent;
	public Character targetedEnemy;
	public Character playerCharacter;
	public bool combatAreaLocked;
	public RectTransform targetReticle;
	private bool playerExectutingAttack;
	private IEnumerator playerExecuteAttackCoroutine;
	private bool enemyExectutingAttack;
	private IEnumerator enemyExecuteAttackCoroutine;
	public List<Character> enemiesInCombat = new List<Character>();
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		for(int i = 0; i < charactersArray.Length; i++)
		{
			characters.Add(charactersArray[i].name, charactersArray[i]);
		}
	}
	
	public void StartCombat(string playerCharacterName, string[] enemies)
	{
		combatAreaLocked = true;
		GameObject newPlayerGO = Instantiate(characters[playerCharacterName], Vector2.zero, Quaternion.identity, characterParent);
		newPlayerGO.name = playerCharacterName;
		Character newPlayer = newPlayerGO.GetComponent<Character>();
		playerCharacter = newPlayer;
		newPlayer.currentHealth = GameManager.instance.currentHealth;
		newPlayer.currentShield = 0;
		// newPlayer.UpdateCharacterStats();
		newPlayer.PopulateAnimationDictionary();
		newPlayer.rt.anchoredPosition = new Vector2(playerSpawnX, newPlayer.startingY);
		newPlayer.StartAnimation("Run", false, true);
		newPlayer.StartAnimationAfterDelay("Idle", LocalInterface.instance.animationDuration);
		newPlayer.StartMove(new Vector2(newPlayer.combatX, newPlayer.startingY));
		newPlayer.combatOrigin = new Vector2(newPlayer.combatX, newPlayer.startingY);
		for(int i = 0; i < enemies.Length; i++)
		{
			GameObject newEnemyGO = Instantiate(characters[enemies[i]], Vector2.zero, Quaternion.identity, characterParent);
			newEnemyGO.name = enemies[i];
			Character newEnemy = newEnemyGO.GetComponent<Character>();
			enemiesInCombat.Add(newEnemy);
			if(i == 0)
			{
				targetedEnemy = newEnemy;
			}
			newEnemy.PopulateAnimationDictionary();
			newEnemy.SetupEnemyForCombat();
			newEnemy.rt.anchoredPosition = new Vector2(enemySpawnX, newEnemy.startingY);
			newEnemy.StartAnimation("Run", false, true);
			newEnemy.StartAnimationAfterDelay("Idle", LocalInterface.instance.animationDuration);
			newEnemy.StartMove(new Vector2(newEnemy.combatX - 54 * (enemies.Length - i - 1), newEnemy.startingY));
			newEnemy.combatOrigin = new Vector2(newEnemy.combatX - 54 * (enemies.Length - i - 1), newEnemy.startingY);
		}
		
		StartCoroutine(SetupEnemiesForCombatAfterDelay(LocalInterface.instance.animationDuration, enemiesInCombat));
	}
	
	public void ChangeTargetedEnemy(Character newTarget)
	{
		targetedEnemy = newTarget;
		targetReticle.gameObject.SetActive(true);
		targetReticle.anchoredPosition = newTarget.rt.anchoredPosition + newTarget.targetReticleAdjust;
	}
	
	private IEnumerator SetupEnemiesForCombatAfterDelay(float delay, List<Character> enemies)
	{
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime;
			yield return null;
		}
		playerCharacter.UpdateCharacterStats();
		for(int i = 0; i < enemies.Count; i++)
		{
			enemies[i].EnemyJoinedBattle();
			if(enemies[i] == targetedEnemy)
			{
				targetReticle.gameObject.SetActive(true);
				targetReticle.anchoredPosition = enemies[i].rt.anchoredPosition + enemies[i].targetReticleAdjust;
			}
		}
		combatAreaLocked = false;
	}
	
	public void StartCombatTurn(float delay)
	{
		combatAreaLocked = true;
		StartCoroutine(CombatTurn(delay));
	}
	
	private IEnumerator CombatTurn(float delay)
	{
		playerCharacter.transform.SetSiblingIndex(playerCharacter.transform.parent.childCount - 1);
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime;
			yield return null;
		}
		for(int i = 0; i < 4; i++)
		{
			string currentSuit = LocalInterface.instance.suitNamesByOrderDictionary[i];
			int currentSuitBaseInt = LocalInterface.instance.baseSuitOrderDictionary[currentSuit];
			int currentSuitOrderedInt = LocalInterface.instance.suitOrderDictionary[currentSuit];
			float baseEffect = HandArea.instance.lockedInHandBaseValues[currentSuitBaseInt] * HandArea.instance.lockedInHandMultipliers[currentSuitBaseInt];
			if(!Mathf.Approximately(baseEffect, 0))
			{
				Debug.Log($"Doing {currentSuit} with baseEffect of {baseEffect}");
				switch(currentSuit)
				{
					case "Spade":
						GameManager.instance.ModifyCurrentShield(baseEffect);
					break;
					case "Club":
						StartPlayerExecuteAttack(baseEffect);
						while(playerExectutingAttack)
						{
							yield return null;
						}
					break;
					case "Heart":
						GameManager.instance.ModifyCurrentHealth(baseEffect);
					break;
					case "Diamond":
						GameManager.instance.ModifyCurrentCurrency(baseEffect);
					break;
				}
				
				t = 0;
				while(t < LocalInterface.instance.animationDuration)
				{
					t += Time.deltaTime;
					yield return null;
				}
			}
		}
		// enemy turns start
		for(int i = 0; i < enemiesInCombat.Count; i++)
		{
			enemiesInCombat[i].currentShield = 0;
			enemiesInCombat[i].UpdateCharacterStats();
			enemiesInCombat[i].transform.SetSiblingIndex(enemiesInCombat[i].transform.parent.childCount - 1);
			for(int k = 0; k < enemiesInCombat[i].turns[enemiesInCombat[i].currentTurn].actions.Length; k++)
			{
				string currentAction = enemiesInCombat[i].turns[enemiesInCombat[i].currentTurn].actions[k].actionType;
				float currentActionImpact = enemiesInCombat[i].turns[enemiesInCombat[i].currentTurn].actions[k].actionImpact;
				switch(currentAction)
				{
					case "Attack":
						currentActionImpact = enemiesInCombat[i].GetAttackValueAfterStatusEffects(currentActionImpact);
						StartEnemyExexuteAttack(currentActionImpact, enemiesInCombat[i]);
						while(enemyExectutingAttack)
						{
							yield return null;
						}
					break;
					case "Defend":
						enemiesInCombat[i].currentShield += currentActionImpact;
						enemiesInCombat[i].UpdateCharacterStats();
					break;
					case "IncreaseStrength":
						enemiesInCombat[i].AddStatusEffect(currentAction, Mathf.RoundToInt(currentActionImpact));
					break;
				}
				t = 0;
				while(t < LocalInterface.instance.animationDuration)
				{
					t += Time.deltaTime;
					yield return null;
				}
				enemiesInCombat[i].DeleteOldTurns();
			}
		}
		if(enemiesInCombat.Count > 0)
		{
			HandArea.instance.HandOver(false);
			for(int i = 0; i < enemiesInCombat.Count; i++)
			{
				enemiesInCombat[i].CycleTurnForward();
				enemiesInCombat[i].UpdateEnemyTurn();
			}
			combatAreaLocked = false;
		}
		else
		{
			HandArea.instance.HandOver(true);
			targetReticle.gameObject.SetActive(false);
			LocalAnimations.instance.mo["HandArea"].StartMove("OffScreen");
			t = 0;
			while(t < LocalInterface.instance.animationDuration)
			{
				t += Time.deltaTime;
				yield return null;
			}
		}
	}
	
	public void StartPlayerExecuteAttack(float damage)
	{
		if(playerExectutingAttack)
		{
			StopCoroutine(playerExecuteAttackCoroutine);
		}
		playerExecuteAttackCoroutine = PlayerExecuteAttack(damage);
		StartCoroutine(playerExecuteAttackCoroutine);
	}
	
	private IEnumerator PlayerExecuteAttack(float damage)
	{
		playerExectutingAttack = true;
		playerCharacter.StartMoveAttackJumpBack(new Vector2(targetedEnemy.rt.anchoredPosition.x - targetedEnemy.spriteWidth / 2 - 8f, targetedEnemy.rt.anchoredPosition.y));
		float t = 0;
		while(t < LocalInterface.instance.animationDuration * 0.5f)
		{
			t += Time.deltaTime;
			yield return null;
		}
		targetedEnemy.EnemyTakesDamage(damage);
		t -= LocalInterface.instance.animationDuration * 0.5f;
		while(t < LocalInterface.instance.animationDuration * 0.5f)
		{
			t += Time.deltaTime;
			yield return null;
		}
		playerExectutingAttack = false;
	}
	
	public void StartEnemyExexuteAttack(float damage, Character enemyCharacter)
	{
		if(enemyExectutingAttack)
		{
			StopCoroutine(enemyExecuteAttackCoroutine);
		}
		enemyExecuteAttackCoroutine = EnemyExecuteAttack(damage, enemyCharacter);
		StartCoroutine(enemyExecuteAttackCoroutine);
	}
	
	private IEnumerator EnemyExecuteAttack(float damage, Character enemyCharacter)
	{
		enemyExectutingAttack = true;
		enemyCharacter.StartMoveAttackJumpBack(new Vector2(playerCharacter.rt.anchoredPosition.x + playerCharacter.spriteWidth / 2 + 8f, playerCharacter.rt.anchoredPosition.y));
		float t = 0;
		while(t < LocalInterface.instance.animationDuration * 0.8f)
		{
			t += Time.deltaTime;
			yield return null;
		}
		GameManager.instance.PlayerTakesDamage(damage);
		t -= LocalInterface.instance.animationDuration * 0.8f;
		while(t < LocalInterface.instance.animationDuration * 0.2f)
		{
			t += Time.deltaTime;
			yield return null;
		}
		enemyExectutingAttack = false;
	}
}
