using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
   [SerializeField] private GameObject prefab;
   [SerializeField] private int size;

   private Queue<GameObject> queue;

   private Transform parentTf;

   public GameObject Prefab => prefab;
   

   public void Initialize(Transform parent)
   {
      parentTf = parent;

      queue = new Queue<GameObject>();

      for (var i = 0; i < size; i++)
      {
         queue.Enqueue(Copy());
      }
   }

   private GameObject Copy()
   {
      var copyGO = GameObject.Instantiate(prefab);
      copyGO.SetActive(false);
      copyGO.transform.SetParent(parentTf);
      if (parentTf != null)
      {
         PoolManager.Instance.BindDefaultParentTf(copyGO,parentTf);
      }
      return copyGO;
   }

   private GameObject GetAvailableGO()
   {
      var availableGO = new GameObject();

      if (queue.Count > 0 && !queue.Peek().activeSelf)
      {
         availableGO = queue.Dequeue();
      }
      else
      {
         availableGO = Copy();
      }

      queue.Enqueue(availableGO);
      
      return availableGO;
   }

   public GameObject GetPreparedGO()
   {
      var preparedGO = GetAvailableGO();
      preparedGO.SetActive(true);
      
      return preparedGO;
   }
   public GameObject GetPreparedGO(Vector3 pos)
   {
      var preparedGO = GetAvailableGO();
      preparedGO.transform.position = pos;
      preparedGO.SetActive(true);
      return preparedGO;
   }
   public GameObject GetPreparedGO(Vector3 pos,Quaternion rotation)
   {
      var preparedGO = GetAvailableGO();
      preparedGO.transform.position = pos;
      preparedGO.transform.rotation = rotation;
      preparedGO.SetActive(true);
      return preparedGO;
   }
   public GameObject GetPreparedGO(Vector3 pos,Quaternion rotation,Vector3 scale)
   {
      var preparedGO = GetAvailableGO();
      preparedGO.transform.position = pos;
      preparedGO.transform.rotation = rotation;
      preparedGO.transform.localScale = scale;
      preparedGO.SetActive(true);
      return preparedGO;
   }
   
   
}
