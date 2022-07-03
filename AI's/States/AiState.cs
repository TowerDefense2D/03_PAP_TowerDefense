using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Classe básica para o estado do AI.
/// </summary>
public class AiState : MonoBehaviour
{
	// Tipos de trigger permitidos para transações de estado de IA
	public enum Trigger
	{
		TriggerEnter,	// No colidor entra
		TriggerStay,	// No colidor fica
		TriggerExit,	// No colidor sai 
		Damage,			// No dano recebido 
		Cooldown,       // Em algum cooldown expirado 
		Alone           // Quando nenhum outro colidor se cruza durante o tempo
	}

	[Serializable]
	// Permite especificar a mudança de estado do AI em qualquer gatilho
	public class AiTransaction
	{
		public Trigger trigger;
		public AiState newState;
	}
	// Lista com transações especificadas para este estado de AI
	public AiTransaction[] specificTransactions;

	// Controlador de animação para este AI
	protected Animator anim;
	// Comportamento de AI deste objeto
	protected AiBehavior aiBehavior;

	/// <summary>
	/// Ativa esta instância 
	/// </summary>
	public virtual void Awake()
	{
		aiBehavior = GetComponent<AiBehavior> ();
		anim = GetComponentInParent<Animator>();
		Debug.Assert (aiBehavior, "Wrong initial parameters");
	}

	/// <summary>
	/// Aumenta o estado do enter event .
	/// </summary>
	/// <param name="previousState">Previous state.</param>
	/// <param name="newState">New state.</param>
	public virtual void OnStateEnter(AiState previousState, AiState newState)
	{
		
	}

	/// <summary>
	/// Aumenta o estado do exit event 
	/// </summary>
	/// <param name="previousState">Previous state.</param>
	/// <param name="newState">New state.</param>
	public virtual void OnStateExit(AiState previousState, AiState newState)
	{

	}

	/// <summary>
	/// Aumenta o estado do trigger event 
	/// </summary>
	/// <param name="trigger">Trigger.</param>
	/// <param name="my">My.</param>
	/// <param name="other">Other.</param>
	public virtual bool OnTrigger(Trigger trigger, Collider2D my, Collider2D other)
	{
		bool res = false;
		// Verifica se este estado de AI possui transações específicas para este acionador
		foreach (AiTransaction transaction in specificTransactions)
		{
			if (trigger == transaction.trigger)
			{
				aiBehavior.ChangeState(transaction.newState);
				res = true;
				break;
			}
		}
		return res;
	}
}
