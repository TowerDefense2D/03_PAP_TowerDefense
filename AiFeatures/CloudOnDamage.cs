using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Crie exaustão em caso de danos e cobre todas as unidades mais próximas em nuvens.
/// </summary>
public class CloudOnDamage : MonoBehaviour
{
	// Duração das nuvens
	public float duration = 3f;
	// Resfriamento entre a exaustão
	public float cooldown = 5f;
	// Raio da exaustão
	public float radius = 1f;
	// Prefab das nuvens
	public Clouded cloudPrefab;
	// Efeito visual de escape
	public GameObject exhaustFX;
	// Tags de objetos permitidos para deteção de colisão
	public List<string> tags = new List<string>();

	// Estado da máquina
	private enum MyState
	{
		WaitForDamage,
		Clouded,
		Cooldown
	}
	// Estado atual desta instância
	private MyState myState = MyState.WaitForDamage;
	// Contador de duração do cooldown
	private float counter;

	// (Para mim próprio) Utiliza isso para inicialização
	void Start()
	{
		Debug.Assert(cloudPrefab && exhaustFX, "Wrong initial settings");
		counter = 0f;
	}

	/// <summary>
	/// Atualiza esta instância.
	/// </summary>
	void Update()
	{
		switch (myState)
		{
		case MyState.Cooldown:  // Aguarde o fim do cooldown
				if (counter > 0f)
			{
				counter -= Time.deltaTime;
			}
			else
			{
				counter = 0f;
				myState = MyState.WaitForDamage;
			}
			break;
		case MyState.Clouded:	// Faz uma nuvem para ficar invisível todas as unidades mais próximas
			if (counter > 0f)
			{
				counter -= Time.deltaTime;
			}
			else
			{
				counter = cooldown;
				myState = MyState.Cooldown;
			}
			break;
		}
	}

	/// <summary>
	/// Ativa o evento de dano (do script DamageTaker se definido como gatilho)
	/// </summary>
	public void OnDamage()
	{
		// Se é permitido estado Now
		if (myState == MyState.WaitForDamage)
		{
			myState = MyState.Clouded;
			counter = duration;
			CloudNow();
			// Cria efeito visual
			GameObject obj = Instantiate(exhaustFX, transform);
			Destroy(obj, duration);
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
	/// Cobre as unidades mais próximas em nuvens
	/// </summary>
	private void CloudNow()
	{
		// Procura por unidades
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
		foreach (Collider2D col in cols)
		{
			if (IsTagAllowed(col.tag) == true)
			{
				// Adiciona nuvem à unidade
				Clouded clouded = Instantiate(cloudPrefab, col.gameObject.transform);
				clouded.duration = duration;
			}
		}
	}
}
