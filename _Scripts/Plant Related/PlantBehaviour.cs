using System.Threading.Tasks;
using TMPro;
using tzdevil.DatabaseRelated;
using tzdevil.GameplayRelated;
using tzdevil.ZombieRelated;
using UnityEngine;
using UnityEngine.UI;

namespace tzdevil.PlantRelated
{
    public enum PlantState { CanAttack, Stunned, Dead }

    public class PlantBehaviour : MonoBehaviour
    {
        [HideInInspector] public GameObject Area; // hangi objeden geldiði

        [Header("Plant Related")]
        [HideInInspector] public PlantSO Plant;
        private PlantState plantState;
        private float shootRate; // kaç saniyede bir ateþ ettiði
        [HideInInspector] public float PlantHp;

        private TextMeshPro HpText => transform.GetChild(0).GetComponent<TextMeshPro>();
        private Animator Anim => transform.GetChild(1).GetComponent<Animator>();

        private void Start() => Setup();

        private void Setup()
        {
            shootRate = Plant.ShootRate;
            PlantHp = Plant.PlantHealth;
            GetComponent<SpriteRenderer>().sprite = Plant.PlantSprite;
            UISetup();
        }

        private void Update() {
            ShootSetup();

            // Plant HP
            HpText.text = PlantHp.ToString();
        }

        private void UISetup()
        {
            Anim.gameObject.SetActive(false);
        }

        private void ShootSetup()
        {
            // Check if conditions are met.
            if (shootRate > 0) { shootRate -= Time.deltaTime; return; }
            if (!Plant && !Plant.Bullet) return;

            // Shoot a bullet if the Plant Type is Damage
            if (Plant.PlantType == PlantType.Damage)
            {
                // Spawn an additional bullet IF the bullet direction is Diagonal.
                for (int i = 0; i < (Plant.bulletDirection == BulletDirection.Straight ? 1 : 2); i++)
                    SpawnNewBullet(i);
            }
            // Gain gold per X seconds if the Plant Type is Gold.
            else if (Plant.PlantType == PlantType.Gold)
            {
                GameManager.Gold += Plant.PlantValue;
            }
            // Reset ShootTimer.
            shootRate = Plant.ShootRate;
        }

        private void SpawnNewBullet(int bulletName)
        {
            // Spawn the bullet.
            GameObject bullet = Instantiate(Plant.Bullet, transform);
            bullet.name = $"Bullet of {Plant.name} [{bulletName}]";
            bullet.GetComponent<BulletBehaviour>().BulletSpeed = Plant.BulletSpeed;
            bullet.GetComponent<BulletBehaviour>().Plant = Plant;
            Destroy(bullet, 4f);
        }

        private void OnPlantDeath()
        {
            // Reactive the Raycast so you can summon a plant there.
            Area.GetComponent<Image>().raycastTarget = true; 
            Destroy(gameObject);
        }

        public async void TakeDamage(ZombieBehaviour zombie)
        {
            // IF the attacker is still alive, then take damage.
            if (!zombie.gameObject) return;
            PlantHp -= zombie.Zombie.ZombieAttack;

            if (PlantHp <= 0) { OnPlantDeath(); return; }

            // Set up animation.
            Anim.gameObject.SetActive(true);
            Anim.GetComponent<TextMeshPro>().text = $"-{zombie.Zombie.ZombieAttack:0}";
            Anim.enabled = true;

            // Wait until animations finishes.
            await Task.Delay(1000);

            // If the zombie is still alive, then stop the animation.
            if (!gameObject) return;
            Anim.enabled = false;
            Anim.gameObject.SetActive(false);
        }
    }
}