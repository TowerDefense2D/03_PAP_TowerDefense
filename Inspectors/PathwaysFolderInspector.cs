using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor da pasta Pathways.
/// </summary>
public class PathwaysFolderInspector : MonoBehaviour
{
	// Prefab para o caminho
	public GameObject pathwayPrefab;
	// Pasta para os caminhos
	public Transform pathwayFolder;
	// Prefab para o ponto de captura
	public GameObject capturePointPrefab;
	// Pasta para pontos de captura
	public Transform capturePointFolder;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		Debug.Assert(pathwayPrefab && pathwayFolder && capturePointPrefab && capturePointFolder, "Wrong stuff settings");
	}

	/// <summary>
	/// Adiciona o caminho.
	/// </summary>
	/// <returns>O caminho.</returns>
	public GameObject AddPathway()
	{
		Pathway[] array = GetComponentsInChildren<Pathway>();
		GameObject res = Instantiate(pathwayPrefab, pathwayFolder).gameObject;
		res.transform.SetAsLastSibling();
		res.name = pathwayPrefab.name + " (" + (array.Length + 1) + ")";
		return res;
	}

	/// <summary>
	/// Obtém o próximo caminho.
	/// </summary>
	/// <returns>O próximo caminho</returns>
	/// <param name="currentSelected">Atual selecionado.</param>
	public GameObject GetNextPathway(GameObject currentSelected)
	{
		return InspectorsUtil<Pathway>.GetNext(pathwayFolder, currentSelected);
	}

	/// <summary>
	/// Obtém o caminho anterior.
	/// </summary>
	/// <returns>caminho anterior.</returns>
	/// <param name="currentSelected">Atual selecionado</param>
	public GameObject GetPrevioustPathway(GameObject currentSelected)
	{
		return InspectorsUtil<Pathway>.GetPrevious(pathwayFolder, currentSelected);
	}

	/// <summary>
	/// Adiciona o ponto de captura.
	/// </summary>
	/// <returns>O ponto de captura</returns>
	public GameObject AddCapturePoint()
	{
		CapturePoint[] array = GetComponentsInChildren<CapturePoint>();
		GameObject res = Instantiate(capturePointPrefab, capturePointFolder).gameObject;
		res.transform.SetSiblingIndex(array.Length);
		res.name = capturePointPrefab.name + " (" + (array.Length + 1) + ")";
		return res;
	}

	/// <summary>
	/// Obtém o próximo ponto de captura.
	/// </summary>
	/// <returns>o próximo ponto de captura</returns>
	/// <param name="currentSelected">Atual selecionado</param>
	public GameObject GetNextCapturePoint(GameObject currentSelected)
	{
		return InspectorsUtil<CapturePoint>.GetNext(capturePointFolder, currentSelected);
	}

	/// <summary>
	/// Obtém o ponto de captura anterior.
	/// </summary>
	/// <returns>o ponto de captura anterior.</returns>
	/// <param name="currentSelected">Atual selecionado</param>
	public GameObject GetPrevioustCapturePoint(GameObject currentSelected)
	{
		return InspectorsUtil<CapturePoint>.GetPrevious(capturePointFolder, currentSelected);
	}
}
#endif
