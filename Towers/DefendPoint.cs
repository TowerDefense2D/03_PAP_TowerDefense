using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Posição para os defensores.
/// </summary>
public class DefendPoint : MonoBehaviour
{
	// Prefab para defender o ponto
	public GameObject defendPointPrefab;
	// Prefab para a bandeira
	public GameObject defendFlagPrefab;
	// Direção do caminho próximo a esta torre (para orientação dos defensores)
	public bool clockwiseDirection = false;
	[HideInInspector]
	// Lista de defensores ativos
	public Dictionary<GameObject, Transform> activeDefenders = new Dictionary<GameObject, Transform>();

	// Sinalizador ativo
	private GameObject activeDefendFlag;
	// Lista com locais de defesa para este ponto de defesa
	private List<Transform> defendPlaces = new List<Transform>();
	// Estado da máquina
	private enum MyState
	{
		Inactive,
		Active
	}
	// estado atua
	private MyState myState = MyState.Inactive;
	// Local de construção
	private BuildingPlace buildingPlace;
	// Torre
	private Tower tower;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		EventManager.StartListening("UserClick", UserClick);
		EventManager.StartListening("UserUiClick", UserUiClick);
		EventManager.StartListening("UnitDie", UnitDie);
	}

	/// <summary>
	/// Ativa o disable event
	/// </summary>
	void OnDisable()
	{
		EventManager.StopListening("UserClick", UserClick);
		EventManager.StopListening("UserUiClick", UserUiClick);
		EventManager.StopListening("UnitDie", UnitDie);
	}

	/// <summary>
	/// Desperta esta instância
	/// </summary>
	void Awake()
	{
		Debug.Assert(defendPointPrefab && defendFlagPrefab, "Wrong initial settings");
		// Obtém lugares de defesa do s prefabs de ponto de defesa e coloca-os em cena
		foreach (Transform defendPlace in defendPointPrefab.transform)
		{
			Instantiate(defendPlace.gameObject, transform);
		}
		// Cria lista de locais de defesa
		foreach (Transform child in transform)
		{
			defendPlaces.Add(child);
		}
		BuildingPlace buildingPlace = GetComponentInParent<BuildingPlace>();
		LookAtDirection2D((Vector2)(buildingPlace.transform.position - transform.position));
	}

	/// <summary>
	/// Obtém a lista de pontos de defesa.
	/// </summary>
	/// <returns>Pontos de defesa.</returns>
	public List<Transform> GetDefendPoints()
    {
		return defendPlaces;
    }

	/// <summary>
	/// Define a bandeira de defesa visível.
	/// </summary>
	/// <param name="enabled">If set to <c>true</c> enabled.</param>
	public void SetVisible(bool enabled)
	{
		if (enabled == true)
		{
			if (myState == MyState.Inactive)
			{
				buildingPlace = GetComponentInParent<BuildingPlace>();
				tower = buildingPlace.GetComponentInChildren<Tower>();
				// Mostrar bandeira de defesa
				activeDefendFlag = Instantiate(defendFlagPrefab);
				activeDefendFlag.transform.position = transform.position;
				myState = MyState.Active;
			}
		}
		else
		{
			if (myState == MyState.Active)
			{
				myState = MyState.Inactive;
				// Ocultar alcance de defesa
				tower.ShowRange(false);
				// Ocultar bandeira
				Destroy(activeDefendFlag);
			}
		}
	}

	/// <summary>
	/// Na UI do utilizador, clica.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UserUiClick(GameObject obj, string param)
	{
		if (myState == MyState.Active)
		{
			SetVisible(false);
		}
	}

	/// <summary>
	/// Manipulador de cliques do utilizador.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UserClick(GameObject obj, string param)
	{
		if (myState == MyState.Active)
		{
			myState = MyState.Inactive;
			Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 delta = position - (Vector2)tower.transform.position;
			delta = Vector2.ClampMagnitude(delta, tower.range.transform.localScale.x);
			transform.position = tower.transform.position + (Vector3)delta;
			LookAtDirection2D((Vector2)(tower.transform.position - transform.position));
			SetVisible(false);
			// Move os defensores para a nova posição da bandeira
			foreach (KeyValuePair<GameObject, Transform> pair in activeDefenders)
			{
				AiBehavior aiBehavior = pair.Key.GetComponent<AiBehavior>();
				aiBehavior.ChangeState(aiBehavior.GetComponent<AiStateMove>());
			}
			Destroy(activeDefendFlag);
		}
	}

	/// <summary>
	/// Aumenta em cada dado de unidade.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UnitDie(GameObject obj, string param)
	{
		// Se este for um objeto da minha lista
		if (activeDefenders.ContainsKey(obj) == true)
		{
			// Remove-o da lista
			activeDefenders.Remove(obj);
		}
	}

	/// <summary>
	/// Obtém a lista de defensores.
	/// </summary>
	/// <returns>Lista de defensores</returns>
	public List<GameObject> GetDefenderList()
	{
		List<GameObject> res = new List<GameObject>();
		foreach (KeyValuePair<GameObject, Transform> pair in activeDefenders)
		{
			res.Add(pair.Key);
		}
		return res;
	}

	/// <summary>
	/// Obtém a posição de defesa livre se for.
	/// </summary>
	/// <returns>A posição de defesa livre.</returns>
	/// <param name="index">Index.</param>
	public Transform GetFreeDefendPosition()
	{
		Transform res = null;
		List<Transform> points = GetDefendPoints();
		foreach (Transform point in points)
		{
			// Se este ponto já não estiver ocupado
			if (activeDefenders.ContainsValue(point) == false)
			{
				res = point;
				break;
			}
		}
		return res;
	}

	/// <summary>
	/// Olha para direction2d.
	/// </summary>
	/// <param name="direction">Direção</param>
	private void LookAtDirection2D(Vector2 direction)
	{
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		float offset = clockwiseDirection == false ? 90f : -90f;
		transform.rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
	}
}
