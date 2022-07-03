using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Câmera em movimento e dimensionamento automático.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
	/// <summary>
	/// Tipo de controle para dimensionamento automático da câmera.
	/// </summary>
	public enum ControlType
    {
        ConstantWidth,      // A câmera manterá a largura constante
		ConstantHeight,     // A câmera manterá a altura constante
		OriginCameraSize    // Não dimensionar a câmera
	}

	// Tipo de controle da câmera
	public ControlType controlType;
	// A câmera será dimensionada automaticamente para caber neste objeto
	public SpriteRenderer focusObjectRenderer;
	// Deslocamento horizontal das bordas do objeto de foco
	public float offsetX = 0f;
	// Deslocamento vertical das bordas do objeto de foco
	public float offsetY = 0f;
	// Velocidade da câmera ao se mover (arrastando-se)
	public float dragSpeed = 2f;

	// Pontos restritivos para movimentação da câmera
	private float maxX, minX, maxY, minY;
	// Câmera a arrastar o now vector
	private float moveX, moveY;
	// Componente da câmera deste gameobject
	private Camera cam;
	// Proporção da câmera de origem
	private float originAspect;

	/// <summary>
	/// Inicia esta instância
	/// </summary>
	void Start()
	{
		cam = GetComponent<Camera>();
		Debug.Assert(focusObjectRenderer && cam, "Wrong initial settings");
		originAspect = cam.aspect;
		// Obtém pontos restritivos dos cantos do objeto de foco
		maxX = focusObjectRenderer.bounds.max.x;
		minX = focusObjectRenderer.bounds.min.x;
		maxY = focusObjectRenderer.bounds.max.y;
		minY = focusObjectRenderer.bounds.min.y;
		UpdateCameraSize();
	}

	/// <summary>
	/// Atualiza esta instância
	/// </summary>
    void LateUpdate()
    {
		// A proporção da câmera foi alterada
		if (originAspect != cam.aspect)
		{
			UpdateCameraSize();
			originAspect = cam.aspect;
		}
		// Se recisar de mover a câmera horizontalmente
		if (moveX != 0f)
        {
			bool permit = false;
			// Mover para a direita
			if (moveX > 0f)
			{
				// Se o ponto restritivo não for atingido
				if (cam.transform.position.x + (cam.orthographicSize * cam.aspect) < maxX - offsetX)
				{
					permit = true;
				}
			}
			// Mover para a esquerda
			else
			{
				// Se o ponto restritivo não for atingido
				if (cam.transform.position.x - (cam.orthographicSize * cam.aspect) > minX + offsetX)
				{
					permit = true;
				}
			}
			if (permit == true)
			{
				// Mover câmera
				transform.Translate(Vector3.right * moveX * dragSpeed, Space.World);
			}
            moveX = 0f;
        }
		// Se precisar de mover a câmera verticalmente
		if (moveY != 0f)
        {
			bool permit = false;
			// Subir
			if (moveY > 0f)
			{
				// Se o ponto restritivo não for atingido
				if (cam.transform.position.y + cam.orthographicSize < maxY - offsetY)
				{
					permit = true;
				}
			}
			// Mover para baixo
			else
			{
				// Se o ponto restritivo não for atingido
				if (cam.transform.position.y - cam.orthographicSize > minY + offsetY)
				{
					permit = true;
				}
			}
			if (permit == true)
			{
				// Mover câmera
				transform.Translate (Vector3.up * moveY * dragSpeed, Space.World);
			}
            moveY = 0f;
        }
    }

	/// <summary>
	/// Precisa de mover a câmera horizontalmente.
	/// </summary>
	/// <param name="distance">Distância</param>
	public void MoveX(float distance)
    {
        moveX = distance;
    }

	/// <summary>
	/// Precisa mover a câmera verticalmente.
	/// </summary>
	/// <param name="distance">Distância</param>
	public void MoveY(float distance)
    {
        moveY = distance;
    }

	/// <summary>
	/// Atualiza o tamanho da câmera para ajustar o objeto de foco.
	/// </summary>
	private void UpdateCameraSize()
	{
		switch (controlType)
		{
		case ControlType.ConstantWidth:
			cam.orthographicSize = (maxX - minX - 2 * offsetX) / (2f * cam.aspect);
			break;
		case ControlType.ConstantHeight:
			cam.orthographicSize = (maxY - minY - 2 * offsetY) / 2f;
			break;
		}
	}
}
