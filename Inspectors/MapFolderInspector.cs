using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// Inspetor de pastas dos mapas.
/// </summary>
public class MapFolderInspector : MonoBehaviour
{
	// Imagem do mapa
	public SpriteRenderer map;
	// Pasta para cada imagem dos icons do spawn
	public Transform spawnIconFolder;
	// Pasta para imagem de ícons de captura
	public Transform captureIconFolder;

	/// <summary>
	/// Ativa o enable event
	/// </summary>
	void OnEnable()
	{
		Debug.Assert(map && spawnIconFolder && captureIconFolder, "Wrong stuff settings");
	}

	/// <summary>
	/// Altera o sprite do mapa.
	/// </summary>
	/// <returns>O sprite do mapa</returns>
	/// <param name="mapPrefab">Map prefab.</param>
	public void ChangeMapSprite(Sprite sprite)
	{
		if (map != null && sprite != null)
		{
			map.sprite = sprite;
		}
	}

	public void LoadMap(GameObject mapPrefab)
	{
		if (mapPrefab != null)
		{
			if (map != null)
			{
				DestroyImmediate(map.gameObject);
			}
			GameObject newMap = Instantiate(mapPrefab, transform);
			newMap.name = mapPrefab.name;
			map = newMap.GetComponent<SpriteRenderer>();
			Debug.Assert(map, "Wrong stuff settings");
		}
	}

	/// <summary>
	/// Adiciona o ícon de spawn.
	/// </summary>
	/// <returns>O ícone de spawn.</returns>
	/// <param name="spawnIconPrefab">Prefab do icon de spawn</param>
	public GameObject AddSpawnIcon(GameObject spawnIconPrefab)
	{
		GameObject res = Instantiate(spawnIconPrefab, spawnIconFolder);
		res.name = spawnIconPrefab.name;
		return res;
	}

	/// <summary>
	/// Adiciona o ícon de captura.
	/// </summary>
	/// <returns>O ícone de captura.</returns>
	/// <param name="captureIconPrefab">Capture icon prefab.</param>
	public GameObject AddCaptureIcon(GameObject captureIconPrefab)
	{
		GameObject res = Instantiate(captureIconPrefab, captureIconFolder);
		res.name = captureIconPrefab.name;
		return res;
	}
}
#endif
