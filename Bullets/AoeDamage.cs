using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dano de área de efeito (AOE) ao destruir.
/// </summary>
public class AoeDamage : MonoBehaviour
{
    // Percentagem de dano AOE em parte do dano Ibullet. 0f = 0%, 1f = 100%
    public float aoeDamageRate = 1f;
    // Raio da área
    public float radius = 0.3f;
    // prefab da explosão
    public GameObject explosion;
    // Duração visual da explosão
    public float explosionDuration = 1f;
    // Efeito sonoro
    public AudioClip sfx;
    // Tags de objetos permitidos para deteção de colisão
    public List<string> tags = new List<string>();

    // Componente Ibullet deste gameObject obtém a quantidade de dano
    private IBullet bullet;
    // A cena está fechada agora. Proibido criar novos objetos ao destruir
    private bool isQuitting;

    /// <summary>
    /// Desperta esta instância.
    /// </summary>
    void Awake()
	{
		bullet = GetComponent<IBullet>();
		Debug.Assert(bullet != null, "Wrong initial settings");
	}

    /// <summary>
    /// Ativa o enable event.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("SceneQuit", SceneQuit);
    }

    /// <summary>
    /// Ativa o disable event. 
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening("SceneQuit", SceneQuit);
    }

    /// <summary>
    /// Ativa a aplicação quit event 
    /// </summary>
    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    /// <summary>
    /// Ativa o destroy event 
    /// </summary>
    void OnDestroy()
    {
        // Se a cena estiver em andamento
        if (isQuitting == false)
        {
            // Encontra todos os colidores no raio especificado
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (Collider2D col in cols)
            {
				if (IsTagAllowed(col.tag) == true)
				{
                    // Se o alvo pode receber dano
                    DamageTaker damageTaker = col.gameObject.GetComponent<DamageTaker>();
					if (damageTaker != null)
					{
                        // Alvo recebe dano igual ao dano de bala * Taxa de dano AOE
                        damageTaker.TakeDamage((int)(Mathf.Ceil(aoeDamageRate * (float)bullet.GetDamage())));
					}
				}
            }
            if (explosion != null)
            {
                // Cria efeito visual de explosão
                Destroy(Instantiate<GameObject>(explosion, transform.position, transform.rotation), explosionDuration);
            }
			if (sfx != null && AudioManager.instance != null)
			{
				// Ativa o sfx
				AudioManager.instance.PlaySound(sfx);
			}
        }
    }

    /// <summary>
    /// Levanta na cena quit.
    /// </summary>
    /// <param name="obj">Objeto</param>
    /// <param name="param">Perimetro</param>
    private void SceneQuit(GameObject obj, string param)
    {
        isQuitting = true;
    }

    /// <summary>
    /// Determina se esta instância tem permissão de tag para a tag especificada.
    /// </summary>
    /// <returns><c>true</c> se esta instância é tag permitida a tag especificada; senão <c>false</c>.</returns>
    /// <param name="tag">Tag.</param>
    private bool IsTagAllowed(string tag)
	{
		bool res = false;
		if (tags.Count > 0)
		{
			foreach (string str in tags)
			{
				if (str == tag)
				{
					res = true;
					break;
				}
			}
		}
		else
		{
			res = true;
		}
		return res;
	}
}
