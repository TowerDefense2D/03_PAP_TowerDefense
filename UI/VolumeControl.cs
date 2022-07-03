using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla os efeitos sonoros e o volume da trilha sonora por meio de sliders.
/// </summary>
public class VolumeControl : MonoBehaviour
{
	// slider para volume de efeitos sonoros
	public Slider sound;
	// Slider para o volume da trilha sonora
	public Slider music;

	/// <summary>
	/// Começa esta instância
	/// </summary>
	void Start()
	{
		Debug.Assert(sound && music, "Wrong initial settings");
		sound.value = DataManager.instance.configs.soundVolume;
		music.value = DataManager.instance.configs.musicVolume;
		sound.onValueChanged.AddListener(delegate {OnVolumeChanged();});
		music.onValueChanged.AddListener(delegate {OnVolumeChanged();});
	}

	/// <summary>
	/// Aumenta o evento de volume alterado.
	/// </summary>
	private void OnVolumeChanged()
	{
		// Armazenaa as novas configurações
		DataManager.instance.configs.soundVolume = sound.value;
		DataManager.instance.configs.musicVolume = music.value;
		DataManager.instance.SaveGameConfigs();
		// Aplica novas configurações
		AudioManager.instance.SetVolume(DataManager.instance.configs.soundVolume, DataManager.instance.configs.musicVolume);
	}
}
