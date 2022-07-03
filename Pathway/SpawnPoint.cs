using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Spawner Inimigo.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
	/// <summary>
	/// Estrutura da ronda de inimigos por vir 
	/// </summary>
	[System.Serializable]
	public class Wave
	{
		// Atraso antes de a ronda ser executada
		public float delayBeforeWave;
		// Lista de inimigos para a ronda 
		public List<GameObject> enemies = new List<GameObject>();
	}

	// Os inimigos têm velocidades diferentes num intervalo especifíco 
	public float speedRandomizer = 0.2f;
	// Atraso de tempo entre os inimigos e a ronda
	public float unitSpawnDelay = 1.5f;
	// Lista de Rondas para este spawner 
	public List<Wave> waves;
	// MODO de Inimigos sem fim para este spawn point 
	public bool endlessWave = false;
	// Esta lista é usada para um ponto de lista aleatório 
	[HideInInspector]
	public List<GameObject> randomEnemiesList = new List<GameObject>();

	// Os inimigos vão-se mover por este caminho 
	private Pathway path;
	// Atraso no counter 
	private float counter;
	// Buffer com inimigos ativos que já levaram spawn
	private List<GameObject> activeEnemies = new List<GameObject>();
	// Todos os inimigos foram spawnados
	private bool finished = false;

	/// <summary>
	/// Ativar esta Instância
	/// </summary>
	void Awake ()
	{
		path = GetComponentInParent<Pathway>();
		Debug.Assert(path != null, "Wrong initial parameters");
	}

	/// <summary>
	/// Permite aumentar o evento ativado (enabled event)
	/// </summary>
	void OnEnable()
	{
		EventManager.StartListening("UnitDie", UnitDie);
		EventManager.StartListening("WaveStart", WaveStart);
	}

	/// <summary>
	/// Permite aumentar o event desativado (disabled event)
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("UnitDie", UnitDie);
		EventManager.StopListening("WaveStart", WaveStart);
	}

	/// <summary>
	/// Atualiza esta instância 
	/// </summary>
	void Update()
	{
		// Se todos os inimigos que levarem spawn estão mortos  
		if ((finished == true) && (activeEnemies.Count <= 0))
		{
			EventManager.TriggerEvent("AllEnemiesAreDead", null, null);
			gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Executa esta wave 
	/// </summary>
	/// <returns>A wave.</returns>
	private IEnumerator RunWave(int waveIdx)
	{
		if (waves.Count > waveIdx)
		{
			yield return new WaitForSeconds(waves[waveIdx].delayBeforeWave);

			while (endlessWave == true)
			{
				GameObject prefab = randomEnemiesList[Random.Range (0, randomEnemiesList.Count)];
				// Cria Inimigo 
				GameObject newEnemy = Instantiate(prefab, transform.position, transform.rotation);
				newEnemy.name = prefab.name;
				// Imprime o caminho que os inimigos seguem 
				newEnemy.GetComponent<AiStatePatrol>().path = path;
				NavAgent agent = newEnemy.GetComponent<NavAgent>();
				// Imprime a velocidade dos inimigos  
				agent.speed = Random.Range(agent.speed * (1f - speedRandomizer), agent.speed * (1f + speedRandomizer));
				// Põe o inimigo na lista 
				activeEnemies.Add(newEnemy);
				// Espera pelo delay antes da próxima ronda de inimigos 
				yield return new WaitForSeconds(unitSpawnDelay);
			}

			foreach (GameObject enemy in waves[waveIdx].enemies)
			{
				GameObject prefab = null;
				prefab = enemy;
				// Se o prefab do inimigo não for especifico - spawn a inimigo aleatório 
				if (prefab == null && randomEnemiesList.Count > 0)
				{
					prefab = randomEnemiesList[Random.Range (0, randomEnemiesList.Count)];
				}
				if (prefab == null)
				{
					Debug.LogError("Have no enemy prefab. Please specify enemies in Level Manager or in Spawn Point");
				}
				// Cria Inimigo 
				GameObject newEnemy = Instantiate(prefab, transform.position, transform.rotation);
				newEnemy.name = prefab.name;
				// Imprime o caminho que os inimigos seguem 
				newEnemy.GetComponent<AiStatePatrol>().path = path;
				NavAgent agent = newEnemy.GetComponent<NavAgent>();
				// Imprime a velocidade dos inimigos  
				agent.speed = Random.Range(agent.speed * (1f - speedRandomizer), agent.speed * (1f + speedRandomizer));
				// Põe o inimigo na lista 
				activeEnemies.Add(newEnemy);
				// Espera pelo delay antes da próxima ronda de inimigos 
				yield return new WaitForSeconds(unitSpawnDelay);
			}
			if (waveIdx + 1 == waves.Count)
			{
				finished = true;
			}
		}
	}

	/// <summary>
	/// Na unidade morre
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="param">Parameter.</param>
	private void UnitDie(GameObject obj, string param)
	{
		// Se for inimigo ativo 
		if (activeEnemies.Contains(obj) == true)
		{
			// Remove-o do buffer
			activeEnemies.Remove(obj);
		}
	}

	// Evento do começo da ronda começado 
	private void WaveStart(GameObject obj, string param)
	{
		int waveIdx;
		int.TryParse(param, out waveIdx);
		StartCoroutine("RunWave", waveIdx);
	}

	/// <summary>
	/// Aumenta o evento de destruição 
	/// </summary>
	void OnDestroy()
	{
		StopAllCoroutines();
	}
}
