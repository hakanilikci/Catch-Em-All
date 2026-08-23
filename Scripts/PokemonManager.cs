using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PokemonManager : MonoBehaviour
{
    [Header("Pokemon Settings")]
    public List<GameObject> pokemonPrefabs; 
    public int spawnCount = 25;
    public float movementInterval = 5f;
    public float baseSpawnRadius = 20f; 
    
    [Header("Spawn Adjustments")]
    public bool useWholeMap = true; 
    public float scaleMultiplier = 1.0f; 
    public Vector3 rotationOffset = new Vector3(0, 0, 0); 
    public float heightOffset = 0.5f; 

    private List<GameObject> spawnedPokemons = new List<GameObject>();
    private NavMeshTriangulation navMeshData;

    void Start()
    {
        // Calculate NavMesh triangulation for random point generation
        if (useWholeMap)
        {
            navMeshData = NavMesh.CalculateTriangulation();
        }
        SpawnPokemons();
        // Start the routine to move Pokemons periodically
        StartCoroutine(MovePokemonsRoutine());
    }

    void SpawnPokemons()
    {
        if (pokemonPrefabs == null || pokemonPrefabs.Count == 0) return;

        for (int i = 0; i < spawnCount; i++)
        {
            // Picks a random Pokemon prefab and a random location
            GameObject prefab = pokemonPrefabs[Random.Range(0, pokemonPrefabs.Count)];
            Vector3 spawnPosition = GetRandomNavMeshPosition();

            if (spawnPosition != Vector3.zero)
            {
                Quaternion spawnRotation = Quaternion.Euler(rotationOffset);
                GameObject newPokemon = Instantiate(prefab, spawnPosition, spawnRotation);
                
                // Force tag and trigger for gameplay logic
                newPokemon.tag = "Pokemon"; 
                
                // Ensure it has a collider that is a trigger (for pickup)
                Collider col = newPokemon.GetComponent<Collider>();
                if (col == null) col = newPokemon.AddComponent<BoxCollider>();
                col.isTrigger = true;

                newPokemon.transform.localScale = newPokemon.transform.localScale * scaleMultiplier;

                // If it has a NavMeshAgent, warp it to the valid position
                NavMeshAgent agent = newPokemon.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.Warp(spawnPosition);
                    agent.baseOffset = heightOffset; 
                }
                else
                {
                    newPokemon.transform.position = spawnPosition + Vector3.up * heightOffset;
                }

                spawnedPokemons.Add(newPokemon);
            }
        }
    }

    IEnumerator MovePokemonsRoutine()
    {
        // Infinite loop to relocate Pokemons every 'movementInterval' seconds
        while (true)
        {
            yield return new WaitForSeconds(movementInterval);
            RelocatePokemons();
        }
    }

    void RelocatePokemons()
    {
        // Clean up list
        spawnedPokemons.RemoveAll(p => p == null || !p.activeInHierarchy);
        if (spawnedPokemons.Count == 0) return;

        foreach (GameObject pokemon in spawnedPokemons)
        {
            if (pokemon != null && pokemon.activeInHierarchy)
            {
                // Find a new random position for each Pokemon
                Vector3 newPosition = GetRandomNavMeshPosition();
                if (newPosition != Vector3.zero)
                {
                    NavMeshAgent agent = pokemon.GetComponent<NavMeshAgent>();
                    if (agent != null)
                    {
                        agent.Warp(newPosition); 
                        agent.baseOffset = heightOffset; 
                    }
                    else
                    {
                        pokemon.transform.position = newPosition + Vector3.up * heightOffset;
                    }
                }
            }
        }
    }

    Vector3 GetRandomNavMeshPosition()
    {
        if (useWholeMap)
        {
            int t = Random.Range(0, navMeshData.indices.Length - 3);
            
            Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
            Vector3 randomPoint = Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

            return randomPoint;
        }
        else
        {
            Vector3 randomDirection = Random.insideUnitSphere * baseSpawnRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, baseSpawnRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return Vector3.zero;
        }
    }
}
