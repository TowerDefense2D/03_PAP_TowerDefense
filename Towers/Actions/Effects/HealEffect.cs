using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Efeito de cura.
/// </summary>
public class HealEffect : TowerSkillEffect
{
	// Prefab do Efeito visual
	public GameObject healFxPrefab;
	// Duração do efeito visual
	public float fxDuration = 1f;

	/// <summary>
	/// Começa esta instância
	/// </summary>
	void Start()
	{
		DefendersSpawner defendersSpawner = tower.GetComponent<DefendersSpawner>();
		if (defendersSpawner != null)
		{
			// Obtém todos os defensores ativos
			foreach (GameObject defender in defendersSpawner.defPoint.GetDefenderList())
			{
				DamageTaker damageTaker = defender.GetComponent<DamageTaker>();
				// Cura
				damageTaker.TakeDamage(-damageTaker.hitpoints);
				if (healFxPrefab != null)
				{
					// Criar efeito visual de cura
					Destroy(Instantiate(healFxPrefab, defender.transform), fxDuration);
				}
			}
		}
		else
		{
			Debug.Log("This tower can not use heal skills");
		}
		Destroy(gameObject);
	}
}
