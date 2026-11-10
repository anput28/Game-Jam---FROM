using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class SpawnObjects : MonoBehaviour
{
    [Header("Object Info")]
    [SerializeField] private GameObject ObjectPrefab;
    [SerializeField] private int ObjectCount = 10;

    [Header("Position Valiation")]
    [SerializeField] private float CheckRadius = 100.0f;
    [SerializeField] private LayerMask ObstacleMask;
    [SerializeField] [Range(-1, 0)] private float OffsetFromGround = -0.5f;

    private BoxCollider _spawnArea;
    private float _objectHalfHeight;
    private readonly float _maxRetries = 5;
    private readonly List<Vector3> _spawnedPositions = new();

    private void Awake()
    {
        _spawnArea = GetComponent<BoxCollider>();

        if (ObjectPrefab != null)
        {
            _objectHalfHeight = ObjectPrefab.transform.localScale.y / 2f;
        }
    }

    private void Start()
    {
        for (int i = 0; i < ObjectCount; i++)
        {
            if (!TryGetValidPosition(out Vector3 randomPosition))
            {
                Debug.LogWarning($"Spawn {i} failed after {_maxRetries} attempts.");
                continue;
            }

            Instantiate(ObjectPrefab, randomPosition, Quaternion.identity);
            // Ensure physics updates after instantiation, without this, the next position check may not account for the newly spawned object.
            Physics.SyncTransforms();

            _spawnedPositions.Add(randomPosition);
        }
    }

    private bool TryGetValidPosition(out Vector3 position)
    {
        for (int i = 0; i < _maxRetries; i++)
        {
            position = GetRandomPosition();
            if (IsPositionValid(position))
            {
                return true;
            }
        }

        position = default;
        return false;
    }

    private bool IsPositionValid(Vector3 position)
    {
        return !Physics.CheckSphere(position, CheckRadius, ObstacleMask, QueryTriggerInteraction.Ignore);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 min = _spawnArea.bounds.min;
        Vector3 max = _spawnArea.bounds.max;

        Vector3 randomPos = new(
            Random.Range(min.x, max.x),
            0,
            Random.Range(min.z, max.z)
        );

        // Adjust y based on terrain and object's height  
        float terrainHeight = Terrain.activeTerrain.SampleHeight(randomPos);
        terrainHeight += _objectHalfHeight + OffsetFromGround;

        return new Vector3(randomPos.x, terrainHeight, randomPos.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (_spawnedPositions != null)
        {
            foreach (var pos in _spawnedPositions)
            {
                Gizmos.DrawWireSphere(pos, CheckRadius);
            }
        }
    }
}
