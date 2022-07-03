using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiTriggerAlone : MonoBehaviour
{
	// Tempo sozinho antes do gatilho

	public float aloneDuration = 1f;
	public List<Component> receivers = new List<Component>();
	// Tags de objetos permitidos para detetar a colisão
	public List<string> tags = new List<string>();

	// O Meu AiBehavior
	private AiBehavior ai;
	// O Meu Colisor 
	private Collider2D col;
	// Contador de duração sozinho
	private float counter;
	// Já acionado
	private bool triggered;

	void Awake()
	{
		ai = GetComponentInParent<AiBehavior>();
		col = GetComponent<Collider2D>();
		Debug.Assert(ai && col, "Wrong initial parameters");
		col.enabled = false;
	}

	void Start()
	{
		col.enabled = true;
		counter = aloneDuration;
		triggered = false;
	}

	/// <summary>
	/// Ativa o evento de trigger stay2d.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject != ai.gameObject && IsTagAllowed(other.tag) == true)
		{
			if (triggered == true)
			{
				foreach (Component receiver in receivers)
				{
					receiver.SendMessage("OnTriggerAloneEnd");
				}
			}
			triggered = false;
			counter = aloneDuration;
		}
	}

	void FixedUpdate()
	{
		if (triggered == false)
		{
			if (counter > 0f)
			{
				counter -= Time.fixedDeltaTime;
			}
			else
			{
				triggered = true;
				counter = 0f;
				foreach (Component receiver in receivers)
				{
					receiver.SendMessage("OnTriggerAloneStart");
				}
			}
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
}
