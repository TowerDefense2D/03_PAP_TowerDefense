using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script de controle de nível.
/// </summary>
public class LevelManager : MonoBehaviour
{
	// cena da interface do utilizador. Carregar no início do nível
	public string levelUiSceneName;
	// Quantidade de ouro para este nível
	public int goldAmount = 20;
	// Quantas vezes os inimigos podem alcançar o ponto de captura antes da derrota
	public int defeatAttempts = 1;
	// Lista com inimigos gerados aleatoriamente permitidos para este nível
	public List<GameObject> allowedEnemies = new List<GameObject>();
	// Lista com torres permitidas para este nível
	public List<GameObject> allowedTowers = new List<GameObject>();
	// Lista com feitiços permitidos para este nível
	public List<GameObject> allowedSpells = new List<GameObject>();

	// Gerador de interface do utilizador
	private UiManager uiManager;
	// Números de spawners inimigos neste nível
	private int spawnNumbers;
	// Contador solto atual
	private int beforeLooseCounter;
	// Condição de vitória ou derrota já acionada
	private bool triggered = false;

    /// <summary>
    /// Ativar esta instância
    /// </summary>
    void Awake()
    {
		// Carregar cena da interface do utilizador
		SceneManager.LoadScene(levelUiSceneName, LoadSceneMode.Additive);
    }

	/// <summary>
	/// Começar esta instância
	/// </summary>
	void Start()
	{
		uiManager = FindObjectOfType<UiManager>();
		SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();
		spawnNumbers = spawnPoints.Length;
		if (spawnNumbers <= 0)
		{
			Debug.LogError("Have no spawners");
		}
		// Define uma lista de inimigos aleatórios para cada spawner
		foreach (SpawnPoint spawnPoint in spawnPoints)
		{
			spawnPoint.randomEnemiesList = allowedEnemies;
		}
		Debug.Assert(uiManager, "Wrong initial parameters");
		// Define a quantidade de ouro para este nível
		uiManager.SetGold(goldAmount);
		beforeLooseCounter = defeatAttempts;
		uiManager.SetDefeatAttempts(beforeLooseCounter);
	}

    /// <summary>
    /// Ativa o enable event
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("Captured", Captured);
        EventManager.StartListening("AllEnemiesAreDead", AllEnemiesAreDead);
    }

    /// <summary>
    /// Ativa o disable event
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening("Captured", Captured);
        EventManager.StopListening("AllEnemiesAreDead", AllEnemiesAreDead);
    }

	/// <summary>
	/// O inimigo atingiu o ponto de captura.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void Captured(GameObject obj, string param)
    {
		if (beforeLooseCounter > 0)
		{
			beforeLooseCounter--;
			uiManager.SetDefeatAttempts(beforeLooseCounter);
			if (beforeLooseCounter <= 0)
			{
				triggered = true;
				// Derrota
				EventManager.TriggerEvent("Defeat", null, null);
			}
		}
    }

	/// <summary>
	/// Todos os inimigos estão mortos.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void AllEnemiesAreDead(GameObject obj, string param)
    {
        spawnNumbers--;
		//Inimigos mortos em todos os spawners
		if (spawnNumbers <= 0)
        {
			// Verifique se a condição solta não foi acionada antes
			if (triggered == false)
			{
				// Vitória
				EventManager.TriggerEvent("Victory", null, null);
			}
        }
    }
}
