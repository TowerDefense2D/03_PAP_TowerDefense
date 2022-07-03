using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cura um alvo aleatório no intervalo especificado (collider)
/// </summary>
public class Heal : AiFeature
{
	// Quantidade de hp (Health Points ;) ) curado
	public int healAmount = 1;
	// Cooldown entre a cura
	public float cooldown = 3f;
	// Efeito visual para a cura
	public GameObject healVisualPrefab;
	// Duração do efeito visual de cura
	public float healVisualDuration = 1f;
	// Tags de objetos permitidos para deteção da colisão
	public List<string> tags = new List<string>();

	// Contador para o cooldown
	private float cooldownCounter;

	/// <summary>
	/// Começa esta instância
	/// </summary>
	void Start()
	{
		cooldownCounter = cooldown;
	}

	/// <summary>
	/// Corrige a atualização.
	/// </summary>
	void FixedUpdate()
	{
		if (cooldownCounter < cooldown)
		{
			cooldownCounter += Time.fixedDeltaTime;
		}
	}

	/// <summary>
	/// Determina se esta instância tem permissão de tag para a tag especificada.
	/// </summary>
	/// <returns><c>true</c> se esta instância é tag permitida a tag especificada; senão <c>false</c>.</returns>
	/// <param name="tag">Tag.</param>
	private bool IsTagAllowed(string tag)
	{
		bool res = false;
		if (tags.Count > 0)
		{
			foreach (string str in tags)
			{
				if (str == tag)
				{
					res = true;
					break;
				}
			}
		}
		else
		{
			res = true;
		}
		return res;
	}

	/// <summary>
	/// Cura o alvo especificado se o tempo de espera expirou.
	/// </summary>
	/// <param name="target">Alvo</param>
	private void TryToHeal(DamageTaker target)
	{
		// Se o cooldown expirou
		if (cooldownCounter >= cooldown)
		{
			cooldownCounter = 0f;
			target.TakeDamage(-healAmount);
			if (healVisualPrefab != null)
			{
				// Cria efeito de cura visual no alvo
				GameObject effect = Instantiate(healVisualPrefab, target.transform);
				// E destruí-lo após o tempo limite especificado
				Destroy(effect, healVisualDuration);
			}
		}
	}

	/// <summary>
	/// Ativa o evento de trigger stay2d
	/// </summary>
	/// <param name="other">Outro</param>
	void OnTriggerStay2D(Collider2D other)
	{
		if (IsTagAllowed(other.tag) == true)
		{
			// Se tiver o componente Damage Taker (Recetor de Dano)
			DamageTaker target = other.gameObject.GetComponent<DamageTaker>();
			if (target != null)
			{
				// Se o alvo for ferido
				if (target.currentHitpoints < target.hitpoints)
				{
					TryToHeal(target);
				}
			}
		}
	}
}
