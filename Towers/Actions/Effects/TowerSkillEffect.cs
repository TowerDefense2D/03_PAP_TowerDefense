using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe básica para efeito de habilidade da torre.
/// </summary>
public class TowerSkillEffect : MonoBehaviour
{
	[HideInInspector]
	// Raio de ataque da torre
	public CircleCollider2D radiusCollider;
	[HideInInspector]
	// Torre
	public Tower tower;
}
