using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor da escolha do nível
/// </summary>
public class LevelChooseInspector : MonoBehaviour
{
	// Descrição do nível
	public Transform levelFolder;

	// Componente selecionador do nível
	private LevelChoose levelChooser;

	/// <summary>
	/// Ativa o enable event.
	/// </summary>
	void OnEnable()
	{
		levelChooser = GetComponent<LevelChoose>();
		Debug.Assert(levelFolder && levelChooser, "Wrong initial settings");
		// Exclui todos os prefabs ausentes da lista
		levelChooser.levelsPrefabs.RemoveAll(GameObject => GameObject == null);
	}

	public void AddLevel(GameObject levelPrefab)
	{
		if (levelPrefab != null)
		{
			// Adicionar à lista de níveis permitidos
			if (levelChooser.levelsPrefabs.Contains(levelPrefab) == false)
			{
				levelChooser.levelsPrefabs.Add(levelPrefab);
			}
		}
	}

	/// <summary>
	/// Define o nível ativo.
	/// </summary>
	/// <param name="levelPrefab">prefab do Nível</param>
	public void SetActiveLevel(GameObject level)
	{
		LevelDescriptionInspector oldLevel = levelFolder.GetComponentInChildren<LevelDescriptionInspector>();
		// Destroi a descrição do nível antigo
		if (oldLevel != null)
		{
			DestroyImmediate(oldLevel.gameObject);
		}
		// Atualizaa a descrição do nível
		level.transform.SetParent(levelFolder, false);
		level.transform.SetAsFirstSibling();
		levelChooser.currentLevel = level;
	}

	public List<GameObject> GetLevelPrefabs()
	{
		return levelChooser.levelsPrefabs;
	}
}
#endif
