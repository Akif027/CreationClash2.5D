using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
   private Rigidbody rb;
   public float Damage;

   public Collider col;
   void Start()
   {
      // Get the Rigidbody component
      rb = GetComponent<Rigidbody>();

      col = GetComponentInChildren<Collider>();

   }
   public void EnableCollider()
   {
      StartCoroutine(Delay());
   }
   private IEnumerator Delay()
   {

      yield return new WaitForSeconds(1);
      Debug.Log("disabling");
      col.isTrigger = false;
      Destroy(gameObject, 5);

   }
   void FixedUpdate()
   {
      if (rb != null)
      {
         // Reset the rotation while allowing physics for position
         rb.rotation = Quaternion.identity; // Or any specific rotation you want
      }
   }

   private void OnCollisionEnter(Collision collision)
   {
      BodyPart bodyPart = collision.collider.GetComponent<BodyPart>();
      if (bodyPart != null)
      {

         bodyPart.TakeDamage(Damage);
         Destroy(gameObject);
      }
      else
      {
         Debug.Log("Hit something other than a body part.");
      }
   }


}
