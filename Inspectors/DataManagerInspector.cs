using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor do gerador de dados.
/// </summary>
public class DataManagerInspector : MonoBehaviour
{
	// Componente do gerador de dados
	private DataManager dataManager;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		dataManager = GetComponent<DataManager>();
		Debug.Assert(dataManager, "Wrong initial settings");
	}

	/// <summary>
	/// Redefine o progresso do jogo.
	/// </summary>
	public void ResetGameProgress()
	{
		dataManager.DeleteGameProgress();
	}

	/// <summary>
	/// Permite o nível pelo seu nome.
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void PermitLevel(string levelName)
	{
		if (dataManager.progress.openedLevels.Contains(levelName) == false)
		{
			dataManager.progress.openedLevels.Add(levelName);
			dataManager.SaveGameProgress();
		}
	}
}
#endif
