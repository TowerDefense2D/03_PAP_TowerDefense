using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// O menu principal funciona.
/// </summary>
public class MainMenu : MonoBehaviour
{
	// Nome da cena para iniciar ao clicar
	public string startSceneName;
	// Menu de créditos
	public GameObject creditsMenu;

	/// <summary>
	/// Ativa o enable event.
	/// </summary>
	void OnEnable()
	{
		EventManager.StartListening("ButtonPressed", ButtonPressed);
	}

	/// <summary>
	/// Ativa o disable event
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("ButtonPressed", ButtonPressed);
	}

	void Awake()
	{
		Debug.Assert(creditsMenu, "Wrong initial settings");
	}

	/// <summary>
	/// Manipulador de botões pressionado.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void ButtonPressed(GameObject obj, string param)
	{
		switch (param)
		{
		case "Quit":
			Application.Quit();
			break;
		case "Start":
			SceneManager.LoadScene(startSceneName);
			break;
		case "OpenCredits":
			creditsMenu.SetActive(true);
			break;
		case "CloseCredits":
			creditsMenu.SetActive(false);
			break;
		}
	}
}
