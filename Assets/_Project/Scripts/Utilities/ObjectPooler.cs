using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize;

    private List<GameObject> pool;
    private int counter = 0;

    private void Awake()
    {
        InitializePool();
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in pool)
            if (!obj.activeInHierarchy)
                return obj;

        return CreateNewObject();
    }

    public void ReturnObjectToPool(GameObject objectToReturn)
    {
        objectToReturn.transform.SetParent(transform);
        objectToReturn.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        objectToReturn.SetActive(false);
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        counter++;
        obj.name = $"{prefab.name} {counter}";
        obj.SetActive(false);
        pool.Add(obj);

        return obj;
    }

    private void InitializePool()
    {
        pool = new();

        for (int i = 0; i < poolSize; i++)
            CreateNewObject();
    }
}