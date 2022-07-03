using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tempos das rondas pertencentes aos inimigas.
/// </summary>
[ExecuteInEditMode]
public class WavesInfo : MonoBehaviour
{
	// Entre as rondas
	public List<float> wavesTimeouts = new List<float>();

	// Executar apenas no modo de edição
#if UNITY_EDITOR

	// Entre as rondas
	private float defaultWaveTimeout = 30f;
	// Lista com spawners ativos no nível
	private SpawnPoint[] spawners;

	/// <summary>
	/// Na atualização do editor.
	/// </summary>
	public void Update()
	{
		spawners = FindObjectsOfType<SpawnPoint>();
		int wavesCount = 0;
		// Obtém o número máximo de rondas dos geradores
		foreach (SpawnPoint spawner in spawners)
		{
			if (spawner.waves.Count > wavesCount)
			{
				wavesCount = spawner.waves.Count;
			}
		}
		// Exibe uma lista real com tempos limite das rondas
		if (wavesTimeouts.Count < wavesCount)
		{
			int i;
			for (i = wavesTimeouts.Count; i < wavesCount; ++i)
			{
				wavesTimeouts.Add (defaultWaveTimeout);
			}
		}
		else if (wavesTimeouts.Count > wavesCount)
		{
			wavesTimeouts.RemoveRange (wavesCount, wavesTimeouts.Count - wavesCount);
		}
	}

#endif
}
