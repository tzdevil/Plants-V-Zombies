using tzdevil.DatabaseRelated;
using tzdevil.ZombieRelated;
using UnityEngine;

namespace tzdevil.PlantRelated
{
    public class BulletBehaviour : MonoBehaviour
    {
        public float BulletSpeed;
        public PlantSO Plant;

        private void Update()
        {
            // Check if conditions are met to utilize shooting mechanics.
            if (BulletSpeed == 0) return;
            Plant.ShootMechanic(transform);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // On collide with a zombie, deal damage and destroy this.
            if (collision.CompareTag("Zombie"))
            {
                ZombieBehaviour zombie = collision.GetComponent<ZombieBehaviour>();
                zombie.TakeDamage(Plant.BulletDamage);
                Destroy(gameObject);

                // IF the zombie's health hits 0 or under, then call OnZombieDeath function.
                if (zombie.ZombieHp <= 0)
                    zombie.OnZombieDeath();
            }
        }
    }
}