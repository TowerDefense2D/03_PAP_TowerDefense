using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Efeito de ataque especial. Ataca um inimigo aleatório no raio.
/// </summary>
public class SpecialStrikeEffect : TowerSkillEffect
{
	// Prefab da bala
	public GameObject bulletPrefab;

	/// <summary>
	/// Começa esta instância
	/// </summary>
	void Start()
	{
		Debug.Assert(bulletPrefab, "Wrong initial settings");
		AttackRanged attack = tower.GetComponentInChildren<AttackRanged>();
		if (attack != null)
		{
			float radius = radiusCollider.radius * Mathf.Max(radiusCollider.transform.localScale.x, radiusCollider.transform.localScale.y);
			// Obtém um inimigo aleatório
			Collider2D enemy = Physics2D.OverlapCircle(radiusCollider.transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));
			if (enemy != null)
			{
				GameObject defaultBulletPrefab = attack.arrowPrefab;
				attack.arrowPrefab = bulletPrefab;
				// Ataca-o
				attack.Fire(enemy.gameObject.transform);
				attack.arrowPrefab = defaultBulletPrefab;
			}
		}
		else
		{
			Debug.Log("This tower can not use attack skills");
		}
		Destroy(gameObject);
	}
}
