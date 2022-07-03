using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mostra informações da unidade na folha especial.
/// </summary>
public class ShowInfo : MonoBehaviour
{
	// Nome da unidade
	public Text unitName;
	// Ícon principal para exibição
	public Image primaryIcon;
	// Texto principal para exibição
	public Text primaryText;
	// Ícon secundário para exibição
	public Image secondaryIcon;
	// Texto secundário para exibição
	public Text secondaryText;
	// Ícon de pontos de vida para exibição
	public Sprite hitpointsIcon;
	// Ícon de ataque corpo a corpo para exibição
	public Sprite meleeAttackIcon;
	// Ícon de ataque à distância para exibição
	public Sprite rangedAttackIcon;
	// Ícon de número de defensores para exibição
	public Sprite defendersNumberIcon;
	// Ícon de cooldown para exibição
	public Sprite cooldownIcon;

    /// <summary>
    /// Ativa o destroy event.
    /// </summary>
    void OnDestroy()
    {
		EventManager.StopListening("UserClick", UserClick);
    }

	/// <summary>
	/// Desperta esta instância
	/// </summary>
    void Awake()
    {
        Debug.Assert(unitName && primaryIcon && primaryText && secondaryIcon && secondaryText, "Wrong intial settings");
    }

	/// <summary>
	/// Começa esta instância
	/// </summary>
    void Start()
    {
		EventManager.StartListening("UserClick", UserClick);
        HideUnitInfo();
    }

	/// <summary>
	/// Mostra as informações da unidade.
	/// </summary>
	/// <param name="info">Info.</param>
	public void ShowUnitInfo(UnitInfo info, GameObject obj)
    {
		if (info.unitName != "")
		{
			unitName.text = info.unitName;
		}
		else
		{
			unitName.text = obj.name;
		}

		if (info.primaryIcon != null || info.secondaryIcon != null || info.primaryText != "" || info.secondaryText != "")
		{
			primaryText.text = info.primaryText;
			secondaryText.text = info.secondaryText;

			if (info.primaryIcon != null)
			{
				primaryIcon.sprite = info.primaryIcon;
				primaryIcon.gameObject.SetActive(true);
			}

			if (info.secondaryIcon != null)
			{
				secondaryIcon.sprite = info.secondaryIcon;
				secondaryIcon.gameObject.SetActive(true);
			}
		}
		else
		{
			DamageTaker damageTaker = obj.GetComponentInChildren<DamageTaker>();
			Attack attack = obj.GetComponentInChildren<Attack>();
			DefendersSpawner spawner = obj.GetComponentInChildren<DefendersSpawner>();

			// Define automaticamente o ícone e o texto principais
			if (damageTaker != null)
			{
				primaryText.text = damageTaker.hitpoints.ToString();
				primaryIcon.sprite = hitpointsIcon;
				primaryIcon.gameObject.SetActive(true);
			}
			else
			{
				if (attack != null)
				{
					if (attack != null)
					{
						primaryText.text = attack.cooldown.ToString();
						primaryIcon.sprite = cooldownIcon;
						primaryIcon.gameObject.SetActive(true);
					}
				}
				else if (spawner != null)
				{
					primaryText.text = spawner.cooldown.ToString();
					primaryIcon.sprite = cooldownIcon;
					primaryIcon.gameObject.SetActive(true);
				}
			}

			if (attack != null)
			{
				secondaryText.text = attack.damage.ToString();
				if (attack is AttackMelee)
				{
					secondaryIcon.sprite = meleeAttackIcon;
				}
				else if (attack is AttackRanged)
				{
					secondaryIcon.sprite = rangedAttackIcon;
				}
				secondaryIcon.gameObject.SetActive(true);
			}
			else
			{
				if (spawner != null)
				{
					secondaryText.text = spawner.maxNum.ToString();
					secondaryIcon.sprite = defendersNumberIcon;
					secondaryIcon.gameObject.SetActive(true);
				}
			}
		}
		gameObject.SetActive(true);
    }

	/// <summary>
	/// Esconde as informações da unidade.
	/// </summary>
	public void HideUnitInfo()
    {
        unitName.text = primaryText.text = secondaryText.text = "";
        primaryIcon.gameObject.SetActive(false);
        secondaryIcon.gameObject.SetActive(false);
		gameObject.SetActive(false);
    }

	/// <summary>
	/// Manipulador de cliques do utilizador.
	/// </summary>
	/// <param name="obj">Objeto</param>
	/// <param name="param">Perimetro</param>
	private void UserClick(GameObject obj, string param)
    {
        HideUnitInfo();
        if (obj != null)
        {
			// O objeto clicado tem informações para exibição
			UnitInfo unitInfo = obj.GetComponentInChildren<UnitInfo>();
            if (unitInfo != null)
            {
				ShowUnitInfo(unitInfo, obj);
            }
        }
    }
}
