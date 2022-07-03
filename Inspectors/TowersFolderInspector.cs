using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor da pasta Towers.
/// </summary>
public class TowersFolderInspector : MonoBehaviour
{
	// Prefab do local de construção
	public GameObject towerPrefab;
	// Pasta para as torres
	public Transform towerFolder;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		Debug.Assert(towerPrefab && towerFolder, "Wrong stuff settings");
	}

	/// <summary>
	/// Adiciona a torre
	/// </summary>
	/// <returns>A torre.</returns>
	public GameObject AddTower()
	{
		int towerCount = FindObjectsOfType<Tower>().Length;
		GameObject tower = Instantiate(towerPrefab, towerFolder);
		tower.name = towerPrefab.name;
		if (towerCount > 0)
		{
			tower.name += " (" + towerCount.ToString() + ")";
		}
		tower.transform.SetAsLastSibling();
		return tower;
	}

	/// <summary>
	/// Obtém o próximo local de construção.
	/// </summary>
	/// <returns>próximo local de construção.</returns>
	/// <param name="currentSelected">Atual selecionado.</param>
	public GameObject GetNextBuildingPlace(GameObject currentSelected)
	{
		return InspectorsUtil<BuildingPlace>.GetNext(towerFolder, currentSelected);
	}

	/// <summary>
	/// Obtém o local de construção anterior.
	/// </summary>
	/// <returns>O local de construção anterior.</returns>
	/// <param name="currentSelected">Atual selecionado.</param>
	public GameObject GetPrevioustBuildingPlace(GameObject currentSelected)
	{
		return InspectorsUtil<BuildingPlace>.GetPrevious(towerFolder, currentSelected);
	}
}
#endif
