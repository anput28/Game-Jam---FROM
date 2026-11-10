using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnObjects : MonoBehaviour
{
    [SerializeField] private GameObject ObjectPrefab;
    [SerializeField] private int ObjectCount = 10;

    private BoxCollider _spawnArea;
    private float _objectHalfHeight;

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
            Vector3 randomPosition = GetRandomPosition();
            Instantiate(ObjectPrefab, randomPosition, Quaternion.identity);
        }
        
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 min = _spawnArea.bounds.min;
        Vector3 max = _spawnArea.bounds.max;

        Vector3 randomPos = new(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );

        // Adjust y based on terrain and object's height  
        float terrainHeight = Terrain.activeTerrain.SampleHeight(randomPos);
        terrainHeight += _objectHalfHeight;
        return new Vector3(randomPos.x, terrainHeight, randomPos.z);
    }
}
