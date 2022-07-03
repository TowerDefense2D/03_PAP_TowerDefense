using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Filtro dinâmico para máscara de colisão AI
/// </summary>
public class AiTriggerCollider : MonoBehaviour
{
    // Tags de objetos permitidos para detetar a colisão
    public List<string> tags = new List<string>();

    // Meu colisor
    private Collider2D col;
    // Componente de comportamento de IA no objeto pai
    private AiBehavior aiBehavior;

    /// <summary>
    /// Desperta esta instância.
    /// </summary>
    void Awake()
    {
        col = GetComponent<Collider2D>();
        aiBehavior = GetComponentInParent<AiBehavior>();
        Debug.Assert(col && aiBehavior, "Wrong initial parameters");
		col.enabled = false;
    }

    /// <summary>
    /// Inicia esta instância.
    /// </summary>
    void Start()
	{
		col.enabled = true;
	}

    /// <summary>
    /// Determina se esta instância tem permissão de tag para a tag especificada.
    /// </summary>
    /// <returns><c>true</c> se esta instância é tag permitida a tag especificada; senão, <c>false</c>.</returns>
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

    /// <summary>
    /// Ativa o evento enter2d do gatilho.
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsTagAllowed(other.tag) == true)
        {
            // Notifica o comportamento do AI ​​sobre este evento
            aiBehavior.OnTrigger(AiState.Trigger.TriggerEnter, col, other);
        }
    }

    /// <summary>
    /// Ativa o evento de trigger stay2d.
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        if (IsTagAllowed(other.tag) == true)
        {
            // Notifica o comportamento da AI ​​sobre este evento
            aiBehavior.OnTrigger(AiState.Trigger.TriggerStay, col, other);
        }
    }

    /// <summary>
    /// Ativa o evento exit2d do gatilho.
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (IsTagAllowed(other.tag) == true)
        {
            // Notifica o comportamento do AI ​​sobre este evento
            aiBehavior.OnTrigger(AiState.Trigger.TriggerExit, col, other);
        }
    }
}
