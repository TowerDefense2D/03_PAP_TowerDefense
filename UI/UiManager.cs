using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Interface do utilizador e gerador de eventos.
/// </summary>
public class UiManager : MonoBehaviour
{
    // Esta cena será carregada após a saída de nível
    public string exitSceneName;
    // Tela inicial do canvas
    public GameObject startScreen;
    // Tela de Pausa do menu
    public GameObject pauseMenu;
    // Tela de Derrota do menu
    public GameObject defeatMenu;
    // Tela de Vitórioa do menu 
    public GameObject victoryMenu;
    // Interface de nível
    public GameObject levelUI;
    // Quantidade de ouro disponível
    public Text goldAmount;
    // Tentativas de captura antes da derrota
    public Text defeatAttempts;
    // Atraso de exibição do menu de vitória e derrota
    public float menuDisplayDelay = 1f;

    // O jogo está em pause?
    private bool paused;
    // A câmera está a ser arrastada agora
    private bool cameraIsDragged;
    // Ponto de origem do início do arrastamento da câmera
    private Vector3 dragOrigin = Vector3.zero;
    // Componente de controle da câmera
    private CameraControl cameraControl;

	/// <summary>
	/// Ativa esta instância
	/// </summary>
	void Awake()
	{
		cameraControl = FindObjectOfType<CameraControl>();
		Debug.Assert(cameraControl && startScreen && pauseMenu && defeatMenu && victoryMenu && levelUI && defeatAttempts && goldAmount, "Wrong initial parameters");
	}

    /// <summary>
    /// Ativa o enable event.
    /// </summary>
    void OnEnable()
    {
		EventManager.StartListening("UnitKilled", UnitKilled);
		EventManager.StartListening("ButtonPressed", ButtonPressed);
		EventManager.StartListening("Defeat", Defeat);
		EventManager.StartListening("Victory", Victory);
    }

    /// <summary>
    /// Ativa o disable event.
    /// </summary>
    void OnDisable()
    {
		EventManager.StopListening("UnitKilled", UnitKilled);
		EventManager.StopListening("ButtonPressed", ButtonPressed);
		EventManager.StopListening("Defeat", Defeat);
		EventManager.StopListening("Victory", Victory);
    }

    /// <summary>
    /// Começa esta instância
    /// </summary>
    void Start()
    {
		PauseGame(true);
    }

