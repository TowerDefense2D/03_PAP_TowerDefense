using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor de informações das rondas.
/// </summary>
public class WavesInfoInspector : MonoBehaviour
{
	[HideInInspector]
	// Tempos limite entre rondas
	public List<float> timeouts
	{
		get
		{
			return wavesInfo.wavesTimeouts;
		}
		set
		{
			wavesInfo.wavesTimeouts = value;
		}
	}

	// Componente de informações das rondas
	private WavesInfo wavesInfo;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		wavesInfo = GetComponent<WavesInfo>();
		Debug.Assert(wavesInfo, "Wrong stuff settings");
	}

	/// <summary>
	/// Atualiza esta instância
	/// </summary>
	public void Update()
	{
		wavesInfo.Update();
	}
}
#endif
