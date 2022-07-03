using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permite os AI atacar os alvos 
/// </summary>
public class AiStateAttack : AiState
{
	[Space(10)]
    // Ataca o alvo mais perto do ponto de captura 
    public bool useTargetPriority = false;
    // Vai para este estado se o evento passivo for ativado 
	public AiState passiveAiState;

    // Alvo para o ataque 
    private GameObject target;
    // Lista com alvos potenciais encontrados durante este frame 
    private List<GameObject> targetsList = new List<GameObject>();
    // O meu tipo de ataque corpo a corpo se for
    private Attack meleeAttack;
    // O meu tipo de ataque com alcançe se for 
    private Attack rangedAttack;
    // Tipo do último ataque for feito 
    private Attack myLastAttack;
    // Permite aguardar um novo alvo para um quadro antes de sair deste estado
    private bool targetless;

    /// <summary>
    /// Ativa esta instância.
    /// </summary>
    public override void Awake()
    {
		base.Awake();
        meleeAttack = GetComponentInChildren<AttackMelee>() as Attack;
        rangedAttack = GetComponentInChildren<AttackRanged>() as Attack;
    }

	/// <summary>
	/// Aumenta o estado do enter event 
	/// </summary>
	/// <param name="previousState">Previous state.</param>
	/// <param name="newState">New state.</param>
	public override void OnStateEnter(AiState previousState, AiState newState)
	{
		// Para-se de mexer 
		if (aiBehavior.navAgent != null)
		{
			aiBehavior.navAgent.move = false;
		}
	}

    /// <summary>
    /// Aumenta o estado do exit event
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
	public override void OnStateExit(AiState previousState, AiState newState)
    {
        LoseTarget();
    }

    /// <summary>
    /// Atualização resolvida para esta instância 
    /// </summary>
    void FixedUpdate()
    {
        // Se não tiver nenhum alvo, tenta encontrar um novo 
        if ((target == null) && (targetsList.Count > 0))
        {
            target = GetTopmostTarget();
        }
        // Não há alvos há volta 
        if (target == null)
        {
            if (targetless == false)
            {
                targetless = true;
            }
            else
            {
                // Se não tiver destino mais de um quadro - sair deste estado
                aiBehavior.ChangeState(passiveAiState);
            }
        }
    }

    /// <summary>
    /// Obtém o destino de prioridade máxima da lista.
    /// </summary>
    /// <returns>The topmost target.</returns>
    private GameObject GetTopmostTarget()
    {
        GameObject res = null;
        if (useTargetPriority == true) // Obtenha o alvo com distância mínima até o ponto de captura
        {
            float minPathDistance = float.MaxValue;
            foreach (GameObject ai in targetsList)
            {
                if (ai != null)
                {
                    AiStatePatrol aiStatePatrol = ai.GetComponent<AiStatePatrol>();
                    float distance = aiStatePatrol.GetRemainingPath();
                    if (distance < minPathDistance)
                    {
                        minPathDistance = distance;
                        res = ai;
                    }
                }
            }
        }
        else // Obter o primeiro alvo da lista
        {
            res = targetsList[0];
        }
        // Lista clara de alvos em potencial
        targetsList.Clear();
        return res;
    }

    /// <summary>
    /// Perde o alvo atual.
    /// </summary>
    private void LoseTarget()
    {
        target = null;
        targetless = false;
        myLastAttack = null;
    }

	/// <summary>
	/// Aumenta o estado do trigger event 
	/// </summary>
	/// <param name="trigger">Trigger.</param>
	/// <param name="my">My.</param>
	/// <param name="other">Other.</param>
	public override bool OnTrigger(AiState.Trigger trigger, Collider2D my, Collider2D other)
	{
		if (base.OnTrigger(trigger, my, other) == false)
		{
			switch (trigger)
			{
			case AiState.Trigger.TriggerStay:
				TriggerStay(my, other);
				break;
			case AiState.Trigger.TriggerExit:
				TriggerExit(my, other);
				break;
			}
		}
		return false;
	}

    /// <summary>
    /// Aciona a estadia.
    /// </summary>
    /// <param name="my">My.</param>
    /// <param name="other">Other.</param>
	private void TriggerStay(Collider2D my, Collider2D other)
    {
        if (target == null) // Adicionar novo alvo à lista de alvos potenciais
        {
            targetsList.Add(other.gameObject);
        }
        else // Atacar o alvo atual
        {
            // Se este é meu alvo atual
            if (target == other.gameObject)
            {
                if (my.name == "MeleeAttack") // Se o alvo estiver no alcance de ataque corpo a corpo
                {
                    // Se eu tiver o tipo de ataque corpo a corpo
                    if (meleeAttack != null)
                    {
						if (aiBehavior.navAgent != null)
						{
                            // Olhar para o alvo
                            aiBehavior.navAgent.LookAt(target.transform);
						}
                        // Lembra-se do meu último tipo de ataque
                        myLastAttack = meleeAttack as Attack;
                        // Tenta fazer um ataque corpo a corpo
                        meleeAttack.TryAttack(other.transform);
                    }
                }
                else if (my.name == "RangedAttack") // Se o alvo estiver no alcance de ataque à distância
                {
                    // Se eu tiver o tipo de ataque à distância
                    if (rangedAttack != null)
                    {
                        // Se o alvo não estiver no alcance do ataque corpo a corpo
                        if ((meleeAttack == null)
                            || ((meleeAttack != null) && (myLastAttack != meleeAttack)))
                        {
							if (aiBehavior.navAgent != null)
							{
                                // Olhar para o alvo
                                aiBehavior.navAgent.LookAt(target.transform);
							}
                            // Lembra-se do meu último tipo de ataque
                            myLastAttack = rangedAttack as Attack;
                            // Tenta fazer um ataque à distância
                            rangedAttack.TryAttack(other.transform);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Aciona a saída.
    /// </summary>
    /// <param name="my">My.</param>
    /// <param name="other">Other.</param>
	private void TriggerExit(Collider2D my, Collider2D other)
    {
        if (other.gameObject == target)
        {
            //Perco meu alvo se ele sair do alcance de ataque
            LoseTarget();
        }
    }
}
