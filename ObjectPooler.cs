using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
	public int amountToPool;
	public GameObject objectToPool;
	public bool shouldExpand;
}

public class ObjectPooler : MonoBehaviour {

	public static ObjectPooler instance;

	public List<ObjectPoolItem> itemsToPool;
	public List<GameObject> pooledObjects;

	void Awake()
	{
		if (instance != this && instance == null)
		instance = this;
	}

	void Start()
	{
		pooledObjects = new List<GameObject>();
		foreach (ObjectPoolItem item in itemsToPool) {
			for (int i = 0; i < item.amountToPool; i++) {
				GameObject obj = (GameObject) Instantiate(item.objectToPool,transform);
				obj.SetActive(false);
				pooledObjects.Add(obj);
			}
		}
	}

	public GameObject GetPooledObject(string tag)
	{
		for (int i = 0; i < pooledObjects.Count; i++) {
			if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag) {
				return pooledObjects[i];
			}
		}
		foreach (ObjectPoolItem item in itemsToPool) {
			if (item.objectToPool.tag == tag) {
				if (item.shouldExpand) {
					GameObject obj = (GameObject) Instantiate(item.objectToPool,transform);
					obj.SetActive(false);
					pooledObjects.Add(obj);
					return obj;
				}
			}
		}
		return null;
	}

	public GameObject Instantiate(GameObject obj, Vector3 pos, Quaternion rot)
	{
		obj = GetPooledObject(obj.tag);
		obj.transform.position = pos;
		obj.transform.rotation = rot;
		obj.SetActive(true);
		return obj;
	}

}
