using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor de caminho.
/// </summary>
public class PathwayInspector : MonoBehaviour
{
	// Ponto de spawn deste caminho
	public SpawnPoint spawnPoint;
	// Prefab para o ponto
	public Waypoint waypointPrefab;
	// Pasta de waypoint para este caminho
	public Transform waypointsFolder;
	// Deslocamento do waypoint quando colocado na cena
	public Vector2 offset = new Vector2(1f, 0f);

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		Debug.Assert(spawnPoint && waypointPrefab && waypointsFolder, "Wrong stuff settings");
	}

	/// <summary>
	/// Obtém o ponto de spawn.
	/// </summary>
	/// <returns>O ponto de spawn</returns>
	public GameObject GetSpawnPoint()
	{
		return spawnPoint.gameObject;
	}

	/// <summary>
	/// Adiciona o ponto de passagem.
	/// </summary>
	/// <returns>O ponto de passagem</returns>
	public GameObject AddWaypoint()
	{
		Waypoint[] array = GetComponentsInChildren<Waypoint>();
		GameObject res = Instantiate(waypointPrefab, waypointsFolder).gameObject;
		res.transform.SetAsLastSibling();
		res.name = waypointPrefab.name + " (" + (array.Length + 1) + ")";
		if (array.Length > 0)
		{
			res.transform.position = array[array.Length - 1].transform.position + (Vector3)offset;
		}
		else
		{
			res.transform.position += (Vector3)offset;
		}
		return res;
	}

	/// <summary>
	/// Obtém o próximo waypoint.
	/// </summary>
	/// <returns>O próximo waypoint</returns>
	/// <param name="currentSelected">Atual selecionado</param>
	public GameObject GetNextWaypoint(GameObject currentSelected)
	{
		return InspectorsUtil<Waypoint>.GetNext(waypointsFolder, currentSelected);
	}

	/// <summary>
	/// Obtém o waypoint anterior.
	/// </summary>
	/// <returns>O waypoint anterior.</returns>
	/// <param name="currentSelected">Current selected.</param>
	public GameObject GetPrevioustWaypoint(GameObject currentSelected)
	{
		return InspectorsUtil<Waypoint>.GetPrevious(waypointsFolder, currentSelected);
	}
}
#endif
