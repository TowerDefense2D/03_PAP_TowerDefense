using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor de gerador de nível.
/// </summary>
public class LevelManagerInspector : MonoBehaviour
{
	[HideInInspector]
	// Lista com todos os prefabs dos inimigos
	public List<GameObject> enemiesList = new List<GameObject>();

	[HideInInspector]
	// Lista de inimigos para este nível
	public List<GameObject> enemies
	{
		get
		{
			return levelManager.allowedEnemies;
		}
		set
		{
			levelManager.allowedEnemies = value;
		}
	}

	[HideInInspector]
	// Lista com todas os prefabs das torres
	public List<GameObject> towersList = new List<GameObject>();

	[HideInInspector]
	// Lista de torres para este nível
	public List<GameObject> towers
	{
		get
		{
			return levelManager.allowedTowers;
		}
		set
		{
			levelManager.allowedTowers = value;
		}
	}

	[HideInInspector]
	// Lista com todos os prefabs de feitiços
	public List<GameObject> spellsList = new List<GameObject>();

	[HideInInspector]
	// Lista de feitiços para este nível
	public List<GameObject> spells
	{
		get
		{
			return levelManager.allowedSpells;
		}
		set
		{
			levelManager.allowedSpells = value;
		}
	}

	[HideInInspector]
	// Starting gold amount for this level
	public int goldAmount
	{
		get
		{
			return levelManager.goldAmount;
		}
		set
		{
			levelManager.goldAmount = value;
		}
	}

	[HideInInspector]
	//tentativas de derrota antes de perder para este nível
	public int defeatAttempts
	{
		get
		{
			return levelManager.defeatAttempts;
		}
		set
		{
			levelManager.defeatAttempts = value;
		}
	}

	// Componente do gerador de nível
	private LevelManager levelManager;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		levelManager = GetComponent<LevelManager>();
		Debug.Assert(levelManager, "Wrong stuff settings");
	}
}
#endif
