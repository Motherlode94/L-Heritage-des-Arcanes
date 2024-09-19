using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public int attackDamage = 20;
    public float attackRange = 2f; 
    public Transform attackPoint;  
    public LayerMask enemyLayers;  
    public float attackCooldown = 1f;  
    public Animator animator; // Utilisé pour les animations

    private PlayerControls playerControls;
    private bool canAttack = true;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Attack.performed += OnAttack;  
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
        // Jouer l'animation d'attaque
        if (animator != null)
        {
            animator.SetTrigger("Attack");  // Jouer l'animation d'attaque
        }

        // Gestion du cooldown de l'attaque
        StartCoroutine(AttackCooldown());

        // Détection des ennemis dans la portée de l'attaque
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(attackDamage);  // Appliquer les dégâts
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);  // Visualiser la portée de l'attaque
    }
}
