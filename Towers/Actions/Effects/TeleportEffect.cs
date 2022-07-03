using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Teletransporta o alvo de volta ao caminho.
/// </summary>
public class TeleportEffect : TowerSkillEffect
{
	// Deslocamento no caminho
	public float teleportOffset = 1f;
	// Efeito visual
	public GameObject fxPrefab;
	// Duração do efeito visual
	public float fxDuration = 3f;

	/// <summary>
	/// Começa esta instância.
	/// </summary>
	void Start()
	{
		float radius = radiusCollider.radius * Mathf.Max(radiusCollider.transform.localScale.x, radiusCollider.transform.localScale.y);
		// Obtém um inimigo aleatório
		Collider2D enemy = Physics2D.OverlapCircle(radiusCollider.transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));
		if (enemy != null)
		{
			if (fxPrefab != null)
			{
				// Efeito visual na posição de teletransporte
				GameObject fx = Instantiate(fxPrefab);
				fx.transform.position = enemy.gameObject.transform.position;
				Destroy(fx, fxDuration);
			}
			AiStatePatrol aiStatePatrol = enemy.gameObject.GetComponent<AiStatePatrol>();
			// Desloca o alvo no caminho
			Vector2 teleportPosition = aiStatePatrol.path.GetOffsetPosition(ref aiStatePatrol.destination, aiStatePatrol.transform.position, teleportOffset);
			aiStatePatrol.transform.position = new Vector3(teleportPosition.x, teleportPosition.y, aiStatePatrol.transform.position.z);
			// Atualiza o destino da patrulha
			aiStatePatrol.UpdateDestination(false);
			if (fxPrefab != null)
			{
				// Efeito visual numa nova posição
				GameObject fx = Instantiate(fxPrefab);
				fx.transform.position = enemy.gameObject.transform.position;
				Destroy(fx, fxDuration);
			}
		}
		Destroy(gameObject);
	}
}
