using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObstacleDataCollection", menuName = "Obstacle/Collection")]
public class ObstacleDataCollection : ScriptableObject
{
   [System.Serializable]
   public class ObstacleType
   {
      public string name;
      public GameObject obstaclePrefab;
      public float speed;
     // public Vector2 size;
      public int damage;
      public int weight; // Spawn frequency weight
   }

   public List<ObstacleType> obstacles;
}
