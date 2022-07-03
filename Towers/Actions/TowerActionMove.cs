using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ação da torre para os defensores em movimento.
/// </summary>
public class TowerActionMove : TowerAction
{
	// Defende o ponto desta torre
	private DefendPoint defendPoint;
	// Torre
	private Tower tower;

	/// <summary>
	/// Começa esta instância
	/// </summary>
	void Start()
	{
		BuildingPlace buildingPlace = GetComponentInParent<BuildingPlace>();
		defendPoint = buildingPlace.GetComponentInChildren<DefendPoint>();
		tower = GetComponentInParent<Tower>();
		Debug.Assert(defendPoint && tower, "Wrong initial settings");
	}

	/// <summary>
	/// Clica nesta instância
	/// </summary>
	protected override void Clicked()
	{
		defendPoint.SetVisible(true);
		tower.ShowRange(true);
	}
}
