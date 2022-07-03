using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeEffect : MonoBehaviour
{
	// Nome do efeito
	public string effectName;
	// Modificador de efeito (-1f = -100%, 0f = 0%, 1f = 100%)
	public float modifier = 1f;
	// Duração do efeito
	public float duration = 3f;
	// Raio da área
	public float radius = 1f;
	// Prefab Explosion FX
	public GameObject explosionFx;
	// Explosion FX duração
	public float explosionFxDuration = 1f;
	// Effect FX prefab
	public GameObject effectFx;
	// Efeito sonoro
	public AudioClip sfx;
	// Tags de objetos permitidos para deteção da colisão
	public List<string> tags = new List<string>();

	// A cena está fechada agora. Proibido criar novos objetos ao destruir
	private bool isQuitting;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		EventManager.StartListening("SceneQuit", SceneQuit);
	}

	/// <summary>
	/// Ativa o disable event
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("SceneQuit", SceneQuit);
	}

	/// <summary>
	/// Ativa a aplicação quit event 
	/// </summary>
	void OnApplicationQuit()
	{
		isQuitting = true;
	}

	/// <summary>
	/// Ativa o destroy event
	/// </summary>
	void OnDestroy()
	{
		// Se a cena estiver em andamento
		if (isQuitting == false)
		{
			// Encontra todos os colidores no raio especificado
			Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
			foreach (Collider2D col in cols)
			{
				if (IsTagAllowed(col.tag) == true)
				{
					EffectControl effectControl = col.gameObject.GetComponent<EffectControl>();
					if (effectControl != null)
					{
						effectControl.AddEffect(effectName, modifier, duration, effectFx);
					}
				}
			}
			if (explosionFx != null)
			{
				// Cria efeito visual de uma explosão
				Destroy(Instantiate<GameObject>(explosionFx, transform.position, transform.rotation), explosionFxDuration);
			}
			if (sfx != null && AudioManager.instance != null)
			{
				// Ativa o sfx
				AudioManager.instance.PlaySound(sfx);
			}
		}
	}

	/// <summary>
	/// Ativa a cena quit.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void SceneQuit(GameObject obj, string param)
	{
		isQuitting = true;
	}

	/// <summary>
	/// Determina se esta instância tem permissão de tag para a tag especificada.
	/// </summary>
	/// <returns><c>true</c> se esta instância é tag permitida a tag especificada; senão, <c>false</c>.</returns>
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
}
