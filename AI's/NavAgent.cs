using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Operação de movimentação e giro.
/// </summary>
public class NavAgent : MonoBehaviour
{
    // Velocidade em m/s (milisegundos)
    public float speed = 1f;
    // Consegue mover
	[HideInInspector]
    public bool move = true;
    // Consegues virar 
	[HideInInspector]
    public bool turn = true;
    // Posição de destino
    [HideInInspector]
    public Vector2 destination;
    // Vetor de velocidade
    [HideInInspector]
    public Vector2 velocity;

	private Animator anim;
    // Posição no último frame
    private Vector2 prevPosition;

    /// <summary>
    /// Ativa o evento de habilitação.
    /// </summary>
    void OnEnable()
    {
        prevPosition = transform.position;
		anim = GetComponentInParent<Animator>();
    }

    /// <summary>
    /// Atualize esta instância.
    /// </summary>
    void FixedUpdate()
    {
        // Se o movimento é permitido
        if (move == true)
        {
            // Mover-se para o ponto de destino
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime);
        }
        // Calcular velocidade
        velocity = (Vector2)transform.position - prevPosition;
		velocity /= Time.fixedDeltaTime;
        // Se o giro for permitido
        if (turn == true)
        {
            SetSpriteDirection(destination - (Vector2)transform.position);
        }
        // Salvar última posição
        prevPosition = transform.position;
    }

    /// <summary>
    /// Define a direção do sprite no eixo x.
    /// </summary>
    /// <param name="direction">Direction.</param>
    private void SetSpriteDirection(Vector2 direction)
    {
        // Flip gameobject dependendo da direção
        if (direction.x > 0f && transform.localScale.x < 0f) // Para a direita 
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0f && transform.localScale.x > 0f) // Para a esquerda 
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        // Definir direção para animação 8d (se usado animador correspondente)
        if (anim != null)
		{
			foreach (AnimatorControllerParameter param in anim.parameters)
			{
				if (param.name == "directionY")
				{
					float directionY = direction.y / (Mathf.Abs(direction.x) + Mathf.Abs(direction.y));
					anim.SetFloat("directionY", directionY);
					break;
				}
			}
		}
    }

    /// <summary>
    /// Olha a direção.
    /// </summary>
    /// <param name="direction">Direction.</param>
    public void LookAt(Vector2 direction)
    {
        SetSpriteDirection(direction);
    }

    /// <summary>
    /// Olha o alvo.
    /// </summary>
    /// <param name="target">Target.</param>
    public void LookAt(Transform target)
    {
        SetSpriteDirection(target.position - transform.position);
    }
}
