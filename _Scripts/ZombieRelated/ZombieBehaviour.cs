using System.Threading.Tasks;
using TMPro;
using tzdevil.DatabaseRelated;
using tzdevil.GameplayRelated;
using tzdevil.PlantRelated;
using UnityEngine;

namespace tzdevil.ZombieRelated
{
    public class ZombieBehaviour : MonoBehaviour
    {
        //[Header("Zombie Related")]
        [HideInInspector] public ZombieSO Zombie;
        private bool isAttacking;
        [HideInInspector] public float ZombieHp;
        public float attackSpeed;

        private PlantBehaviour plant;

        private TextMeshPro HpText => transform.GetChild(0).GetComponent<TextMeshPro>();
        private Animator Anim => transform.GetChild(1).GetComponent<Animator>();

        private void Start() => Setup();

        private void Setup()
        {
            ZombieHp = Zombie.ZombieHealth;
            attackSpeed = .1f;

            UISetup();
        }

        private void UISetup()
        {
            Anim.gameObject.SetActive(false);
        }

        private void Update()
        {
            // Attack depending on a bool:isAttacking and a float:attackSpeed.
            if (isAttacking)
            {
                if (attackSpeed > 0) attackSpeed -= Time.deltaTime;
                else
                {
                    plant.TakeDamage(this);
                    attackSpeed = Zombie.ZombieAttackSpeed;
                }
            }
            Zombie.MoveMechanic(transform);

            // Zombie HP.
            HpText.text = ZombieHp.ToString();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // On collide with a plant, start attacking.
            if (collision.gameObject.CompareTag("Plant"))
            {
                plant = collision.gameObject.GetComponent<PlantBehaviour>();
                attackSpeed = Zombie.ZombieAttackSpeed;
                isAttacking = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            // Stop attacking if the plant is dead.
            if (collision.gameObject.CompareTag("Plant"))
            {
                isAttacking = false;
                plant = null;
            }
        }

        public async void TakeDamage(float damageTaken)
        {
            // Deal damage to zombie and set up damage taken animation.
            ZombieHp -= damageTaken;
            Anim.GetComponent<TextMeshPro>().text = $"-{damageTaken:0}";
            Anim.gameObject.SetActive(true);
            Anim.enabled = true;

            // Wait until animations finishes.
            await Task.Delay(1000);

            // If the zombie is still alive, then stop the animation.
            if (!this) return;
            Anim.enabled = false;
            Anim.gameObject.SetActive(false);
        }

        public void OnZombieDeath()
        {
            // Remove any Action and check if this was the last zombie before Destroy().
            isAttacking = false;
            //Destroy(gameObject, .2f); // efektler mesela.

            if (GameManager.totalZombieThisRound <= 0) GameManager.EndRound();
            Destroy(gameObject);
        }
    }
}