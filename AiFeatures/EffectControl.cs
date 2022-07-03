using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
/// Controla os efeitos na unidade. Todos os efeitos devem ser aplicados através deste script.
/// </summary>
public class EffectControl : MonoBehaviour
{
	// Descrição com dados do efeito ativo
	private class EffectDescriptor
	{
		public string name; // Nome do efeito
		public GameObject fx; // Efeito visual
		public Dictionary<float, float> effects = new Dictionary<float, float>(); // Lista com durações do efeito: 1d - modificador de efeito, 2º - tempo expirado,
	}
	// Gatilhos para manipulação de efeitos
	private enum EffectTrigger
	{
		Added,
		NewModifier,
		ModifierExpired,
		Removed
	}
	// Lista com efeitos ativos
	private List<EffectDescriptor> effectList = new List<EffectDescriptor>();

	/// <summary>
	/// Adiciona o efeito.
	/// </summary>
	/// <param name="effectName">Nome do efeito.</param>
	/// <param name="modifier">Modificador.</param>
	/// <param name="duration">Duração (se == MaxValue - este é um efeito constante).</param>
	/// <param name="fxPrefab">Fx prefab.</param>
	public void AddEffect(string effectName, float modifier, float duration, GameObject fxPrefab)
	{
		// Obtém um manipulador de efeitos do nome do efeito
		MethodInfo methodInfo = GetType().GetMethod(effectName, BindingFlags.Instance | BindingFlags.NonPublic);
		if (methodInfo != null)
		{
			EffectDescriptor hit = null;
			// Pesquisa se o mesmo efeito já está ativo
			foreach (EffectDescriptor sem in effectList)
			{
				if (effectName == sem.name)
				{
					hit = sem;
					break;
				}
			}

			if (hit == null) // Não tem esse efeito ativo
			{
				EffectDescriptor newSem = new EffectDescriptor();
				newSem.name = effectName;
				// Criar efeito
				newSem.effects.Add(modifier, Time.time + duration);
				// Adiciona-o à lista
				effectList.Add(newSem);
				// Reproduz um efeito visual
				if (fxPrefab != null)
				{
					newSem.fx = Instantiate(fxPrefab, transform);
				}
				// Efeito adicionado
				methodInfo.Invoke(this, new object[] {EffectTrigger.Added, modifier});
			}
			else // Já tem o mesmo efeito
			{
				// Adiciona-o à lista atual
				hit.effects.Add(modifier, Time.time + duration);
				// Mais um modificador adicionado ao efeito existente
				methodInfo.Invoke(this, new object[] {EffectTrigger.NewModifier, modifier});
			}
		}
		else
		{
			Debug.LogError("Unknown effect - " + effectName);
		}
	}

	public void AddConstantEffect(string effectName, float modifier, GameObject fxPrefab)
	{
		AddEffect(effectName, modifier, float.MaxValue, fxPrefab);
	}

	public bool RemoveConstantEffect(string effectName, float modifier)
	{
		bool res = false;
		foreach (EffectDescriptor desc in effectList)
		{
			if (desc.name == effectName)
			{
				List<float> expiredTimes = new List<float>(desc.effects.Keys);
				for (int i = 0; i < expiredTimes.Count; ++i)
				{
					if (expiredTimes[i] == modifier && desc.effects[expiredTimes[i]] == float.MaxValue)
					{
						res = true;
						desc.effects[expiredTimes[i]] = Time.time;
						break;
					}
				}
			}
		}
		return res;
	}

	public bool RemoveEffects(string effectName)
	{
		bool res = false;
		foreach (EffectDescriptor desc in effectList)
		{
			if (desc.name == effectName)
			{
				res = true;
				List<float> expiredTimes = new List<float>(desc.effects.Keys);
				for (int i = 0; i < expiredTimes.Count; ++i)
				{
					desc.effects[expiredTimes[i]] = Time.time;
				}
				break;
			}
		}
		return res;
	}

