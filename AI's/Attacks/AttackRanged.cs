using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ataque com arma de alcançe 
/// </summary>
public class AttackRanged : Attack
{
	// Prefab para as setas 
	public GameObject arrowPrefab;
	// Desta posição as setas serão atiradas 
	public Transform firePoint;

	// Controlador da animação para este AI 
	private Animator anim;
	// Contra para o calcúlo do cooldown 
	private float cooldownCounter;

	/// <summary>
	/// Ativa esta instância 
	/// </summary>
	void Awake()
	{
		anim = GetComponentInParent<Animator>();
		cooldownCounter = cooldown;
		Debug.Assert(arrowPrefab && firePoint, "Wrong initial parameters");
	}

	/// <summary>
	/// Atualiza a instância. 
	/// </summary>
	void FixedUpdate()
	{
		if (cooldownCounter < cooldown)
		{
			cooldownCounter += Time.fixedDeltaTime;
		}
	}

	/// <summary>
	/// Ataca o alvo especifíco se o cooldown for expirado 
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

	private IEnumerator FireCoroutine(Transform target, GameObject bulletPrefab)
	{
		if (target != null && bulletPrefab != null)
		{
			// Se a unidade for animada
			if (anim != null && anim.runtimeAnimatorController != null)
			{
				// Procurar por clip 
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
				// Cria uma seta 
				GameObject arrow = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
				IBullet bullet = arrow.GetComponent<IBullet>();
				bullet.SetDamage(damage);
				bullet.Fire(target);
				// Ativa o som 
				if (sfx != null && AudioManager.instance != null)
				{
					AudioManager.instance.PlayAttack(sfx);
				}
			}
		}
	}

	/// <summary>
	/// Cria um ataque de alcançe 
	/// </summary>
	/// <param name="target">Target.</param>
	public override void Fire(Transform target)
	{
		StartCoroutine(FireCoroutine(target, arrowPrefab));
	}

	/// <summary>
	/// Cria um ataque de alcançe com uma bala especial 
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="bulletPrefab">Bullet prefab.</param>
	public void Fire(Transform target, GameObject bulletPrefab)
	{
		StartCoroutine(FireCoroutine(target, bulletPrefab));
	}

	void OnDestroy()
	{
		StopAllCoroutines();
	}
}
