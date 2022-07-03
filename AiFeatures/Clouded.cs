using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unidade obtém uma nuvem que o torna invisível
/// </summary>
public class Clouded : MonoBehaviour
{
	[HideInInspector]
	// Duração da nuvem
	public float duration;

	// Componente do colisor deste objeto no jogo
	private Collider2D col;
	// Counter for clouded duration
	private float counter;

	/// <summary>
	/// Começa esta instância.
	/// </summary>
	void Start()
	{
		col = GetComponentInParent<Collider2D>();
		Debug.Assert(col, "Wrong initial settings");
		counter = duration;
		// Torna a unidade invisível (não colide com outras unidades)
		col.enabled = false;
	}

	/// <summary>
	/// Corrige a atualização.
	/// </summary>
	void FixedUpdate()
	{
		if (counter > 0f)
		{
			counter -= Time.fixedDeltaTime;
		}
		else
		{
			// Torna a unidade visível
			col.enabled = true;
			Destroy(gameObject);
		}
	}
}
