using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A unidade irá acelerar se não houver nenhuma unidade próxima.
/// </summary>
public class AloneSpeedUp : AiFeature
{
	// Acelera a quantidade quando estiver sozinho
	public float speedUpAmount = 2f;

	// EffectControl desta instância
	private EffectControl effectControl;

	/// <summary>
	/// Começa esta instância
	/// </summary>
	void Start()
	{
		effectControl = GetComponentInParent<EffectControl>();
		Debug.Assert(effectControl, "Wrong initial settings");
	}

	public void OnTriggerAloneStart()
	{
		effectControl.AddConstantEffect("Speed", speedUpAmount, null);
	}

	public void OnTriggerAloneEnd()
	{
		effectControl.RemoveConstantEffect("Speed", speedUpAmount);
	}
}
