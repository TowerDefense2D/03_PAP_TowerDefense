using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cura todas as unidades no raio especificado
/// </summary>
public class AoeHeal : AiFeature
{
	// Quantidade de hp curado
	public int healAmount = 1;
	// Raio de cura
	public CircleCollider2D radius;
	// Intervalo entre a cura
	public float cooldown = 3f;
	// Efeito visual para cura
	public GameObject healVisualPrefab;
	// Duração do efeito visual de cura
	public float healVisualDuration = 1f;
	// Tags de objetos permitidos para deteção de colisão
	public List<string> tags = new List<string>();

	// Contador de cooldown
	private float cooldownCounter;
	// Componente de animação
	private Animator anim;

	/// <summary>
	/// Começa esta instância.
	/// </summary>
	void Start()
	{
		Debug.Assert(radius, "Wrong initial settings");
		anim = GetComponentInParent<Animator>();
		cooldownCounter = cooldown;
		radius.enabled = false;
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
		else
		{
			cooldownCounter = 0f;
			// Tente curar alguém
			if (Heal() == true)
			{
				if (anim != null && anim.runtimeAnimatorController != null)
				{
					// Reproduz animação
					foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
					{
						if (clip.name == "Special")
						{
							anim.SetTrigger("special");
							break;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Determines whether this instance is tag allowed the specified tag.
	/// </summary>
	/// <returns><c>true</c> if this instance is tag allowed the specified tag; otherwise, <c>false</c>.</returns>
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
	/// Heal all targets in radius.
	/// </summary>
	private bool Heal()
	{
		bool res = false;
		// Searching for units
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius.radius * transform.localScale.x);
		foreach (Collider2D col in cols)
		{
			if (IsTagAllowed(col.tag) == true)
			{
				// If it has Damege Taker component
				DamageTaker target = col.gameObject.GetComponent<DamageTaker>();
				if (target != null)
				{
					// If target injured
					if (target.currentHitpoints < target.hitpoints)
					{
						res = true;
						target.TakeDamage(-healAmount);
						if (healVisualPrefab != null)
						{
							// Create visual healing effect on target
							GameObject effect = Instantiate(healVisualPrefab, target.transform);
							// And destroy it after specified timeout
							Destroy(effect, healVisualDuration);
						}
					}
				}
			}
		}
		return res;
	}
}
