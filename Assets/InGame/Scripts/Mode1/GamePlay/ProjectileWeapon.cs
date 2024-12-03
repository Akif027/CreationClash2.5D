using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
   private Rigidbody rb;
   public float Damage;
   public Collider col;
   public Transform anchor; // The attachment point (WeaponAnchor)

   private bool isLaunched = false; // Flag to check if the weapon is launched

   private void LateUpdate()
   {
      if (!isLaunched && anchor != null)
      {
         // Keep the weapon fixed to the anchor
         transform.position = anchor.position;
         transform.rotation = anchor.rotation;
      }
   }

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
      Debug.Log("Enabling collision.");
      col.isTrigger = false;
      Destroy(gameObject, 5);
   }

   private void FixedUpdate()
   {
      if (rb != null && rb.linearVelocity.sqrMagnitude > 0.1f)
      {
         // Align the forward direction with the velocity
         transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
      }
   }
   public void Launch(Vector3 force)
   {
      if (rb != null)
      {
         Debug.Log("Launching projectile...");
         // Detach from the anchor
         isLaunched = true;
         anchor = null;

         // Enable physics and apply force
         rb.isKinematic = false;
         rb.AddForce(force, ForceMode.VelocityChange);
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