    /// <summary>
    /// Atualiza esta instância
    /// </summary>
    void Update()
    {
        if (paused == false)
        {
            // O utilizador clica no o botão do rato
            if (Input.GetMouseButtonDown(0) == true)
            {
                // Verifica se o ponteiro sobre os componentes da interface do utilizador
                GameObject hittedObj = null;
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
				if (results.Count > 0) // Componentes da interface do utilizaodr no ponteiro
                {
                    // Pesquisa por ícons que tenham uma ação atingida nos resultados
                    foreach (RaycastResult res in results)
					{
						if (res.gameObject.CompareTag("ActionIcon"))
						{
							hittedObj = res.gameObject;
							break;
						}
					}
                    // Envia mensagem com dados de clique do utlizador no componente de interface do utilizador
                    EventManager.TriggerEvent("UserUiClick", hittedObj, null);
				}
                else // Nenhum componente de interface do utilizador no ponteiro
                {
                    // Verifica o ponteiro sobre colisores
                    RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
                    foreach (RaycastHit2D hit in hits)
                    {
                        // Se este objeto tiver informações de unidade
                        if (hit.collider.CompareTag("UnitInfo"))
                        {
							Tower tower = hit.collider.GetComponentInParent<Tower>();
							if (tower != null)
							{
								hittedObj = tower.gameObject;
								break;
							}
							AiBehavior aiBehavior = hit.collider.GetComponentInParent<AiBehavior>();
							if (aiBehavior != null)
							{
								hittedObj = aiBehavior.gameObject;
								break;
							}
							hittedObj = hit.collider.gameObject;
                            break;
                        }
                    }
                    // Envia mensagem com dados de clique do utilizador no espaço do jogo
                    EventManager.TriggerEvent("UserClick", hittedObj, null);
                }
                // Se não houver nenhum objeto atingido - inicia o arraste da câmera
                if (hittedObj == null)
                {
                    cameraIsDragged = true;
                    dragOrigin = Input.mousePosition;
                }
            }
            if (Input.GetMouseButtonUp(0) == true)
            {
                // Para de arrastar a câmera ao soltar o rotar
                cameraIsDragged = false;
            }
            if (cameraIsDragged == true)
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                // Arraste da câmera (invertido)
                cameraControl.MoveX(-pos.x);
                cameraControl.MoveY(-pos.y);
            }
        }
    }

    /// <summary>
    /// Para a cena atual e carrega uma nova cena
    /// </summary>
    /// <param name="sceneName">Nome da cena.</param>
    private void LoadScene(string sceneName)
    {
		EventManager.TriggerEvent("SceneQuit", null, null);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Retoma o jogo.
    /// </summary>
	private void ResumeGame()
    {
        GoToLevel();
        PauseGame(false);
    }

    /// <summary>
    /// Vai para o menu principal.
    /// </summary>
	private void ExitFromLevel()
    {
        LoadScene(exitSceneName);
    }

    /// <summary>
    /// Fecha todas as telas de interface do utilizador.
    /// </summary>
    private void CloseAllUI()
    {
		startScreen.SetActive (false);
        pauseMenu.SetActive(false);
        defeatMenu.SetActive(false);
        victoryMenu.SetActive(false);
    }

    /// <summary>
    /// Pausa o jogo.
    /// </summary>
    /// <param name="pause">Se definido como <c>true</c> pausa.</param>
    private void PauseGame(bool pause)
    {
        paused = pause;
        // Para o tempo em pausa
        Time.timeScale = pause ? 0f : 1f;
		EventManager.TriggerEvent("GamePaused", null, pause.ToString());
    }

    /// <summary>
    /// Vai para o menu de pausa.
    /// </summary>
	private void GoToPauseMenu()
    {
        PauseGame(true);
        CloseAllUI();
        pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Vai para o nível.
    /// </summary>
    private void GoToLevel()
    {
        CloseAllUI();
        levelUI.SetActive(true);
        PauseGame(false);
    }

    /// <summary>
    /// Vai para o menu de derrota.
    /// </summary>
	private void Defeat(GameObject obj, string param)
    {
		StartCoroutine("DefeatCoroutine");
    }

    /// <summary>
    /// Exibe menu de derrota após atraso.
    /// </summary>
    /// <returns>A corrotina.</returns>
    private IEnumerator DefeatCoroutine()
	{
		yield return new WaitForSeconds(menuDisplayDelay);
		PauseGame(true);
		CloseAllUI();
		defeatMenu.SetActive(true);
	}

    /// <summary>
    /// Vai para o menu da vitória.
    /// </summary>
	private void Victory(GameObject obj, string param)
    {
		StartCoroutine("VictoryCoroutine");
    }

    /// <summary>
    /// Exibe menu de vitória após atraso.
    /// </summary>
    /// <returns>A corrotina.</returns>
    private IEnumerator VictoryCoroutine()
	{
		yield return new WaitForSeconds(menuDisplayDelay);
		PauseGame(true);
		CloseAllUI();

        // --- Save automático do progresso do jogo ---
        // Obtém o nome do nível concluído
        DataManager.instance.progress.lastCompetedLevel = SceneManager.GetActiveScene().name;
        // Verifica se este nível não foi concluído antes
        bool hit = false;
		foreach (string level in DataManager.instance.progress.openedLevels)
		{
			if (level == SceneManager.GetActiveScene().name)
			{
				hit = true;
				break;
			}
		}
		if (hit == false)
		{
			DataManager.instance.progress.openedLevels.Add(SceneManager.GetActiveScene().name);
		}
        // Salva o progresso do jogo
        DataManager.instance.SaveGameProgress();

		victoryMenu.SetActive(true);
	}

    /// <summary>
    /// Reinicia o nível atual.
    /// </summary>
	private void RestartLevel()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Obtém a quantidade de ouro atual.
    /// </summary>
    /// <returns>O ouro</returns>
	public int GetGold()
    {
        int gold;
        int.TryParse(goldAmount.text, out gold);
        return gold;
    }

    /// <summary>
    /// Define a quantidade de ouro.
    /// </summary>
    /// <param name="gold">Ouro</param>
	public void SetGold(int gold)
    {
        goldAmount.text = gold.ToString();
    }

    /// <summary>
    /// Adiciona o ouro.
    /// </summary>
    /// <param name="gold">Ouro</param>
	public void AddGold(int gold)
    {
        SetGold(GetGold() + gold);
    }

    /// <summary>
    /// Gasta o ouro se for.
    /// </summary>
    /// <returns><c>true</c>, se o ouro foi gasto, <c>false</c> senão</returns>
    /// <param name="cost">Custo.</param>
    public bool SpendGold(int cost)
    {
        bool res = false;
        int currentGold = GetGold();
        if (currentGold >= cost)
        {
            SetGold(currentGold - cost);
            res = true;
        }
        return res;
    }

    /// <summary>
    /// Define as tentativas de derrota.
    /// </summary>
    /// <param name="attempts">Tentativas</param>
    public void SetDefeatAttempts(int attempts)
	{
		defeatAttempts.text = attempts.ToString();
	}

    /// <summary>
    /// a unidade morta por outra unidade.
    /// </summary>
    /// <param name="obj">Objeto</param>
    /// <param name="param">Perimetro</param>
	private void UnitKilled(GameObject obj, string param)
    {
        // Se isto é inimigo
        if (obj.CompareTag("Enemy") || obj.CompareTag("FlyingEnemy"))
        {
            Price price = obj.GetComponent<Price>();
            if (price != null)
            {
                // Adiciona ouro para matar inimigos
                AddGold(price.price);
            }
        }
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
		case "Pause":
			GoToPauseMenu();
			break;
		case "Resume":
			GoToLevel();
			break;
		case "Back":
			ExitFromLevel();
			break;
		case "Restart":
			RestartLevel();
			break;
		}
	}

	/// <summary>
	/// Ativa o destroy event.
	/// </summary>
	void OnDestroy()
	{
		StopAllCoroutines();
	}
}
