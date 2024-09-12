using UnityEngine;
using System.Collections.Generic;

public class EnemyGroupLeader : MonoBehaviour
{
    public List<GameObject> groupMembers; // Liste des membres du groupe
    public float commandRate = 5f; // Délai entre chaque commande
    private float nextCommandTime;

    void Update()
    {
        if (Time.time >= nextCommandTime)
        {
            GiveCommand();
            nextCommandTime = Time.time + commandRate;
        }
    }

    void GiveCommand()
    {
        // Choisir une action de groupe : attaque synchronisée ou autre comportement
        int randomCommand = Random.Range(0, 2);
        
        if (randomCommand == 0)
        {
            // Attaque synchronisée
            foreach (GameObject member in groupMembers)
            {
                EnemyAI enemyAI = member.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.ChargeAtPlayer();
                }
            }
            Debug.Log("Group attacks together!");
        }
        else
        {
            // Formation défensive (par exemple, le soigneur se met en arrière)
            foreach (GameObject member in groupMembers)
            {
                HealerEnemy healer = member.GetComponent<HealerEnemy>();
                if (healer != null)
                {
                    healer.MoveToSafePosition();
                }
            }
            Debug.Log("Group forms a defensive formation!");
        }
    }
}
