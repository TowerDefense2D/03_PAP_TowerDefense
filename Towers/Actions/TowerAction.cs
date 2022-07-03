using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aula básica para ação de torre.
/// </summary>
public class TowerAction : MonoBehaviour
{
	// Ícon para estado ativado
	public GameObject enabledIcon;

	/// <summary>
	/// Ativa o enable event.
	/// </summary>
	void OnEnable()
	{
		EventManager.StartListening("UserUiClick", UserUiClick);
	}

	/// <summary>
	/// Ativa o disable event.
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("UserUiClick", UserUiClick);
	}

	/// <summary>
	/// Na UI do utilizador, clique.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UserUiClick(GameObject obj, string param)
	{
		// Se for clicado neste ícon
		if (obj == gameObject)
		{
			if (enabledIcon.activeSelf == true)
			{
				Clicked();
			}
		}
	}

	/// <summary>
	/// Clica nesta instância
	/// </summary>
	protected virtual void Clicked()
	{

	}
}
