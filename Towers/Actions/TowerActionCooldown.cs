using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ação da torre com cooldown.
/// </summary>
public class TowerActionCooldown : TowerAction
{
	// Tempo de espera da ação
	public float cooldown = 10f;
	// Ícon para estado de espera
	public GameObject cooldownIcon;
	// cooldown (texto da interface do usuário)
	public Text cooldownText;

	// Estado da máquina
	private enum MyState
	{
		Active,
		Cooldown
	}
	// Estado atual para esta instância
	private MyState myState = MyState.Active;
	// Hora em que o cooldown foi iniciado
	private float cooldownStartTime;

	/// <summary>
	/// Ativa esta instância
	/// </summary>
	void Awake()
	{
		Debug.Assert(cooldownIcon && cooldownText, "Wrong initial settings");
		StopCooldown();
	}

	/// <summary>
	/// Atualiza esta instância
	/// </summary>
	void Update()
	{
		if (myState == MyState.Cooldown)
		{
			float cooldownCounter = Time.time - cooldownStartTime;
			if (cooldownCounter < cooldown)
			{
				UpdateCooldownText(cooldown - cooldownCounter);
			}
			else
			{
				StopCooldown();
			}
		}
	}

	/// <summary>
	/// Começa o cooldown.
	/// </summary>
	private void StartCooldown()
	{
		myState = MyState.Cooldown;
		cooldownStartTime = Time.time;
		enabledIcon.SetActive(false);
		cooldownIcon.gameObject.SetActive(true);
		cooldownText.gameObject.SetActive(true);
	}

	/// <summary>
	/// Para o cooldown.
	/// </summary>
	private void StopCooldown()
	{
		myState = MyState.Active;
		enabledIcon.SetActive(true);
		cooldownIcon.gameObject.SetActive(false);
		cooldownText.gameObject.SetActive(false);
	}

	/// <summary>
	/// Atualiza o texto do cooldown
	/// </summary>
	private void UpdateCooldownText(float cooldownCounter)
	{
		cooldownText.text = ((int)Mathf.Ceil(cooldownCounter)).ToString();
	}

	/// <summary>
	/// Clica nesta instância
	/// </summary>
	protected override void Clicked()
	{
		StartCooldown();
	}
}
