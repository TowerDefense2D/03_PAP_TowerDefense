using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor de descrição do nível.
/// </summary>
public class LevelDescriptionInspector : MonoBehaviour
{
	// Ícone do nível
	public Image icon;
	// Cabeçalho do nível
	public Text header;
	// Descrição do nível
	public Text description;
	// Atenção do nível
	public Text attention;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		Debug.Assert(icon && header && description && attention, "Wrong level description stuff settings");
	}
}
#endif
