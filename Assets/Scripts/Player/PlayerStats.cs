using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public float speed = 5f;
    public int health = 100;
    public int attackPower = 10;

    // Points de compétence du joueur
    public int skillPoints = 0;

    private PlayerControls playerControls; // Classe générée pour les actions d'input
    private InputAction upgradeAction; // Action "Upgrade"

    void Awake()
    {
        // Initialiser le système d'Input via PlayerControls
        playerControls = new PlayerControls();

        // Utiliser FindAction pour trouver l'action par son nom dans l'Input Map "Player"
        upgradeAction = playerControls.FindAction("Upgrade");

        // Vérifier si l'action est trouvée et s'abonner à l'événement
        if (upgradeAction != null)
        {
            upgradeAction.performed += ctx => Upgrade();
        }
        else
        {
            Debug.LogError("L'action 'Upgrade' n'a pas été trouvée dans PlayerControls.");
        }
    }

    void OnEnable()
    {
        playerControls.Enable(); // Activer les inputs
    }

    void OnDisable()
    {
        playerControls.Disable(); // Désactiver les inputs quand l'objet est désactivé
    }

    void Upgrade()
    {
        // Vérifier si des points de compétence sont disponibles
        if (skillPoints > 0)
        {
            UpgradeSpeed(1f); // Augmente la vitesse
            skillPoints--; // Dépense des points de compétence
            Debug.Log("Speed upgraded. Remaining skill points: " + skillPoints);
        }
        else
        {
            Debug.Log("Not enough skill points!");
        }
    }

    // Méthodes pour améliorer les statistiques du joueur
    public void UpgradeSpeed(float amount)
    {
        speed += amount;
    }

    public void UpgradeHealth(int amount)
    {
        health += amount;
    }

    public void UpgradeAttack(int amount)
    {
        attackPower += amount;
    }

    // Méthode pour gagner des points de compétence
    public void EarnSkillPoints(int points)
    {
        skillPoints += points;
    }
}
