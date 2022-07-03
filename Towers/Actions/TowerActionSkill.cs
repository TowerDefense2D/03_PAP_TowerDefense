using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Habilidade ativa da torre.
/// </summary>
public class TowerActionSkill : TowerActionCooldown
{
	// Prefab para efeito da habilidade
	public TowerSkillEffect effectPrefab;
	// Raio
	[HideInInspector]
	public CircleCollider2D radiusCollider;

	// Torre
	private Tower tower;

	/// <summary>
	/// Ativa esta instância
	/// </summary>
	void Awake()
	{
		tower = GetComponentInParent<Tower>();
		Debug.Assert(effectPrefab && tower, "Wrong initial settings");
	}

	/// <summary>
	/// Clica esta instância.
	/// </summary>
	protected override void Clicked()
	{
		base.Clicked();
		// Aplica efeito de habilidade
		TowerSkillEffect towerSkillEffect = Instantiate(effectPrefab);
		towerSkillEffect.tower = tower;
		AttackRanged attackRanged = tower.GetComponentInChildren<AttackRanged>();
		if (attackRanged != null)
		{
			towerSkillEffect.radiusCollider = attackRanged.GetComponent<CircleCollider2D>();
		}
		else if (tower.range != null)
		{
			towerSkillEffect.radiusCollider = tower.range.GetComponent<CircleCollider2D>();
		}
	}
}
