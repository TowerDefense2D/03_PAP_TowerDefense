using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// O meu tipo de evento.
/// </summary>
[System.Serializable]
public class MyEvent : UnityEvent<GameObject, string>
{

}

/// <summary>
/// Sistema de mensagens.
/// </summary>
public class EventManager : MonoBehaviour
{
	// Singleton
	public static EventManager instance;

    // Lista de eventos
    private Dictionary<string, MyEvent> eventDictionary = new Dictionary<string, MyEvent>();

    /// <summary>
    /// Desperta esta instância.
    /// </summary>
    void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (instance != this)
		{
			Destroy(gameObject);
			return;
		}
	}

	/// <summary>
	/// Ativa o destroy event.
	/// </summary>
	void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

    /// <summary>
    /// Começa a ouvir o evento especificado.
    /// </summary>
    /// <param name="eventName">Nome do evento.</param>
    /// <param name="listener">Ouvinte(Jogador).</param>
    public static void StartListening(string eventName, UnityAction<GameObject, string> listener)
    {
		if (instance == null)
		{
			instance = FindObjectOfType(typeof(EventManager)) as EventManager;
			if (instance == null)
			{
				Debug.Log("Have no event manager on scene");
				return;
			}
		}
        MyEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new MyEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// Para de ouvir o evento especificado.
    /// </summary>
    /// <param name="eventName">Nome do Evento</param>
    /// <param name="listener">Ouvinte</param>
    public static void StopListening(string eventName, UnityAction<GameObject, string> listener)
    {
		if (instance == null)
		{
			return;
		}
        MyEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Aciona o evento especificado.
    /// </summary>
    /// <param name="eventName">Nome do Evento</param>
    /// <param name="obj">Objeto</param>
    /// <param name="param">Perimetro</param>
    public static void TriggerEvent(string eventName, GameObject obj, string param)
    {
		if (instance == null)
		{
			return;
		}
        MyEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(obj, param);
        }
    }
}
