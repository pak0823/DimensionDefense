using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private T prefab;
    private Queue<T> pool = new Queue<T>();
    private Transform parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        for (int i = 0; i < initialSize; i++)
            pool.Enqueue(CreateNew());
    }

    private T CreateNew()
    {
        var go = Object.Instantiate(prefab, parent);
        go.gameObject.SetActive(false);
        return go;
    }

    public T Get(Vector3 position)
    {
        T instance = pool.Count > 0 ? pool.Dequeue() : CreateNew();
        instance.transform.position = position;
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void Release(T instance)
    {
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
}