	/// <summary>
	/// Manipulador de atualização corrigido.
	/// </summary>
	void FixedUpdate()
	{
		float currentTime = Time.time;
		// Lista de efeitos vazios
		List<EffectDescriptor> emptyEffects = new List<EffectDescriptor>();
		// Vê os efeitos ativos atuais
		foreach (EffectDescriptor desc in effectList)
		{
			// Lista para duração de efeitos expirados
			List<float> expiredList = new List<float>();
			foreach (KeyValuePair<float, float> effectData in desc.effects)
			{
				// A duração do efeito expirou (se duração == MaxValue - este é um efeito constante)
				if (effectData.Value != float.MaxValue && currentTime > effectData.Value)
				{
					// Adiciona à lista expirada
					expiredList.Add(effectData.Key);
					// Manipulador expirado do modificador de efeito da chamada
					MethodInfo methodInfo = GetType().GetMethod(desc.name, BindingFlags.Instance | BindingFlags.NonPublic);
					methodInfo.Invoke(this, new object[] {EffectTrigger.ModifierExpired, effectData.Key});
				}
			}
			// Remove os efeitos expirados da lista
			foreach (float expired in expiredList)
			{
				desc.effects.Remove(expired);
			}
			// Se o efeito não tiver mais duração
			if (desc.effects.Count <= 0)
			{
				// Manipulador removido do efeito de chamada
				MethodInfo methodInfo = GetType().GetMethod(desc.name, BindingFlags.Instance | BindingFlags.NonPublic);
				methodInfo.Invoke(this, new object[] {EffectTrigger.Removed, 0f});
				// Adiciona-o à lista de efeitos vazia
				emptyEffects.Add(desc);
			}
		}
		// Remover efeitos vazios da lista
		foreach (EffectDescriptor emptyEffect in emptyEffects)
		{
			if (emptyEffect.fx != null)
			{
				// Remova o efeito visual no final do efeito
				Destroy(emptyEffect.fx);
			}
			effectList.Remove(emptyEffect);
		}
	}

	/// <summary>
	/// Determina se esta instância tem um efeito ativo no effectName especificado.
	/// </summary>
	/// <returns><c>true</c> se esta instância tiver efeito ativo, o effectName especificado; senão, <c>false</c>.</returns>
	/// <param name="effectName">Nome do Efeito</param>
	public bool HasActiveEffect(string effectName)
	{
		bool res = false;
		foreach (EffectDescriptor effect in effectList)
		{
			if (effect.name == effectName)
			{
				res = true;
				break;
			}
		}
		return res;
	}

	/// <summary>
	/// Ativa o evento destroy.
	/// </summary>
	void OnDestroy()
	{
		StopAllCoroutines();
	}

	/// <summary>
	/// Manipulador de efeitos de atordoamento (bool).
	/// </summary>
	/// <param name="trigger">Gatilho</param>
	/// <param name="modifier">Modificador.</param>
	private void Stun(EffectTrigger trigger, float modifier)
	{
		AiBehavior aiBehavior = GetComponent<AiBehavior>();
		NavAgent navAgent = GetComponent<NavAgent>();
		switch (trigger)
		{
		case EffectTrigger.Added:
			aiBehavior.enabled = false;
			navAgent.enabled = false;
			break;
		case EffectTrigger.Removed:
			aiBehavior.enabled = true;
			navAgent.enabled = true;
			break;
		}
	}

	/// <summary>
	/// Manipulador de efeito de velocidade (float).
	/// </summary>
	/// <param name="trigger">Gatilho.</param>
	/// <param name="modifier">Modificador (não pode ser 0f).</param>
	private void Speed(EffectTrigger trigger, float modifier)
	{
		NavAgent navAgent = GetComponent<NavAgent>();
		switch (trigger)
		{
		case EffectTrigger.Added:
		case EffectTrigger.NewModifier:
			navAgent.speed *= 1 + modifier;
			break;
		case EffectTrigger.ModifierExpired:
			navAgent.speed /= 1 + modifier;
			break;
		}
	}
}
