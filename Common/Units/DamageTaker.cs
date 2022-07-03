using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este alvo pode receber dano.
/// </summary>
public class DamageTaker : MonoBehaviour
{
	// Pontos de vida iniciais
	public int hitpoints = 1;
	// Pontos de vida restantes
	[HideInInspector]
    public int currentHitpoints;
	// Duração do efeito visual de dano
	public float damageDisplayTime = 0.2f;
	// Objeto de barra de vida
	public Transform healthBar;
	// SendMessage será acionado no dano recebido
	public bool isTrigger;
	// Efeito sonoro de morrer
	public AudioClip dieSfx;

	// Imagem deste objeto
	private SpriteRenderer sprite;
	// A visualização de hit ou cura está em andamento
	private bool coroutineInProgress;
	// Largura original da barra de vida (hp total)
	private float originHealthBarWidth;

	/// <summary>
	/// Desperta esta instância.
	/// </summary>
	void Awake()
    {
        currentHitpoints = hitpoints;
        sprite = GetComponentInChildren<SpriteRenderer>();
        Debug.Assert(sprite && healthBar, "Wrong initial parameters");
    }

	/// <summary>
	/// Começa esta instância 
	/// </summary>
    void Start()
    {
        originHealthBarWidth = healthBar.localScale.x;
    }

	/// <summary>
	/// Leva dano.
	/// </summary>
	/// <param name="damage">Dano</param>
	public void TakeDamage(int damage)
    {
		if (damage > 0)
		{
			if (this.enabled == true)
			{
				if (currentHitpoints > damage)
				{
					// Continua vivo
					currentHitpoints -= damage;
					UpdateHealthBar();
					// Se nenhuma corrotina agora
					if (coroutineInProgress == false)
					{
						// Visualização do dano
						StartCoroutine(DisplayDamage());
					}
					if (isTrigger == true)
					{
						// Notifica outros componentes deste objeto do jogo
						SendMessage("OnDamage");
					}
				}
				else
				{
					// Morre
					currentHitpoints = 0;
					UpdateHealthBar();
					Die();
				}
			}
		}
		else // Dano < 0
		{
			// Cura
			currentHitpoints = Mathf.Min(currentHitpoints - damage, hitpoints);
			UpdateHealthBar();
		}
    }

	/// <summary>
	/// Atualiza a largura da barra de vida.
	/// </summary>
	private void UpdateHealthBar()
    {
        float healthBarWidth = originHealthBarWidth * currentHitpoints / hitpoints;
        healthBar.localScale = new Vector2(healthBarWidth, healthBar.localScale.y);
    }

	/// <summary>
	/// esta instância morre.
	/// </summary>
	public void Die()
    {
		EventManager.TriggerEvent("UnitKilled", gameObject, null);
		StartCoroutine(DieCoroutine());
    }

	private IEnumerator DieCoroutine()
	{
		if (dieSfx != null && AudioManager.instance != null)
		{
			AudioManager.instance.PlayDie(dieSfx);
		}
		foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
		{
			col.enabled = false;
		}
		GetComponent<AiBehavior>().enabled = false;
		GetComponent<NavAgent>().enabled = false;
		GetComponent<EffectControl>().enabled = false;
		Animator anim = GetComponent<Animator>();
		// Se a unidade conter uma animação 
		if (anim != null && anim.runtimeAnimatorController != null)
		{
			// Pesquisa um clip
			foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
			{
				if (clip.name == "Die")
				{
					// Reproduz a animação
					anim.SetTrigger("die");
					yield return new WaitForSeconds(clip.length);
					break;
				}
			}
		}
		Destroy(gameObject);
	}

	/// <summary>
	/// Visualização do dano.
	/// </summary>
	/// <returns>O dano.</returns>
	IEnumerator DisplayDamage()
    {
        coroutineInProgress = true;
        Color originColor = sprite.color;
        float counter;
		// Define a cor para preto e retorna à cor de origem ao longo do tempo
		for (counter = 0f; counter < damageDisplayTime; counter += Time.fixedDeltaTime)
        {
            sprite.color = Color.Lerp(originColor, Color.black, Mathf.PingPong(counter, damageDisplayTime / 2f));
			yield return new WaitForFixedUpdate();
        }
        sprite.color = originColor;
        coroutineInProgress = false;
    }

	/// <summary>
	/// Ativa o evento destroy.
	/// </summary>
	void OnDestroy()
	{
		EventManager.TriggerEvent("UnitDie", gameObject, null);
		StopAllCoroutines();
	}
}
