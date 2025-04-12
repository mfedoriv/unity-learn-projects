using UnityEngine;

namespace Weapon
{
    public class Bullet : MonoBehaviour
    {
        private void OnCollisionEnter(Collision hitObject)
        {
            if (hitObject.gameObject.CompareTag("Target"))
            {
                print("Hit " + hitObject.gameObject.name);
                
                CreateBulletImpactEffect(hitObject);
                
                Destroy(gameObject);
            }
            
            if (hitObject.gameObject.CompareTag("Wall"))
            {
                print("Hit a wall");
                
                CreateBulletImpactEffect(hitObject);
                
                Destroy(gameObject);
            }
        }

        private void CreateBulletImpactEffect(Collision hitObject)
        {
            ContactPoint contact = hitObject.GetContact(0);

            GameObject hole = Instantiate(
                GlobalReferences.Instance.bulletPrefabEffectPrefab,
                contact.point,
                Quaternion.LookRotation(contact.normal)
                );
            
            hole.transform.SetParent(hitObject.gameObject.transform);
        }
    }
}
