using UnityEngine;

public class FireWall : MonoBehaviour
{
    public int damage = 20;
    public float duration = 10f;

    void Start()
    {
        // Le mur de feu disparaît après un certain temps
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Infliger des dégâts au joueur
            Debug.Log("Player hit by fire wall!");
        }
    }
}
