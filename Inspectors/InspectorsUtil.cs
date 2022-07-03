using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Parte comum para inspetores.
/// </summary>
public static class InspectorsUtil<T> where T : Component
{
	/// <summary>
	/// Obtém o próximo objeto.
	/// </summary>
	/// <returns>O próximo</returns>
	/// <param name="folder">Pasta</param>
	/// <param name="currentlySelected">Atualmente selecionado</param>
	public static GameObject GetNext(Transform folder, GameObject currentlySelected)
	{
		GameObject res = currentlySelected;
		T[] array = folder.GetComponentsInChildren<T>();
		if (array.Length > 0)
		{
			bool hitted = false;
			for (int i = 0; i < array.Length; ++i)
			{
				if (array[i].gameObject == currentlySelected)
				{
					if (array.Length > i + 1)
					{
						res = array[i + 1].gameObject;
					}
					else
					{
						res = array[i].gameObject;
					}
					hitted = true;
					break;
				}
			}
			if (hitted == false)
			{
				res = array[array.Length - 1].gameObject;
			}
		}
		return res;
	}

	/// <summary>
	/// Obtém o objeto anterior.
	/// </summary>
	/// <returns>Anterior</returns>
	/// <param name="folder">Pasta</param>
	/// <param name="currentlySelected">Atual selecionado</param>
	public static GameObject GetPrevious(Transform folder, GameObject currentlySelected)
	{
		GameObject res = currentlySelected;
		T[] array = folder.GetComponentsInChildren<T>();
		if (array.Length > 0)
		{
			bool hitted = false;
			for (int i = array.Length - 1; i >= 0; --i)
			{
				if (array[i].gameObject == currentlySelected)
				{
					if (i > 0)
					{
						res = array[i - 1].gameObject;
					}
					else
					{
						res = array[0].gameObject;
					}
					hitted = true;
					break;
				}
			}
			if (hitted == false)
			{
				res = array[0].gameObject;
			}
		}
		return res;
	}
}
#endif
