using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Caminho para o inimigo andar 
/// </summary>
[ExecuteInEditMode]
public class Pathway : MonoBehaviour
{
    /// <summary>
    /// Tira o ponto de encontro mais perto para uma posição especifíca 
    /// </summary>
    /// <returns>O ponto mais perto</returns>
    /// <param name="position">Position.</param>
    public Waypoint GetNearestWaypoint(Vector3 position)
    {
        float minDistance = float.MaxValue;
        Waypoint nearestWaypoint = null;
        foreach (Waypoint waypoint in GetComponentsInChildren<Waypoint>())
        {
            // Calcula a distância para o ponto de encontro
            Vector3 vect = position - waypoint.transform.position;
            float distance = vect.magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestWaypoint = waypoint;
            }
        }
		for (;;)
		{
			float waypointPathDistance = GetPathDistance(nearestWaypoint);
			Waypoint nextWaypoint = GetNextWaypoint(nearestWaypoint, false);
			if (nextWaypoint != null)
			{
				float myPathDistance = GetPathDistance(nextWaypoint) + (nextWaypoint.transform.position - position).magnitude;
				if (waypointPathDistance <= myPathDistance)
				{
					break;
				}
				else
				{
					nearestWaypoint = nextWaypoint;
				}
			}
			else
			{
				break;
			}
		}
        return nearestWaypoint;
    }

    /// <summary>
    /// Obtém o ponto de encontro mais perto do caminho 
    /// </summary>
    /// <returns>O próximo ponto de encontro</returns>
    /// <param name="currentWaypoint">Current waypoint.</param>
    /// <param name="loop">If set to <c>true</c> loop.</param>
    public Waypoint GetNextWaypoint(Waypoint currentWaypoint, bool loop)
    {
        Waypoint res = null;
        int idx = currentWaypoint.transform.GetSiblingIndex();
        if (idx < (transform.childCount - 1))
        {
            idx += 1;
        }
        else
        {
            idx = 0;
        }
        if (!(loop == false && idx == 0))
        {
            res = transform.GetChild(idx).GetComponent<Waypoint>(); 
        }
        return res;
    }

	/// <summary>
	/// Obtém o ponto de encontro anterior do caminho 
	/// </summary>
	/// <returns>O ponto de encontro anterior</returns>
	/// <param name="currentWaypoint">Current waypoint.</param>
	/// <param name="loop">If set to <c>true</c> loop.</param>
	public Waypoint GetPreviousWaypoint(Waypoint currentWaypoint, bool loop)
	{
		Waypoint res = null;
		int idx = currentWaypoint.transform.GetSiblingIndex();
		if (idx > 0)
		{
			idx -= 1;
		}
		else
		{
			idx = transform.childCount - 1;
		}
		if (!(loop == false && idx == transform.childCount - 1))
		{
			res = transform.GetChild(idx).GetComponent<Waypoint>(); 
		}
		return res;
	}

	/// <summary>
	/// Obtém a restante distância do caminho para o ponto de encontro especifíco
	/// </summary>
	/// <returns>The path distance.</returns>
	/// <param name="fromWaypoint">From waypoint.</param>
    public float GetPathDistance(Waypoint fromWaypoint)
    {
        Waypoint[] waypoints = GetComponentsInChildren<Waypoint>();
        bool hitted = false;
        float pathDistance = 0f;
        int idx;
		// Calcula o caminho restante 
        for (idx = 0; idx < waypoints.Length; ++idx)
        {
            if (hitted == true)
            {
				// Acrescenta distância entre o ponto de encontro e o resultado 
                Vector2 distance = waypoints[idx].transform.position - waypoints[idx - 1].transform.position;
                pathDistance += distance.magnitude;
            }
            if (waypoints[idx] == fromWaypoint)
            {
                hitted = true;
            }
        }
        return pathDistance;
    }

	/// <summary>
	/// Obtém o deslocamento da posição do caminho especifíco 
	/// </summary>
	/// <returns>A posição do deslocamento</returns>
	/// <param name="nextWaypoint">Next waypoint. Will be updated after offset</param>
	/// <param name="currentPosition">Current position.</param>
	/// <param name="offsetDistance">Offset distance.</param>
	public Vector2 GetOffsetPosition(ref Waypoint nextWaypoint, Vector2 currentPosition, float offsetDistance)
	{
		Vector2 res = currentPosition;
		if (offsetDistance >= 0f) // Deslocamento em frente 
		{
			float remainingDistance = offsetDistance;
			Vector2 lastPosition = currentPosition;
			Waypoint waypoint = nextWaypoint;
			Vector2 deltaVector = Vector2.zero;
			// Calcula o ponto de encontro mais perto da posição do deslocamento 
			for (;;)
			{
				deltaVector = (Vector2)waypoint.transform.position - lastPosition;
				float deltaDistance = deltaVector.magnitude;
				if (remainingDistance > deltaDistance)
				{
					remainingDistance -= deltaDistance;
					lastPosition = waypoint.transform.position;
					waypoint = GetNextWaypoint(waypoint, false);
					if (waypoint == null)
					{
						remainingDistance = 0f;
						break;
					}
					else
					{
						nextWaypoint = waypoint;
					}
				}
				else
				{
					break;
				}
			}
			// Calcula o resultado da posição
			res = lastPosition + deltaVector.normalized * remainingDistance;
		}
		else // Deslocamento de trás 
		{
			float remainingDistance = -offsetDistance;
			Vector2 lastPosition = currentPosition;
			Waypoint waypoint = GetPreviousWaypoint(nextWaypoint, false);
			if (waypoint == null)
			{
				return res;
			}
			Vector2 deltaVector = Vector2.zero;
			// Calcula o ponto especifíco mais perto da posição do deslocamento 
			for (;;)
			{
				deltaVector = (Vector2)waypoint.transform.position - lastPosition;
				float deltaDistance = deltaVector.magnitude;
				if (remainingDistance > deltaDistance)
				{
					nextWaypoint = waypoint;
					remainingDistance -= deltaDistance;
					lastPosition = waypoint.transform.position;
					waypoint = GetPreviousWaypoint(waypoint, false);
					if (waypoint == null)
					{
						remainingDistance = 0f;
						break;
					}
				}
				else
				{
					break;
				}
			}
			// Calcula o resultado da posição 
			res = lastPosition + deltaVector.normalized * remainingDistance;
		}
		return res;
	}
}
