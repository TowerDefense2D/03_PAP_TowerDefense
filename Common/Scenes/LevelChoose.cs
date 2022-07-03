using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// gerador da escolha do nível da cena
/// </summary>
public class LevelChoose : MonoBehaviour
{
	// Cena para sair
	public string exitSceneName;
	// Exibição visual para o número de níveis
	public Transform togglesFolder;
	// Active toggle prefab
	public Toggle activeTogglePrefab;
	// Inactive toggle prefab
	public Toggle inactiveTogglePrefab;
	// Botão do próximo nível
	public Button nextLevelButton;
	// Botão do nível anterior
	public Button prevLevelButton;
	// Pasta para a visualização de nível
	public Transform levelFolder;
	// Nível escolhido
	public GameObject currentLevel;
	// Todos os níveis
	public List<GameObject> levelsPrefabs = new List<GameObject>();

	// Índice do último nível permitido para escolha
	private int maxActiveLevelIdx;
	// Índice do nível atual exibido
	private int currentDisplayedLevelIdx;
	// Lista com os toggles ativas
	private List<Toggle> activeToggles = new List<Toggle>();

	/// <summary>
	/// Ativa o enable event. 
	/// </summary>
	void OnEnable()
	{
		EventManager.StartListening("ButtonPressed", ButtonPressed);
	}

	/// <summary>
	/// Ativa o disable event. 
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("ButtonPressed", ButtonPressed);
	}

	/// <summary>
	/// Desperta esta instância.
	/// </summary>
	void Awake()
	{
		maxActiveLevelIdx = -1;
		Debug.Assert(currentLevel && togglesFolder && activeTogglePrefab && inactiveTogglePrefab && nextLevelButton && prevLevelButton && levelFolder, "Wrong initial settings");
	}

	/// <summary>
	/// Começa esta instância.
	/// </summary>
	void Start()
    {
		int hitIdx = -1;
		int levelsCount = DataManager.instance.progress.openedLevels.Count;
		if (levelsCount > 0)
		{
			// Obtém o nome do último nível aberto dos dados armazenados
			string openedLevelName = DataManager.instance.progress.openedLevels[levelsCount - 1];

	        int idx;
			for (idx = 0; idx < levelsPrefabs.Count; ++idx)
	        {
				// Tenta encontrar o último nível aberto na lista de níveis
				if (levelsPrefabs[idx].name == openedLevelName)
	            {
	                hitIdx = idx;
	                break;
	            }
	        }
		}
		// Nível encontrado
		if (hitIdx >= 0)
		{
			if (levelsPrefabs.Count > hitIdx + 1)
			{
				maxActiveLevelIdx = hitIdx + 1;
			}
			else
			{
				maxActiveLevelIdx = hitIdx;
			}
		}
		// nível não encontrado
		else
		{
			if (levelsPrefabs.Count > 0)
			{
				maxActiveLevelIdx = 0;
			}
			else
			{
				Debug.LogError("Have no levels prefabs!");
			}
		}
		if (maxActiveLevelIdx >= 0)
		{
			DisplayToggles();
			DisplayLevel(maxActiveLevelIdx);
		}
    }

	/// <summary>
	/// Exibição visual para o número de níveis
	/// </summary>
	private void DisplayToggles()
	{
		foreach (Toggle toggle in togglesFolder.GetComponentsInChildren<Toggle>())
		{
			Destroy(toggle.gameObject);
		}
		int cnt;
		for (cnt = 0; cnt < maxActiveLevelIdx + 1; cnt++)
		{
			GameObject toggle = Instantiate(activeTogglePrefab.gameObject, togglesFolder);
			activeToggles.Add(toggle.GetComponent<Toggle>());
		}
		if (maxActiveLevelIdx < levelsPrefabs.Count - 1)
		{
			Instantiate(inactiveTogglePrefab.gameObject, togglesFolder);
		}
	}

	/// <summary>
	/// Exibe o nível escolhido.
	/// </summary>
	/// <param name="levelIdx">Índice do nível</param>
	private void DisplayLevel(int levelIdx)
	{
		Transform parentOfLevel = currentLevel.transform.parent;
		Vector3 levelPosition = currentLevel.transform.position;
		Quaternion levelRotation = currentLevel.transform.rotation;
		Destroy(currentLevel);
		currentLevel = Instantiate(levelsPrefabs[levelIdx], parentOfLevel);
		currentLevel.name = levelsPrefabs[levelIdx].name;
		currentLevel.transform.position = levelPosition;
		currentLevel.transform.rotation = levelRotation;
		currentDisplayedLevelIdx = levelIdx;
		foreach (Toggle toggle in activeToggles)
		{
			toggle.isOn = false;
		}
		activeToggles[levelIdx].isOn = true;
		UpdateButtonsVisible (levelIdx);
	}

	/// <summary>
	/// Atualiza os botões visíveis.
	/// </summary>
	/// <param name="levelIdx">Índice do nível</param>
	private void UpdateButtonsVisible(int levelIdx)
	{
		prevLevelButton.interactable = levelIdx > 0 ? true : false;
		nextLevelButton.interactable = levelIdx < maxActiveLevelIdx ? true : false;
	}

	/// <summary>
	/// Exibe o próximo nível.
	/// </summary>
	private void DisplayNextLevel()
	{
		if (currentDisplayedLevelIdx < maxActiveLevelIdx)
		{
			DisplayLevel(currentDisplayedLevelIdx + 1);
		}
	}

	/// <summary>
	/// Exibe o nível anterior.
	/// </summary>
	private void DisplayPrevLevel()
	{
		if (currentDisplayedLevelIdx > 0)
		{
			DisplayLevel (currentDisplayedLevelIdx - 1);
		}
	}

	/// <summary>
	/// Saí de cena
	/// </summary>
	private void Exit()
	{
		SceneManager.LoadScene(exitSceneName);
	}

	/// <summary>
	/// Vai para o nível escolhido.
	/// </summary>
	private void GoToLevel()
	{
		SceneManager.LoadScene(currentLevel.name);
	}

	/// <summary>
	/// Manipulador de botões pressionados.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void ButtonPressed(GameObject obj, string param)
	{
		switch (param)
		{
		case "Start":
			GoToLevel();
			break;
		case "Exit":
			Exit();
			break;
		case "Next":
			DisplayNextLevel();
			break;
		case "Prev":
			DisplayPrevLevel();
			break;
		}
	}
}
