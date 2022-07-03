using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Feitiço como meteorito, starburst e assim por diante
/// </summary>
public class AirStrike : MonoBehaviour
{
	// Atrasos para FX
	public float[] delaysBeforeDamage = {0.5f};
	// Dano por cada hit
	public int damage = 5;
	// Raio de dano
	public float radius = 1f;
	// FX prefab
	public GameObject effectPrefab;
	// Após este tempo limite, o FX será destruído
	public float effectDuration = 2f;
	// Efeito sonoro
	public AudioClip sfx;

	// Estado da máquina
	private enum MyState
	{
		WaitForClick,
		WaitForFX
	}
	// Estado atual para esta instância
	private MyState myState = MyState.WaitForClick;

	/// <summary>
	/// Ativa o enable event 
	/// </summary>
	void OnEnable()
	{
		EventManager.StartListening("UserClick", UserClick);
		EventManager.StartListening("UserUiClick", UserUiClick);
	}

	/// <summary>
	/// Ativa o disable event 
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("UserClick", UserClick);
		EventManager.StopListening("UserUiClick", UserUiClick);
	}

	/// <summary>
	/// Começa esta instância 
	/// </summary>
	void Start()
	{
		Debug.Assert(effectPrefab, "Wrong initial settings");
	}

	/// <summary>
	/// Manipulador de cliques do usuário.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UserClick(GameObject obj, string param)
	{
		if (myState == MyState.WaitForClick)
		{
			// Se não for clicado na torre
			if (obj == null || obj.CompareTag("Tower") == false)
			{
				// Criar FX
				transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
				GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
				Destroy(effect, effectDuration);
				EventManager.TriggerEvent("ActionStart", gameObject, null);
				// Iniciaa a corrotina damage
				StartCoroutine(DamageCoroutine());
				myState = MyState.WaitForFX;
			}
			else // Clicou na torre
			{
				Destroy(gameObject);
			}
		}
	}

	/// <summary>
	/// Manipulador de cliques da interface do usuário.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UserUiClick(GameObject obj, string param)
	{
		// Se clicado na interface do utilizador, em vez do mapa do jogo
		if (myState == MyState.WaitForClick)
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Faz danos especificados por atrasos e quantidade.
	/// </summary>
	/// <returns>A corrotina.</returns>
	private IEnumerator DamageCoroutine()
	{
		foreach (float delay in delaysBeforeDamage)
		{
			yield return new WaitForSeconds(delay);

			// Pesquisaa alvos
			Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
			foreach (Collider2D col in hits)
			{
				if (col.CompareTag("Enemy") == true || col.CompareTag("FlyingEnemy") == true)
				{
					DamageTaker damageTaker = col.GetComponent<DamageTaker>();
					if (damageTaker != null)
					{
						damageTaker.TakeDamage(damage);
					}
				}
			}
			if (sfx != null && AudioManager.instance != null)
			{
				// Reproduz efeito sonoro
				AudioManager.instance.PlaySound(sfx);
			}
		}

		Destroy(gameObject);
	}

	/// <summary>
	/// Ativa o destroy event.
	/// </summary>
	void OnDestroy()
	{
		if (myState == MyState.WaitForClick)
		{
			EventManager.TriggerEvent("ActionCancel", gameObject, null);
		}
		StopAllCoroutines();
	}
}
