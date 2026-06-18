using UnityEngine;
using UnityEngine.AI;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject collectiblePrefab;
    public int totalCollectibles = 7;
    public int TotalCollectibles => totalCollectibles;

    [Header("Square Bounds Settings")]
    // Instead of one radius, we define the exact width and length of your square floor
    [SerializeField] private float boxWidthX = 15f;
    [SerializeField] private float boxLengthZ = 15f;

    [Header("Wall Clearance Offset")]
    [SerializeField] private float wallCheckRadius = 1.0f;
    public LayerMask obstacleLayer;

    void Start()
    {
        SpawnCollectibles();
    }

    void SpawnCollectibles()
    {
        int spawnedCount = 0;
        int safetyNetAttempts = 0;

        while (spawnedCount < totalCollectibles && safetyNetAttempts < 100)
        {
            safetyNetAttempts++;

            // 1. Pick a random X position and a random Z position independently inside our box limits
            float randomX = Random.Range(-boxWidthX / 2f, boxWidthX / 2f);
            float randomZ = Random.Range(-boxLengthZ / 2f, boxLengthZ / 2f);

            // Combine them into a target position relative to the spawner's center position
            Vector3 randomPoint = transform.position + new Vector3(randomX, 0f, randomZ);

            // 2. Project that point onto the nearest walkable NavMesh surface
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                Vector3 validSpawnPoint = hit.position;
                validSpawnPoint.y += 0.5f;

                // 3. Offset Check: Ensure it isn't clipping into any wall layers
                if (!Physics.CheckSphere(validSpawnPoint, wallCheckRadius, obstacleLayer))
                {
                    Instantiate(collectiblePrefab, validSpawnPoint, Quaternion.identity);
                    spawnedCount++;
                }
            }
        }
    }

    // Draws a beautiful green wireframe box in the Scene Editor view to match your square level perfectly!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        // Creates a 3D box wireframe outline based on our custom Width and Length values
        Vector3 boxSize = new Vector3(boxWidthX, 1f, boxLengthZ);
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}