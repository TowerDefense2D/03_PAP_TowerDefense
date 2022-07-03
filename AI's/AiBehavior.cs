using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script principal para controlar todos os estados de AI
/// </summary>
public class AiBehavior : MonoBehaviour
{
    // Agente de navegação, se necessário
    [HideInInspector]
	public NavAgent navAgent;
    // Este estado será ativado no início
    public AiState defaultState;

    // Lista com todos os estados para este AI
    private List<AiState> aiStates = new List<AiState>();
    // O estado que era antes
    private AiState previousState;
    // Estado ativo
    private AiState currentState;

	/// <summary>
	/// Ativa esta instância 
	/// </summary>
	void Awake()
	{
		if (navAgent == null)
		{
            // Tenta encontrar o agente de navegação para este objeto
            navAgent = GetComponentInChildren<NavAgent>();
		}
	}

    /// <summary>
    /// Aumenta o evento de habilitação.
    /// </summary>
    void OnEnable()
	{
        // Habilita O  AI no AiBehavior ativado
        if (currentState != null && currentState.enabled == false)
		{
			EnableNewState();
		}
	}

    /// <summary>
    /// Aumenta o evento disable.
    /// </summary>
    void OnDisable()
	{
        // Desativa AI na desativação do AiBehavior
        DisableAllStates();
	}

    /// <summary>
    /// Ativa esta instância
    /// </summary>
    void Start()
    {
        // Obtenha todos os estados de AI deste gameobject
        AiState[] states = GetComponents<AiState>();
        if (states.Length > 0) 
        {
			foreach (AiState state in states)
            {
                // Adicionar estado à lista
                aiStates.Add(state);
            }
            if (defaultState != null)
            {
                // Define estados ativos e anteriores como estado padrão
                previousState = currentState = defaultState;
                if (currentState != null)
                {
                    // Vai para o estado ativo
                    ChangeState(currentState);
                }
                else
                {
                    Debug.LogError("Incorrect default AI state " + defaultState);
                }
            }
            else
            {
                Debug.LogError("AI have no default state");
            }
        } 
        else 
        {
            Debug.LogError("No AI states found");
        }
    }

    /// <summary>
    /// Define o AI para o estado padrão.
    /// </summary>
    public void GoToDefaultState()
    {
        previousState = currentState;
		currentState = defaultState;
        NotifyOnStateExit();
        DisableAllStates();
        EnableNewState();
        NotifyOnStateEnter();
    }

    /// <summary>
    /// Muda o estado do AI 
    /// </summary>
    /// <param name="state">State.</param>
	public void ChangeState(AiState state)
    {
		if (state != null)
        {
            // Tenta encontrar tal estado na lista
            foreach (AiState aiState in aiStates)
            {
                if (state == aiState)
                {
                    previousState = currentState;
                    currentState = aiState;
                    NotifyOnStateExit();
                    DisableAllStates();
                    EnableNewState();
                    NotifyOnStateEnter();
                    return;
                }
            }
            Debug.Log("No such state " + state);
            // Se não tiver esse estado - vai para o estado padrão
            GoToDefaultState();
            Debug.Log("Go to default state " + aiStates[0]);
        }
    }

    /// <summary>
    /// Desliga todos os componentes de estados de AI.
    /// </summary>
    private void DisableAllStates()
    {
		foreach (AiState aiState in aiStates) 
        {
			aiState.enabled = false;
        }
    }

    /// <summary>
    /// Ativa o componente de estado AI ativo.
    /// </summary>
    private void EnableNewState()
    {
		currentState.enabled = true;
    }

    /// <summary>
    /// Envia notificação OnStateExit para o estado anterior.
    /// </summary>
    private void NotifyOnStateExit()
    {
		previousState.OnStateExit(previousState, currentState);
    }

    /// <summary>
    /// Envia a notificação OnStateEnter para o novo estado.
    /// </summary>
    private void NotifyOnStateEnter()
    {
		currentState.OnStateEnter(previousState, currentState);
    }

    /// <summary>
    /// Ativa o evento de trigger.
    /// </summary>
    /// <param name="trigger">Trigger.</param>
    /// <param name="my">My.</param>
    /// <param name="other">Other.</param>
    public void OnTrigger(AiState.Trigger trigger, Collider2D my, Collider2D other)
    {
		if (currentState == null)
		{
			Debug.Log("Current sate is null");
		}
		currentState.OnTrigger(trigger, my, other);
    }
}
