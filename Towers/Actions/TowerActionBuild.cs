using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Constroi a torre.
/// </summary>
public class TowerActionBuild : TowerAction
{
    // prefab da torre fabricada para este icon
    public GameObject towerPrefab;
	// Ícon para estado desabilitado
	public GameObject disabledIcon;
	// Ícon para estado bloqueado enquanto o jogador não tem ouro suficiente
	public GameObject blockedIcon;

	// Campo de texto para o preço da torre
	private Text priceText;
	// Preço da torre em ouro
	private int price = 0;
	// O gerador de níveís tem uma lista com upgrades de torre permitidos para este nível.
	private LevelManager levelManager;
	// O gerador de interface do utilizador permite verificar a quantidade de ouro atual
	private UiManager uiManager;

    /// <summary>
    /// Ativa esta instância
    /// </summary>
    void Awake()
    {
        priceText = GetComponentInChildren<Text>();
		levelManager = FindObjectOfType<LevelManager>();
		uiManager = FindObjectOfType<UiManager>();
		Debug.Assert(priceText && towerPrefab && enabledIcon && disabledIcon && levelManager && uiManager, "Wrong initial parameters");
		// Preço da torre de exibição
		price = towerPrefab.GetComponent<Price>().price;
		priceText.text = price.ToString();
		if (levelManager.allowedTowers.Contains(towerPrefab) == true)
		{
			enabledIcon.SetActive(true);
			disabledIcon.SetActive(false);
		}
		else
		{
			enabledIcon.SetActive(false);
			disabledIcon.SetActive(true);
		}
    }

	/// <summary>
	/// Atualiza esta instância
	/// </summary>
	void Update()
	{
		// Ícon de construção de máscara que bloqueia o ícon se o jogador não tiver ouro suficiente
		if (enabledIcon == true && blockedIcon != null)
		{
			if (uiManager.GetGold() >= price)
			{
				blockedIcon.SetActive(false);
			}
			else
			{
				blockedIcon.SetActive(true);
			}
		}
	}

	/// <summary>
	/// Clica nesta instância
	/// </summary>
	protected override void Clicked()
	{
		// Se o jogador tiver ouro suficiente
		if (blockedIcon == null || blockedIcon.activeSelf == false)
		{
			// Constroi a torre
			Tower tower = GetComponentInParent<Tower>();
			if (tower != null)
			{
				tower.BuildTower(towerPrefab);
			}
		}
	}
}
