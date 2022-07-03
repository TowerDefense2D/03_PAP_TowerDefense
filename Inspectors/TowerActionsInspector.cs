using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor de ações da torre.
/// </summary>
public class TowerActionsInspector : MonoBehaviour
{
	// Distância do centro
	public float radialDistance = 1.4f;
	// Deslocamento do ângulo inicial
	public float angleOffset = 180f;
	// Lista com ações da torre
	public List<TowerAction> actions = new List<TowerAction>();

	/// <summary>
	/// Atualiza esta instância
	/// </summary>
	void Update()
	{
		// Atualizar ações das torres
		actions.Clear();
		foreach (TowerAction action in GetComponentsInChildren<TowerAction>())
		{
			actions.Add(action);
		}
		PlaceAround();
	}

	/// <summary>
	/// Coloca as ações da torre ao redor da árvore de construção.
	/// </summary>
	private void PlaceAround()
	{
		float deltaAngle = 360f / actions.Count;
		for (int i = 0; i < actions.Count; ++i)
		{
			float radians = (angleOffset + i * deltaAngle) * Mathf.Deg2Rad;
			actions[i].transform.localPosition = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians)) * radialDistance;
		}
	}

	public GameObject AddAction(GameObject actionPrefab)
	{
		GameObject res = Instantiate(actionPrefab, transform);
		res.name = actionPrefab.name;
		Update();
		return res;
	}

	public GameObject GetNextAction(GameObject currentSelected)
	{
		return InspectorsUtil<TowerAction>.GetNext(transform, currentSelected);
	}

	public GameObject GetPrevioustAction(GameObject currentSelected)
	{
		return InspectorsUtil<TowerAction>.GetPrevious(transform, currentSelected);
	}
}
#endif
