using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tzdevil.DatabaseRelated
{
    [CreateAssetMenu(fileName = "New Zombie", menuName = "Create New/Zombie", order = 52)]
    public class ZombieSO : ScriptableObject
    {
        [HideInInspector] public string ZombieName => name;

        [Header("Zombie Information")]
        public int ZombieAttack;
        public int ZombieHealth;
        [Tooltip("The bigger this number is, the faster zombie moves.")] 
        public float ZombieSpeed;
        [Tooltip("The bigger this number is, the slower the zombie attacks.")]
        public float ZombieAttackSpeed;

        public void MoveMechanic(Transform t)
        {
            // Translate in position.
            t.Translate(- ZombieSpeed * Time.deltaTime * t.right);
        }
    }
}