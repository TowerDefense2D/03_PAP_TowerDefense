using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ataca com uma arma de corpo a corpo 
/// </summary>
public class AttackMelee : Attack
{
	// Controlador de animação para este AI 
	private Animator anim;
	// Contra para o calcúlo de cooldown 
	private float cooldownCounter;

	/// <summary>
	/// Ativa esta instância 
	/// </summary>
	void Awake()
	{
		anim = GetComponentInParent<Animator>();
		cooldownCounter = cooldown;
	}

	/// <summary>
	/// Atualiza esta instância 
	/// </summary>
	void FixedUpdate()
	{
		if (cooldownCounter < cooldown)
		{
			cooldownCounter += Time.fixedDeltaTime;
		}
	}

	/// <summary>
	/// Ataca o inimigo especifíco se cooldown acabou 
	/// </summary>
	/// <param name="target">Target.</param>
	public override void TryAttack(Transform target)
	{
		if (cooldownCounter >= cooldown)
		{
			cooldownCounter = 0f;
			Fire(target);
		}
	}

	private IEnumerator FireCoroutine(Transform target)
	{
		if (target != null)
		{
			// Se a unidade tiver um animador 
			if (anim != null && anim.runtimeAnimatorController != null)
			{
				// Procurar por um clip 
				foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
				{
					if (clip.name == "Attack")
					{
						// Ativa a animação 
						anim.SetTrigger("attack");
						break;
					}
				}
			}
			// Atraso para sincronizar com a animação 
			yield return new WaitForSeconds(fireDelay);
			if (target != null)
			{
				// Se o alvo conseguir receber dano 
				DamageTaker damageTaker = target.GetComponent<DamageTaker>();
				if (damageTaker != null)
				{
					damageTaker.TakeDamage(damage);
				}
				// Imprime o sound effect 
				if (sfx != null && AudioManager.instance != null)
				{
					AudioManager.instance.PlayAttack(sfx);
				}
			}
		}
	}

	/// <summary>
	/// Criar o ataque corpo a corpo 
	/// </summary>
	/// <param name="target">Target.</param>
	public override void Fire(Transform target)
	{
		StartCoroutine(FireCoroutine(target));
	}

	void OnDestroy()
	{
		StopAllCoroutines();
	}
}
