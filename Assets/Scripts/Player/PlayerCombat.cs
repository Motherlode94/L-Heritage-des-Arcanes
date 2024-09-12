using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public int attackDamage = 20;
    public float attackRange = 2f; // Range of attack
    public Transform attackPoint;  // Where the attack is initiated
    public LayerMask enemyLayers;  // Layer for detecting enemies

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls(); // Initialize input controls
    }

    private void OnEnable()
    {
        playerControls.Player.Attack.performed += OnAttack;  // Register attack action
        playerControls.Player.Attack.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Attack.Disable();  // Disable attack action
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack();
        }
    }

    void Attack()
    {
        // Detect enemies within range of the attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Apply damage to enemies
        foreach (Collider enemy in hitEnemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);  // Visualize attack range
    }
}
