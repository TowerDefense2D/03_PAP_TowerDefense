using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Trajetória do voo da seta.
/// </summary>
public class BulletArrow : MonoBehaviour, IBullet
{
    // Quantidade de dano
    [HideInInspector] int damage = 1;
    // Tempo de vida máximo
    public float lifeTime = 3f;
    // Velocidade inicial
    public float speed = 3f;
    // Aceleração constante
    public float speedUpOverTime = 0.5f;
    // Se o alvo estiver próximo do que esta distância - ele será atingido
    public float hitDistance = 0.2f;
    // Deslocamento da trajetória balística (em distância até o alvo)
    public float ballisticOffset = 0.5f;
    // Não gira a bala durante o voo
    public bool freezeRotation = false;
    // Esta bala não causa dano a um único alvo. Apenas dano AOE(Area Of Effect) se for
    public bool aoeDamageOnly = false;

    // A partir desta posição a bala foi disparada
    private Vector2 originPoint;
    // Alvo mirado
    private Transform target;
    // Posição do último alvo
    private Vector2 aimPoint;
    // Posição atual sem deslocamento balístico
    private Vector2 myVirtualPosition;
    // Posição no último quadro
    private Vector2 myPreviousPosition;
    // Contador para o cálculo de aceleração
    private float counter;
    // Imagem desta bala
    private SpriteRenderer sprite;

    /// <summary>
    /// Define a quantidade de dano para esta bala.
    /// </summary>
    /// <param name="damage">Dano</param>
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    /// <summary>
    /// Obtém a quantidade de dano para esta bala.
    /// </summary>
    /// <returns>O dano.</returns>
    public int GetDamage()
	{
		return damage;
	}

    /// <summary>
    /// Dispara a bala em direção ao alvo especificado.
    /// </summary>
    /// <param name="target">Alvo</param>
    public void Fire(Transform target)
    {
        sprite = GetComponent<SpriteRenderer>();
        // Desativa o sprite no primeiro quadro porque ainda não sabemos a direção do voo
        sprite.enabled = false;
        originPoint = myVirtualPosition = myPreviousPosition = transform.position;
        this.target = target;
        aimPoint = target.position;
        // Destrói a bala após a vida
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Atualiza esta instância.
    /// </summary>
    void FixedUpdate ()
    {
		counter += Time.fixedDeltaTime;
        // Adiciona aceleração
        speed += Time.fixedDeltaTime * speedUpOverTime;
        if (target != null)
        {
            aimPoint = target.position;
        }
        // Calcula a distância do ponto de fogo para mirar
        Vector2 originDistance = aimPoint - originPoint;
        // Calcula a distância restante
        Vector2 distanceToAim = aimPoint - (Vector2)myVirtualPosition;
        // Move-se para o objetivo
        myVirtualPosition = Vector2.Lerp(originPoint, aimPoint, counter * speed / originDistance.magnitude);
        // Adiciona deslocamento balístico à trajetória
        transform.position = AddBallisticOffset(originDistance.magnitude, distanceToAim.magnitude);
        // Gira a bala em direção à trajetória
        LookAtDirection2D((Vector2)transform.position - myPreviousPosition);
        myPreviousPosition = transform.position;
        sprite.enabled = true;
        // Perto o suficiente para acertar
        if (distanceToAim.magnitude <= hitDistance)
        {
            if (target != null)
            {
                // Se a bala causar dano a um único alvo
                if (aoeDamageOnly == false)
				{
                    // Se o alvo pode receber dano
                    DamageTaker damageTaker = target.GetComponent<DamageTaker>();
					if (damageTaker != null)
					{
						damageTaker.TakeDamage(damage);
					}
				}
            }
            // Destruir bala
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Olha para a direção 2d.
    /// </summary>
    /// <param name="direction">Direção</param>
    private void LookAtDirection2D(Vector2 direction)
    {
        if (freezeRotation == false)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    /// <summary>
    /// Adiciona deslocamento balístico à trajetória.
    /// </summary>
    /// <returns>O deslocamento balístico.</returns>
    /// <param name="originDistance">distância de origem.</param>
    /// <param name="distanceToAim">Distância para mirar.</param>
    private Vector2 AddBallisticOffset(float originDistance, float distanceToAim)
    {
        if (ballisticOffset > 0f)
        {
            // Calcula deslocamento sinusal
            float offset = Mathf.Sin(Mathf.PI * ((originDistance - distanceToAim) / originDistance));
            offset *= originDistance;
            // Adiciona deslocamento à trajetória
            return (Vector2)myVirtualPosition + (ballisticOffset * offset * Vector2.up);
        }
        else
        {
            return myVirtualPosition;
        }
    }
}
