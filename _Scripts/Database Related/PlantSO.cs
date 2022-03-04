using UnityEngine;
using System.Linq;

namespace tzdevil.DatabaseRelated
{
    public enum PlantType { Damage, Gold, Tank } // Gold = gold veriyor.
    public enum BulletDirection { Straight, Diagonal}

    [CreateAssetMenu(fileName = "New Plant", menuName = "Create New/Plant", order = 51)]
    public class PlantSO : ScriptableObject
    {
        [HideInInspector] public string PlantName => name;

        [Header("Plant Information")]
        public int PlantValue;
        public int PlantHealth;
        public int PlantCost;
        public PlantType PlantType;
        public Sprite PlantSprite;

        [Header("Bullet Information")]
        public GameObject Bullet;
        public float BulletSpeed;
        public float BulletDamage;
        public float ShootRate;
        public BulletDirection bulletDirection;

        public void ShootMechanic(Transform t)
        {
            if (PlantType == PlantType.Gold || PlantType == PlantType.Tank) return;
            
            // Change the bullet direction to diagonal IF the bullet direction is Diagonal.
            if (bulletDirection == BulletDirection.Diagonal)
                t.localRotation = Quaternion.Euler(new Vector3(0, 0, t.gameObject.name[^2] == '0' ? 22.5f : -22.5f));
             
            // Translate in position.
            t.Translate(BulletSpeed * Time.deltaTime * t.right);
        }
    }
}