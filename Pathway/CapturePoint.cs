using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Se o inimigo fica a frente deste ponto - o jogador será derrotado 
/// </summary>
public class CapturePoint : MonoBehaviour
{
    /// <summary>
    /// Ativa o trigger event enter2d
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
		Destroy(other.gameObject);
		EventManager.TriggerEvent("Captured", other.gameObject, null);
    }
}
