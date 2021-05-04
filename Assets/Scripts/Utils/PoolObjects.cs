using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObjects
{
    private Queue<GameObject> queue = new Queue<GameObject>();

    public PoolObjects(Pool p)
    {
        Add(p);
    }

    public void Add(Pool p)
    {
        p.prefab.SetActive(false);

        for (var i = 0; i < p.amount; ++i)
        {
            var clone = UnityEngine.Object.Instantiate(p.prefab);
            clone.SetActive(false);
            queue.Enqueue(clone);
        }
    }

    public void Add(GameObject g, int amount)
    {
        g.SetActive(false);

        for (var i = 0; i < amount; ++i)
        {
            var clone = UnityEngine.Object.Instantiate(g);
            clone.SetActive(false);
            queue.Enqueue(clone);
        }
    }

    public GameObject Get()
    {
        if (queue.Count > 1)
        {
            var obj = queue.Dequeue();
            if (obj.activeSelf == true)
            {
                Debug.Log("Brooo WTF"); // if it eneters here huston we have a fucking problem
            }
            return obj;
        }
        else
        {
            var clone = UnityEngine.Object.Instantiate(queue.Peek());
            clone.SetActive(false);
            Debug.LogWarning(clone.gameObject.name + " DONT EXISTS IN POOL");
            return clone;
        }
    }

    public void Enqueue(GameObject obj)
    {
        obj.SetActive(false);
        queue.Enqueue(obj);
    }

    public T Get<T>()
    {
        return Get().GetComponent<T>();
    }

    public void Shuffle()
    {
        GameObject[] list = queue.ToArray();

        for (var i = 0; i < list.Length; i++)
        {
            var temp = list[i];
            var index = UnityEngine.Random.Range(0, list.Length);
            list[i] = list[index];
            list[index] = temp;
        }

        queue = new Queue<GameObject>(list);
    }
}

[Serializable]
public class Pool
{
    public string key;
    public GameObject prefab;
    public int amount;
    public bool increment = true;
}
