using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permite o AI operar o andar para o seu destinatário 
/// </summary>
public class AiStateMove : AiState
{
	[Space(10)]

    // Vai para esta situação se o evento passivo for ativado 
	public AiState passiveAiState;
	// Termina o ponto final para andar +
	[HideInInspector]
	public Transform destination;

	/// <summary>
	/// Ativa esta instância 
	/// </summary>
	public override void Awake()
	{
		base.Awake();
		Debug.Assert (aiBehavior.navAgent, "Wrong initial parameters");
	}

    /// <summary>
    /// Aumenta o estado do enter event 
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
	public override void OnStateEnter(AiState previousState, AiState newState)
    {
        // Imprime o destinatário para o agente de navegação 
		aiBehavior.navAgent.destination = destination.position;
		// Começa a mexer-se 
		aiBehavior.navAgent.move = true;
		aiBehavior.navAgent.turn = true;
		// Se a unidade tiver uma animação 
        if (anim != null && anim.runtimeAnimatorController != null)
        {
			// Procura por um clip 
			foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
			{
				if (clip.name == "Move")
				{
					// Começa pela animação 
					anim.SetTrigger("move");
					break;
				}
			}
        }
    }

    /// <summary>
    /// Aumenta o estado do exit event 
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
	public override void OnStateExit(AiState previousState, AiState newState)
    {
		// Para-se de mexer 
		aiBehavior.navAgent.move = false;
		aiBehavior.navAgent.turn = false;
    }

    /// <summary>
    /// Atualização resolvida para esta instância 
    /// </summary>
    void FixedUpdate()
    {
        // se o destinatário for concluído 
        if ((Vector2)transform.position == (Vector2)destination.position)
        {
            // Olhar para a direção pedida 
			aiBehavior.navAgent.LookAt(destination.right);
            // Andar para um estado passivo 
            aiBehavior.ChangeState(passiveAiState);
        }
    }
}
