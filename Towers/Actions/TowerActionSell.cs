using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vende a torre.
/// </summary>
public class TowerActionSell : TowerAction
{
	// Prefab para local de construção vazio
	public GameObject emptyPlacePrefab;

	/// <summary>
	/// Ativa esta instância
	/// </summary>
	void Awake()
	{
		Debug.Assert(emptyPlacePrefab, "Wrong initial parameters");
	}

	protected override void Clicked()
	{
		// Vende a torre
		Tower tower = GetComponentInParent<Tower>();
		if (tower != null)
		{
			tower.SellTower(emptyPlacePrefab);
		}
	}
}
