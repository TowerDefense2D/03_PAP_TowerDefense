using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Operação do ícone da interface do usuário com feitiços (ações do usuário).
/// </summary>
public class UserActionIcon : MonoBehaviour
{
	// Recarga de feitiços
	public float cooldown = 10f;
	//  prefab do feitiço
	public GameObject userActionPrefab;
	// Ícone para estado destacado
	public GameObject highlightIcon;
	// Ícone para estado de espera
	public GameObject cooldownIcon;
	// Contador de cooldown (texto da interface do usuário)
	public Text cooldownText;
	public AudioSource audioSource;
	public AudioClip audioClip;

	// Estado da máquina
	private enum MyState
	{
		Active,
		Highligted,
		Cooldown
	}
	// Estado atual para esta instância
	private MyState myState = MyState.Active;
	// Ação do utilizador ativa, instanciada quando destacada
	private GameObject activeUserAction;
	// Contador de cooldown
	private float cooldownCounter;

	/// <summary>
	/// Ativa o enable event 
	/// </summary>
	void OnEnable()
	{
		EventManager.StartListening("UserUiClick", UserUiClick);
		EventManager.StartListening("ActionStart", ActionStart);
		EventManager.StartListening("ActionCancel", ActionCancel);
	}

	/// <summary>
	/// Ativa o disable event 
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("UserUiClick", UserUiClick);
		EventManager.StopListening("ActionStart", ActionStart);
		EventManager.StopListening("ActionCancel", ActionCancel);
	}

	/// <summary>
	/// Começa esta instância 
	/// </summary>
	void Start()
	{
		Debug.Assert(userActionPrefab && highlightIcon && cooldownIcon && cooldownText, "Wrong initial settings");
		StopCooldown();
	}

	/// <summary>
	/// Atualiza esta instância 
	/// </summary>
	void Update()
	{
		if (myState == MyState.Cooldown)
		{
			if (cooldownCounter > 0f)
			{
				cooldownCounter -= Time.deltaTime;
				UpdateCooldownText();
			}
			else if (cooldownCounter <= 0f)
			{
				StopCooldown();
			}
		}
	}

	/// <summary>
	/// Manipulador de cliques da interface do utilizador
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UserUiClick(GameObject obj, string param)
	{
		if (obj == gameObject)  // Clicou neste ícon
		{
			if (myState == MyState.Active)
			{
				highlightIcon.SetActive(true);
				activeUserAction = Instantiate(userActionPrefab);
				// Retorno de chamada para a classe derivada
				Clicked(activeUserAction);
				myState = MyState.Highligted;
			}
		}
		else if (myState == MyState.Highligted) // Clicou em outro UI
		{
			highlightIcon.SetActive(false);
			myState = MyState.Active;
		}
	}

	protected virtual void Clicked(GameObject effect)
	{

	}

	/// <summary>
	/// Manipulador de início de ação.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void ActionStart(GameObject obj, string param)
	{
		if (obj == activeUserAction)
		{
			activeUserAction = null;
			highlightIcon.SetActive(false);
			StartCooldown();
		}
	}

	/// <summary>
	/// Manipulador de cancelamento de ações.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void ActionCancel(GameObject obj, string param)
	{
		if (obj == activeUserAction)
		{
			activeUserAction = null;
			highlightIcon.SetActive(false);
			myState = MyState.Active;
		}
	}

	/// <summary>
	/// Inicia o cooldown.
	/// </summary>
	private void StartCooldown()
	{
		myState = MyState.Cooldown;
		cooldownCounter = cooldown;
		cooldownIcon.gameObject.SetActive(true);
		cooldownText.gameObject.SetActive(true);
	}

	/// <summary>
	/// Interrompe o cooldown.
	/// </summary>
	private void StopCooldown()
	{
		myState = MyState.Active;
		cooldownCounter = 0f;
		cooldownIcon.gameObject.SetActive(false);
		cooldownText.gameObject.SetActive(false);
	}

	/// <summary>
	/// Atualiza o texto do contador de cooldown.
	/// </summary>
	private void UpdateCooldownText()
	{
		cooldownText.text = ((int)Mathf.Ceil(cooldownCounter)).ToString();
	}
}
