using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manipulador de botões.
/// </summary>
public class ButtonHandler : MonoBehaviour
{
	public AudioClip audioClip;

	/// <summary>
	/// Botões pressionados.
	/// </summary>
	/// <param name="buttonName">Botão name.</param>
	public void ButtonPressed(string buttonName)
	{
		StartCoroutine(PressedCoroutine(buttonName));
	}

	/// <summary>
	/// Pressiona a corrotina.
	/// </summary>
	/// <returns>A corrotina</returns>
	/// <param name="buttonName">Nome do botão</param>
	private IEnumerator PressedCoroutine(string buttonName)
	{
		// Reproduz um efeito sonoro
		if (audioClip != null && AudioManager.instance != null)
		{
			Button button = GetComponent<Button>();
			button.interactable = false;
			AudioManager.instance.PlaySound(audioClip);
			// Espera pelo o fim do efeito sonoro
			yield return new WaitForSecondsRealtime(audioClip.length);
			button.interactable = true;
		}
		// Enviaa um evento global sobre premir o botão
		EventManager.TriggerEvent("ButtonPressed", gameObject, buttonName);
	}

	/// <summary>
	/// Ativa o destroy event
	/// </summary>
	void OnDestroy()
	{
		StopAllCoroutines();
	}
}
