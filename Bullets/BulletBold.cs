using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBold : MonoBehaviour, IBullet
{
    // Quantidade de dano
    [HideInInspector] public int damage = 1;
    // Tempo de vida máximo
    public float lifeTime = 3f;
    // Velocidade inicial
    public float speed = 3f;
    // Aceleração constante
    public float speedUpOverTime = 0.5f;
    // Se o alvo estiver próximo do que esta distância - ele será atingido
    public float hitDistance = 0.2f;
    // Deslocamento da trajetória balística (em distância até o alvo)
    public float ballisticOffset = 0.1f;
    // A bala voará através do alvo (na distância de origem até o alvo)
    public float penetrationRatio = 0.3f;
    // Tags de objetos permitidos para deteção de colisão
    public List<string> tags = new List<string>();

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
    // Contador para cálculo de aceleração
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
    /// <returns>O dano</returns>
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
		Debug.Assert(sprite, "Wrong initial settings");
        // Desativa o sprite no primeiro quadro (frame) porque ainda não sabemos a direção do voo
        sprite.enabled = false;
        originPoint = myVirtualPosition = myPreviousPosition = transform.position;
        this.target = target;
        aimPoint = GetPenetrationPoint(target.position);
        // Destroi a bala após a vida
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Atualize esta instância.
    /// </summary>
    void FixedUpdate()
    {
		counter += Time.fixedDeltaTime;
        // Adiciona aceleração
        speed += Time.fixedDeltaTime * speedUpOverTime;
        if (target != null)
        {
            aimPoint = GetPenetrationPoint(target.position);
        }
        // Calcula a distância do ponto de fogo para mirar
        Vector2 originDistance = aimPoint - originPoint;
        // Calcular a distância restante
        Vector2 distanceToAim = aimPoint - (Vector2)myVirtualPosition;
        // Move-se para o objetivo
        myVirtualPosition = Vector2.Lerp(originPoint, aimPoint, counter * speed / originDistance.magnitude);
        // Adiciona deslocamento balístico à trajetória
        transform.position = AddBallisticOffset(originDistance.magnitude, distanceToAim.magnitude);
        // Gira a bala em direção à trajetória
        LookAtDirection2D((Vector2)transform.position - myPreviousPosition);
        myPreviousPosition = transform.position;
        sprite.enabled = true;
        // Se estiver perto o suficiente para acertar
        if (distanceToAim.magnitude <= hitDistance)
        {
            // Destroí a  bala
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Olha para direção 2d.
    /// </summary>
    /// <param name="direction">Direção</param>
    private void LookAtDirection2D(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Calcula ponto de mira com deslocamento de penetração
    /// </summary>
    /// <returns>O ponto de penetração.</returns>
    /// <param name="targetPosition">Posição do alvo.</param>
    private Vector2 GetPenetrationPoint(Vector2 targetPosition)
    {
        Vector2 penetrationVector = targetPosition - originPoint;
        return originPoint + penetrationVector * (1f + penetrationRatio);
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
            // Adiciona o deslocamento à trajetória
            return (Vector2)myVirtualPosition + (ballisticOffset * offset * Vector2.up);
        }
        else
        {
            return myVirtualPosition;
        }
    }

    /// <summary>
    /// Alvos de dano na entrada do gatilho
    /// </summary>
    /// <param name="other">Outro</param>
    void OnTriggerEnter2D(Collider2D other)
    {
		if (IsTagAllowed(other.tag) == true)
		{
            // Se o alvo pode receber dano
            DamageTaker damageTaker = other.GetComponent<DamageTaker> ();
	        if (damageTaker != null)
	        {
	            damageTaker.TakeDamage(damage);
	        }
		}
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
