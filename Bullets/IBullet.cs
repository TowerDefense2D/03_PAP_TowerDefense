using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface para todas as balas.
/// </summary>
public interface IBullet
{
    void SetDamage(int damage);
	int GetDamage();
    void Fire(Transform target);
}
