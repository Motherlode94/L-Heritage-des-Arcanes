using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float speed = 5f;
    public int health = 100;
    public int attackPower = 10;

    // Points de compétence du joueur
    public int skillPoints = 0;

    void Update()
    {
        // Exemple d'amélioration déclenchée via une touche (ici 'U' pour Upgrade)
        if (Input.GetKeyDown(KeyCode.U) && skillPoints > 0)
        {
            UpgradeSpeed(1f); // Augmente la vitesse
            skillPoints--; // Dépense des points de compétence
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
