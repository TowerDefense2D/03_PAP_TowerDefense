using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ordem de operação do sprite depende da posição y.
/// </summary>
public class SpriteSorting : MonoBehaviour
{
    // Static não mudará a ordem na atualização, apenas no início
    public bool isStatic;
    // Multiplicador para aumento de precisão
    public float rangeFactor = 100f;

    // Lista de sprites para este objeto em clildren
    private Dictionary<SpriteRenderer, int> sprites = new Dictionary<SpriteRenderer, int>();

    /// <summary>
    /// Desperta esta instância.
    /// </summary>
    void Awake()
    {
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            sprites.Add(sprite, sprite.sortingOrder);
        }
    }

    /// <summary>
    /// Começa esta instância.
    /// </summary>
    void Start()
    {
        UpdateSortingOrder();
    }

    /// <summary>
    /// Atualiza esta instância 
    /// </summary>
    void Update()
    {
        if (isStatic == false)
        {
            UpdateSortingOrder();
        }
    }

    /// <summary>
    /// Atualiza a ordem de classificação dos sprites.
    /// </summary>
    private void UpdateSortingOrder()
    {
        foreach (KeyValuePair<SpriteRenderer, int> sprite in sprites)
        {
            sprite.Key.sortingOrder = sprite.Value - (int)(transform.position.y * rangeFactor);
        }
    }
}
