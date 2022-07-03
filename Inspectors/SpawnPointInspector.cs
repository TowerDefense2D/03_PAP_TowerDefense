using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor de ponto de spawn.
/// </summary>
public class SpawnPointInspector : MonoBehaviour
{
	[HideInInspector]
	// Lista com número de inimigos para cada ronda
	public List<int> enemies = new List<int>();

	// Componente de ponto de spawn
	private SpawnPoint spawnPoint;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		spawnPoint = GetComponent<SpawnPoint>();
		Debug.Assert(spawnPoint, "Wrong stuff settings");
		// Inicia a lista de rondas
		enemies.Clear();
		foreach (SpawnPoint.Wave wave in spawnPoint.waves)
		{
			enemies.Add(wave.enemies.Count);
		}
	}

	/// <summary>
	/// Atualiza a lista de rondas.
	/// </summary>
	public void UpdateWaveList()
	{
		// Atualiza ondas
		while (spawnPoint.waves.Count > enemies.Count)
		{
			spawnPoint.waves.RemoveAt(spawnPoint.waves.Count - 1);
		}
		while (spawnPoint.waves.Count < enemies.Count)
		{
			spawnPoint.waves.Add(new SpawnPoint.Wave());
		}
		// Atualizaa contagem de inimigos
		for (int i = 0; i < enemies.Count; i++)
		{
			while (spawnPoint.waves[i].enemies.Count > enemies[i])
			{
				spawnPoint.waves[i].enemies.RemoveAt(spawnPoint.waves[i].enemies.Count - 1);
			}
			while (spawnPoint.waves[i].enemies.Count < enemies[i])
			{
				spawnPoint.waves[i].enemies.Add(null);
			}
		}
	}

	/// <summary>
	/// Adiciona a ronda.
	/// </summary>
	public void AddWave()
	{
		enemies.Add(1);
	}

	/// <summary>
	/// Remove a ronda.
	/// </summary>
	public void RemoveWave()
	{
		if (enemies.Count > 0)
		{
			enemies.RemoveAt(enemies.Count - 1);
		}
	}
}
#endif
