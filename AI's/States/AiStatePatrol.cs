using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permite o AI se mover para um caminho especifíco
/// </summary>
public class AiStatePatrol : AiState
{
	[Space(10)]
	[HideInInspector]
    // Caminho especifíco 
    public Pathway path;
    // precisa de um caminho com loop depois de o último ponto for alcançado 
    public bool loop = false;
	[HideInInspector]
	// Destinho Atual 
	public Waypoint destination;

    /// <summary>
    /// Ativar esta instância 
    /// </summary>
	public override void Awake()
    {
		base.Awake();
		Debug.Assert (aiBehavior.navAgent, "Wrong initial parameters");
    }

    /// <summary>
    /// Aumenta o estado enter event 
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
	public override void OnStateEnter(AiState previousState, AiState newState)
    {
        if (path == null)
        {
            // Se nã tiver nenhum caminho - tenta encontrá-lo 
            path = FindObjectOfType<Pathway>();
            Debug.Assert(path, "Have no path");
        }
        if (destination == null)
        {
            // Poe o próximo caminho específico no camnho principal 
            destination = path.GetNearestWaypoint(transform.position);
        }
        // Insere o destino para o agente de navegação 
		aiBehavior.navAgent.destination = destination.transform.position;
		// Começa a mover-se 
		aiBehavior.navAgent.move = true;
		aiBehavior.navAgent.turn = true;
		// Se a unidade tiver um animador 
		if (anim != null && anim.runtimeAnimatorController != null)
        {
			// Procurar pelo clip 
			foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
			{
				if (clip.name == "Move")
				{
					// Ativar a animação 
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
		// Para-se de se mexer 
		aiBehavior.navAgent.move = false;
		aiBehavior.navAgent.turn = false;
    }

    /// <summary>
    /// Resolvido a atualização para esta instância 
    /// </summary>
    void FixedUpdate()
    {
        if (destination != null)
        {
            // se o destino for chegado 
            if ((Vector2)destination.transform.position == (Vector2)transform.position)
            {
                // Poe o próximo caminho específico no camnho principal 
                destination = path.GetNextWaypoint (destination, loop);
                if (destination != null)
                {
                    // Insere o destino para o agente de navegação 
                    aiBehavior.navAgent.destination = destination.transform.position;
                }
            }
        }
    }

    /// <summary>
    /// Consegue a distância restante do caminho especifíco 
    /// </summary>
    /// <returns>O caminho restante</returns>
    public float GetRemainingPath()
    {
        Vector2 distance = destination.transform.position - transform.position;
        return (distance.magnitude + path.GetPathDistance(destination));
    }

	/// <summary>
	/// Atualiza o destino 
	/// </summary>
	/// <param name="getNearestWaypoint">If set to <c>true</c> pegar o caminho especifíco mais perto automaticamente</param>
	public void UpdateDestination(bool getNearestWaypoint)
	{
		if (getNearestWaypoint == true)
		{
			// Conseguir o próximo waypoint do meu caminho 
			destination = path.GetNearestWaypoint(transform.position);
		}
		if (enabled == true)
		{
            // Insere o destino para o agente de navegação 
            aiBehavior.navAgent.destination = destination.transform.position;
		}
	}
}
