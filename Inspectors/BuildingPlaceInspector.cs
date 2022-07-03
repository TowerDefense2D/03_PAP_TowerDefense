using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor de obra.
/// </summary>
public class BuildingPlaceInspector : MonoBehaviour
{
	// Torre deste lugar de construção
	private GameObject myTower;
	// Defend point of this building place
	private GameObject defendPoint;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		defendPoint = GetComponentInChildren<DefendPoint>().gameObject;
		myTower = GetComponentInChildren<Tower>().gameObject;
		Debug.Assert(myTower && defendPoint, "Wrong stuff settings");
	}

	/// <summary>
	/// Obtém o ponto de defesa.
	/// </summary>
	/// <returns>The defend point.</returns>
	public GameObject GetDefendPoint()
	{
		return defendPoint;
	}

	/// <summary>
	/// Escolhe a torre.
	/// </summary>
	/// <returns>A torre.</returns>
	/// <param name="towerPrefab">prefab da Torre</param>
	public GameObject ChooseTower(GameObject towerPrefab)
	{
		// Destroi a torre velha
		if (myTower != null)
		{
			DestroyImmediate(myTower);
		}
		// Cria uma nova torre
		myTower = Instantiate(towerPrefab, transform);
		myTower.name = towerPrefab.name;
		myTower.transform.SetAsLastSibling();
		return myTower;
	}
}
#endif
