using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tzdevil.LawnRollerRelated
{
    public class LawnRollerBehaviour : MonoBehaviour
    {
        private const float lawnRollerSpeed = 6.0f;

        private bool isHit;

        private void Update()
        {
            // If it's hit, then go forward.
            if (isHit)
                transform.Translate(lawnRollerSpeed * Time.deltaTime * -Vector2.up);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // Check if a zombie touched this lawnroller.
            if (collision.CompareTag("Zombie"))
            {
                // If a zombie touched this lawnmoller for the first time, activate it. If it isn't the first time, run over the zombies to kill them.
                if (!isHit)
                    isHit = true;
                else
                    Destroy(collision.gameObject);
            }
        }
    }
}