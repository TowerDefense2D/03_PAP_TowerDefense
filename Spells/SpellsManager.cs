using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla os feitiços permitidos para este nível.
/// </summary>
public class SpellsManager : MonoBehaviour
{
	// Pasta para feitiços

	public Transform spellsFolder;

	/// <summary>
	/// Começar esta instância
	/// </summary>
	void Start()
	{
		LevelManager levelManager = FindObjectOfType<LevelManager>();
		Debug.Assert(spellsFolder && levelManager, "Wrong initial settings");
		foreach (UserActionIcon spell in spellsFolder.GetComponentsInChildren<UserActionIcon>())
		{
			Destroy(spell.gameObject);
		}
		// Adicionar feitiços permitidos na pasta da interface do utlizador dos feitiços
		foreach (GameObject spell in levelManager.allowedSpells)
		{
			Instantiate(spell, spellsFolder);
		}
	}
}
