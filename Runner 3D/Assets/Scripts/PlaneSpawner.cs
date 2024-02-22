using UnityEngine;

public class PlaneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _planePrefab;
    private GameObject _plane;

    private float _spawnRate;
    [SerializeField] private float _minSpawnRate;
    [SerializeField] private float _maxSpawnRate;

    private float _dTime = 0;

    void Start()
    {
        _plane = Instantiate(_planePrefab, new Vector3(0, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation);
        _plane.SetActive(false);
        _spawnRate = Random.Range(_minSpawnRate, _maxSpawnRate);
    }


    void Update()
    {
        _dTime += Time.deltaTime;
        if (_dTime > _spawnRate) SpawnPlane();
    }

    void SpawnPlane()
    {
        _plane.transform.position = new Vector3(0, gameObject.transform.position.y, gameObject.transform.position.z);
        _plane.SetActive(true);
        _dTime = 0;
        _spawnRate = Random.Range(_minSpawnRate, _maxSpawnRate);
        // if plane out of camera view, Recycler is going to deactivate it
    }
}
