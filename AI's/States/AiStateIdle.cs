using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permite o AI operar o estado parado 
/// </summary>
public class AiStateIdle : AiState
{
    /// <summary>
    /// Aumenta o estado do enter event 
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
	public override void OnStateEnter(AiState previousState, AiState newState)
    {
		// Para-se de se mexer e de se virar 
		if (aiBehavior.navAgent != null)
		{
			aiBehavior.navAgent.move = false;
			aiBehavior.navAgent.turn = false;
		}
		// Se a unidade tiver uma animação 
		if (anim != null && anim.runtimeAnimatorController != null)
		{
			// Procurar por um clip 
			foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
			{
				if (clip.name == "Idle")
				{
					// Ativar animação 
					anim.SetTrigger("idle");
					break;
				}
			}
		}
    }
}
