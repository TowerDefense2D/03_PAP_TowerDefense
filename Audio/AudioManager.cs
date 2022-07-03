using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reproduz trilha sonora e efeitos sonoros. Limite o número de efeitos sonoros reproduzidos simultaneamente./// </summary>
public class AudioManager : MonoBehaviour
{
	// Singleton
	public static AudioManager instance;

	// Fonte de som para efeitos sonoros
	public AudioSource soundSource;
	// Fonte de som para trilha sonora
	public AudioSource musicSource;
	// Trilha sonora
	public AudioClip track;
	// SFX do início da ronda
	public AudioClip waveStart;
	// Inimigo atingiu o ponto de captura sfx
	public AudioClip captured;
	// Jogador clica na torre sfx
	public AudioClip towerClick;
	// Jogador clica na unidade sfx
	public AudioClip unitClick;
	// Jogador clica o sfx da interface do usuário (UI)
	public AudioClip uiClick;
	// Construir torre sfx
	public AudioClip towerBuild;
	// Vender torre sfx
	public AudioClip towerSell;
	// Derrotado sfx
	public AudioClip defeat;
	// Vitória sfx
	public AudioClip victory;

	// Ataque sfx é colocado agora
	private bool attackCoroutine = false;
	// Morreu sfx é colocado agora
	private bool dieCoroutine = false;

	/// <summary>
	/// Ativa o enable event.
	/// </summary>
	void OnEnable()
	{
		instance = this;
		EventManager.StartListening("GamePaused", GamePaused);
		EventManager.StartListening("WaveStart", WaveStart);
		EventManager.StartListening("Captured", Captured);
		EventManager.StartListening("UserClick", UserClick);
		EventManager.StartListening("UserUiClick", UserUiClick);
		EventManager.StartListening("TowerBuild", TowerBuild);
		EventManager.StartListening("TowerSell", TowerSell);
		EventManager.StartListening("Defeat", Defeat);
		EventManager.StartListening("Victory", Victory);
	}

	/// <summary>
	/// Ativa o disable event 
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("GamePaused", GamePaused);
		EventManager.StopListening("WaveStart", WaveStart);
		EventManager.StopListening("Captured", Captured);
		EventManager.StopListening("UserClick", UserClick);
		EventManager.StopListening("UserUiClick", UserUiClick);
		EventManager.StopListening("TowerBuild", TowerBuild);
		EventManager.StopListening("TowerSell", TowerSell);
		EventManager.StopListening("Defeat", Defeat);
		EventManager.StopListening("Victory", Victory);
	}

	/// <summary>
	/// Começa esta instância
	/// </summary>
	void Start()
	{
		Debug.Assert(soundSource && musicSource, "Wrong initial settings");
		// Define o volume das configurações armazenadas
		SetVolume(DataManager.instance.configs.soundVolume, DataManager.instance.configs.musicVolume);
	}

	/// <summary>
	/// Ativa o DestroyEvent
	/// </summary>
	void OnDestroy()
	{
		StopAllCoroutines();
		if (instance == this)
		{
			instance = null;
		}
	}

	/// <summary>
	/// Pausa o jogo
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void GamePaused(GameObject obj, string param)
	{
		if (param == bool.TrueString) // Pausado
		{
			// Pausa a trilha sonora
			musicSource.Pause();
		}
		else // Não pausado
		{
			// Reproduz trilha sonora
			if (track != null)
			{
				musicSource.clip = track;
				musicSource.Play();
			}
		}
	}

	/// <summary>
	/// Define o volume.
	/// </summary>
	/// <param name="sound">Som</param>
	/// <param name="music">Musica</param>
	public void SetVolume(float sound, float music)
	{
		soundSource.volume = sound;
		musicSource.volume = music;
	}

	/// <summary>
	/// Reproduz o sfx (sem limite).
	/// </summary>
	/// <param name="audioClip">clip de Aúdio</param>
	public void PlaySound(AudioClip audioClip)
	{
		soundSource.PlayOneShot(audioClip, soundSource.volume);
	}

	/// <summary>
	/// Reproduz o ataque sfx (um sfx ao mesmo tempo).
	/// </summary>
	/// <param name="audioClip">clip de Aúdio</param>
	public void PlayAttack(AudioClip audioClip)
	{
		if (attackCoroutine == false)
		{
			StartCoroutine(AttackCoroutine(audioClip));
		}
	}

	/// <summary>
	/// Corrotina (Coroutine) de ataques.
	/// </summary>
	/// <returns>A corrotina </returns>
	/// <param name="audioClip">clip de Aúdio</param>
	private IEnumerator AttackCoroutine(AudioClip audioClip)
	{
		attackCoroutine = true;
		PlaySound(audioClip);
		// Aguarda o final do clip
		yield return new WaitForSeconds(audioClip.length);
		attackCoroutine = false;
	}

	/// <summary>
	/// Reproduz o dado sfx (um sfx ao mesmo tempo).
	/// </summary>
	/// <param name="audioClip">clip de Aúdio</param>
	public void PlayDie(AudioClip audioClip)
	{
		if (dieCoroutine == false)
		{
			StartCoroutine(DieCoroutine(audioClip));
		}
	}

	/// <summary>
	/// Corrotina morre.
	/// </summary>
	/// <returns>A corrotina.</returns>
	/// <param name="audioClip">clip de Aúdio</param>
	private IEnumerator DieCoroutine(AudioClip audioClip)
	{
		dieCoroutine = true;
		PlaySound(audioClip);
		//Aguarda o final do clip
        yield return new WaitForSeconds(audioClip.length);
		dieCoroutine = false;
	}

	/// <summary>
	/// Rondas começam.
	/// </summary>
	/// <param name="obj">Objeto.</param>
	/// <param name="param">Perimetro</param>
	private void WaveStart(GameObject obj, string param)
	{
		if (waveStart != null)
		{
			PlaySound(waveStart);
		}
	}

	/// <summary>
	/// O inimigo atingiu o ponto de captura.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void Captured(GameObject obj, string param)
	{
		if (captured != null)
		{
			PlaySound(captured);
		}
	}

	/// <summary>
	/// No UI (Interface do utilizador) do utilizador, clica.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UserUiClick(GameObject obj, string param)
	{
		if (obj != null)
		{
			PlaySound(uiClick);
		}
	}

	/// <summary>
	/// Manipulador de cliques do utilizador
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UserClick(GameObject obj, string param)
	{
		if (obj != null)
		{
			Tower tower = obj.GetComponent<Tower>();
			if (tower != null)
			{
				PlaySound(towerClick);
			}
			else
			{
				UnitInfo unitInfo = obj.GetComponent<UnitInfo>();
				if (unitInfo != null)
				{
					PlaySound(unitClick);
				}
			}
		}
	}

	/// <summary>
	/// Manipulador de construção de torres.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetros</param>
	private void TowerBuild(GameObject obj, string param)
	{
		if (towerBuild != null)
		{
			PlaySound(towerBuild);
		}
	}

	/// <summary>
	/// Manipulador de venda de torres.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void TowerSell(GameObject obj, string param)
	{
		if (towerSell != null)
		{
			PlaySound(towerSell);
		}
	}

	/// <summary>
	/// Derrota o manipulador.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void Defeat(GameObject obj, string param)
	{
		if (defeat != null)
		{
			PlaySound(defeat);
		}
	}

	/// <summary>
	/// Manipulador de vitórias.
	/// </summary>
	/// <param name="obj">Objecto</param>
	/// <param name="param">Perimetro</param>
	private void Victory(GameObject obj, string param)
	{
		if (victory != null)
		{
			PlaySound(victory);
		}
	}
}
