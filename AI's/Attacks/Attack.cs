using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe básica para os tipos de ataque 
/// </summary>
public class Attack : MonoBehaviour
{
	// Quantidade de dano 
	public int damage = 1;
	// Tempo entre os ataques 
	public float cooldown = 1f;
	// Atraso na animação de fogo 
	public float fireDelay = 0f;
	// Efeito de som 
	public AudioClip sfx;

	public virtual void TryAttack(Transform target)
	{

	}

	public virtual void Fire(Transform target)
	{

	}
}
