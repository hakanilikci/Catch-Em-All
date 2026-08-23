using UnityEngine;
using UnityEngine.AI;
public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;
    public float speed = 2.0f; 
    private const string SnorlaxSpeedKey = "SnorlaxSpeed";

    void Start()
    {
        // Get the NavMeshAgent component attached to this object
        navMeshAgent = GetComponent<NavMeshAgent>();
        UpdateSpeed(); 
    }

    public void UpdateSpeed()
    {
        // Retrieve the speed setting from PlayerPrefs, default to 2.0 if not found
        speed = PlayerPrefs.GetFloat(SnorlaxSpeedKey, 2.0f);
        if (navMeshAgent != null)
        {
            // Apply the speed to the NavMeshAgent
            navMeshAgent.speed = speed;
        }
    }
    
    void Update()
    {
        if (player != null)
        {
            // Set the enemy's destination to follow the player
            navMeshAgent.SetDestination(player.position);
            
            // Calculate distance to the player
            float distance = Vector3.Distance(transform.position, player.position);

            // If close enough, trigger a hit on the player
            if (distance < 1.2f)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.EnemyHit();
                }
            }
        }
    }
}
