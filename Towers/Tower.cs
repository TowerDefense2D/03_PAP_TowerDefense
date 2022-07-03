using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Construção e operação da torre.
/// </summary>
public class Tower : MonoBehaviour
{
    // Prefab para árvore de ações
    public GameObject actions;
    // Visualização do alcance de ataque ou defesa para esta torre
    public GameObject range;

    // Gerenciador de interface do utilizador
    private UiManager uiManager;

    /// <summary>
    /// Ativa o enable event.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("GamePaused", GamePaused);
        EventManager.StartListening("UserClick", UserClick);
		EventManager.StartListening("UserUiClick", UserClick);
    }

    /// <summary>
    /// Ativa o disable event
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening("GamePaused", GamePaused);
        EventManager.StopListening("UserClick", UserClick);
		EventManager.StopListening("UserUiClick", UserClick);
    }

    /// <summary>
    /// Começa esta instância.
    /// </summary>
    void Start()
    {
        uiManager = FindObjectOfType<UiManager>();
		Debug.Assert(uiManager && actions, "Wrong initial parameters");
		CloseActions();
    }

    /// <summary>
    /// Abre a árvore de ações.
    /// </summary>
    private void OpenActions()
    {
		actions.SetActive(true);
    }

    /// <summary>
    /// Fecha a árvore de ações.
    /// </summary>
    private void CloseActions()
    {
		if (actions.activeSelf == true)
        {
			actions.SetActive(false);
        }
    }

    /// <summary>
    /// Constrói a torre.
    /// </summary>
    /// <param name="towerPrefab">prefab da torre.</param>
    public void BuildTower(GameObject towerPrefab)
    {
        // Fecha a árvore de ações ativas
        CloseActions();
        Price price = towerPrefab.GetComponent<Price>();
        //  Se tiver ouro suficiente
        if (uiManager.SpendGold(price.price) == true)
        {
            // Cria uma nova torre e coloca-a na mesma posição
            GameObject newTower = Instantiate<GameObject>(towerPrefab, transform.parent);
			newTower.name = towerPrefab.name;
            newTower.transform.position = transform.position;
            newTower.transform.rotation = transform.rotation;
            // Destroi a torre velha
            Destroy(gameObject);
			EventManager.TriggerEvent("TowerBuild", newTower, null);
        }
    }

    /// <summary>
    /// Vende a torre por metade do preço.
    /// </summary>
    /// <param name="emptyPlacePrefab">prefab do espaço vazio.</param>
    public void SellTower(GameObject emptyPlacePrefab)
	{
		CloseActions();
		DefendersSpawner defendersSpawner = GetComponent<DefendersSpawner>();
        // Destroi os defensores na venda da torre
        if (defendersSpawner != null)
		{
			foreach (KeyValuePair<GameObject, Transform> pair in defendersSpawner.defPoint.activeDefenders)
			{
				Destroy(pair.Key);
			}
		}
		Price price = GetComponent<Price>();
		uiManager.AddGold(price.price / 2);
        // Local de construção de lugar
        GameObject newTower = Instantiate<GameObject>(emptyPlacePrefab, transform.parent);
		newTower.name = emptyPlacePrefab.name;
		newTower.transform.position = transform.position;
		newTower.transform.rotation = transform.rotation;
        // Destroi a torre velha
        Destroy(gameObject);
		EventManager.TriggerEvent("TowerSell", null, null);
	}

    /// <summary>
    /// Desativa o raycast da torre e feche a árvore de construção na pausa do jogo.
    /// </summary>
    /// <param name="obj">Objeto</param>
    /// <param name="param">Perimetro</param>
    private void GamePaused(GameObject obj, string param)
    {
        if (param == bool.TrueString) // Pausado
        {
            CloseActions();
        }
    }

    /// <summary>
    /// No clique do utilzador.
    /// </summary>
    /// <param name="obj">Objeto</param>
    /// <param name="param">perimetro.</param>
    private void UserClick(GameObject obj, string param)
    {
        if (obj == gameObject) // Se Esta torre é clicada
        {
            // Mostrar alcançe

            ShowRange(true);
			if (actions.activeSelf == false)
            {
                // Abre a árvore do edifício se não estiver
                OpenActions();
            }
        }
        else // Outro clique
        {
            // Oculta alcançe
            ShowRange(false);
            // Fecha árvore de construção ativa
            CloseActions();
        }
    }

    /// <summary>
    /// Exibe o alcance de ataque ou defesa da torre.
    /// </summary>
    /// <param name="condition">If set to <c>true</c> condition.</param>
	public void ShowRange(bool condition)
    {
        if (range != null)
        {
			range.SetActive(condition);
        }
    }
}
