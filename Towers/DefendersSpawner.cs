using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permite que a torre crie novos objetos com cooldown.
/// </summary>
public class DefendersSpawner : MonoBehaviour
{
	// Cooldown entre spawns
	public float cooldown = 10f;
	// Número máximo de objetos criados no buffer
	public int maxNum = 2;
    // prefab de um objeto spawnado
    public GameObject prefab;
	// Posição para spawn
	public Transform spawnPoint;
	[HideInInspector]
	// Defende pontos para esta torre
	public DefendPoint defPoint;

	// Contador para cálculo de cooldown
	private float cooldownCounter;
	// Animador desta instância
	private Animator anim;

	/// <summary>
	/// Começa esta instância
	/// </summary>
	void Start()
	{
		anim = GetComponent<Animator>();
		Debug.Assert(spawnPoint, "Wrong initial settings");
		BuildingPlace buildingPlace = GetComponentInParent<BuildingPlace>();
		defPoint = buildingPlace.GetComponentInChildren<DefendPoint>();
		cooldownCounter = cooldown;
		// Atualiza todos os defensores existentes na construção da torre
		Dictionary<GameObject, Transform> oldDefenders = new Dictionary<GameObject, Transform>();
		foreach (KeyValuePair<GameObject, Transform> pair in defPoint.activeDefenders)
		{
			oldDefenders.Add(pair.Key, pair.Value);
		}
		defPoint.activeDefenders.Clear();
		foreach (KeyValuePair<GameObject, Transform> pair in oldDefenders)
		{
			// Cria um novo defensor no mesmo lugar
			Spawn(pair.Key.transform, pair.Value);
		}
		// Destroi velhos defensores
		foreach (KeyValuePair<GameObject, Transform> pair in oldDefenders)
		{
			Destroy(pair.Key);
		}
	}

    /// <summary>
    /// Atualiza esta instância
    /// </summary>
    void FixedUpdate()
    {
		cooldownCounter += Time.fixedDeltaTime;
        if (cooldownCounter >= cooldown)
        {
			// Tenta criar um novo objeto no cooldown
			if (TryToSpawn() == true)
            {
                cooldownCounter = 0f;
            }
            else
            {
                cooldownCounter = cooldown;
            }
        }
    }

	/// <summary>
	/// Tente criar um novo objeto.
	/// </summary>
	/// <returns><c>true</c>, if to spawn was tryed, <c>false</c> otherwise.</returns>
	private bool TryToSpawn()
    {
        bool res = false;
		// Se o número de objetos criado for menor que o número máximo permitido
		if ((prefab != null) && (defPoint.activeDefenders.Count < maxNum))
        {
			Transform destination = defPoint.GetFreeDefendPosition();
			// Se houver posição de defesa livre
			if (destination != null)
            {
				// Criar novo defensor
				Spawn(spawnPoint, destination);
                res = true;
            }
        }
        return res;
    }

	/// <summary>
	/// Spawn na posição e destino especificados.
	/// </summary>
	/// <param name="position">Posição</param>
	/// <param name="destination">Destino</param>
	private void Spawn(Transform position, Transform destination)
	{
		// Cria um novo objeto
		GameObject obj = Instantiate<GameObject>(prefab, position.position, position.rotation);
		obj.name = prefab.name;
		// Define a posição de destino
		obj.GetComponent<AiStateMove>().destination = destination;
		// Adiciona um objeto criado na lista
		defPoint.activeDefenders.Add(obj, destination);
		// Reproduz animação
		if (anim != null && anim.runtimeAnimatorController != null)
		{
			foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
			{
				if (clip.name == "Spawn")
				{
					anim.SetTrigger("spawn");
					break;
				}
			}
		}
	}
}
