using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Temporizador para exibir a ronda inimiga atual.
/// </summary>
[RequireComponent(typeof(Image))]
public class WavesTimer : MonoBehaviour
{
	// Visualização do TO restante
	public Image timeBar;
	// Campo de texto da ronda atual
	public Text currentWaveText;
	// Campo de texto de ronda máxima
	public Text maxWaveNumberText;
	//  Efeito do temporizador destacado
	public GameObject highlightedFX;
	// Duração do efeito destacado
	public float highlightedTO = 0.2f;

	// Descritor de rondas para este nível de jogo
	private WavesInfo wavesInfo;
	// Lista de rondas
	private List<float> waves = new List<float>();
	// ronda atual
	private int currentWave;
	// TO antes da próxima ronda
	private float currentTimeout;
	// Conta relógio
	private float counter;
	// Temporizador parado
	private bool finished;

	/// <summary>
	/// Ativa o disable event.
	/// </summary>
	void OnDisable()
	{
		StopAllCoroutines ();
	}

	/// <summary>
	/// Ativa esta instància
	/// </summary>
    void Awake()
    {
		wavesInfo = FindObjectOfType<WavesInfo>();
		Debug.Assert(timeBar && highlightedFX && wavesInfo && timeBar && currentWaveText && maxWaveNumberText, "Wrong initial settings");
    }

	/// <summary>
	/// Começa esta instància
	/// </summary>
	void Start()
    {
		highlightedFX.SetActive(false);
		waves = wavesInfo.wavesTimeouts;
        currentWave = 0;
        counter = 0f;
        finished = false;
        GetCurrentWaveCounter();
        maxWaveNumberText.text = waves.Count.ToString();
        currentWaveText.text = "0";
	}
	
	/// <summary>
	/// Atualiza esta instância
	/// </summary>
	void FixedUpdate()
    {
        if (finished == false)
        {
			// Tempo limite expirado
			if (counter <= 0f)
            {
				// Envia um evento sobre o início da próxima ronda
				EventManager.TriggerEvent("WaveStart", null, currentWave.ToString());
                currentWave++;
                currentWaveText.text = currentWave.ToString();
				// Realca o temporizador por um curto período de tempo
				StartCoroutine("HighlightTimer");
				// Todas as rondas são enviadas
				if (GetCurrentWaveCounter() == false)
                {
                    finished = true;
					// Enviaa um evento sobre a parada do timer
					EventManager.TriggerEvent("TimerEnd", null, null);
                    return;
                }
            }
			counter -= Time.fixedDeltaTime;
            if (currentTimeout > 0f)
            {
                timeBar.fillAmount = counter / currentTimeout;
            }
            else
            {
                timeBar.fillAmount = 0f;
            }
        }
	}

	/// <summary>
	/// Obtém o tempo limite da ronda atual.
	/// </summary>
	/// <returns><c>true</c>, se o tempo limite da onda atual for obtido, <c>false</c> otherwise.</returns>
	private bool GetCurrentWaveCounter()
    {
        bool res = false;
        if (waves.Count > currentWave)
        {
            counter = currentTimeout = waves[currentWave];
            res = true;
        }
        return res;
    }

	/// <summary>
	/// Realça a corrotina do temporizador.
	/// </summary>
	/// <returns>O temporizador</returns>
	private IEnumerator HighlightTimer()
	{
		highlightedFX.SetActive(true);
		yield return new WaitForSeconds(highlightedTO);
		highlightedFX.SetActive(false);
	}

	/// <summary>
	/// Ativa o destroy event.
	/// </summary>
	void OnDestroy()
	{
		StopAllCoroutines();
	}
}
