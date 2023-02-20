using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhunkyPhrogs.Core;

namespace PhunkyPhrogs.Environment
{
    public class SectionMover : MonoBehaviour
    {
        private PlayerController _pc;

        // Start is called before the first frame
        private void Start()
        {
            _pc = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        }

        // FixedUpdate is called once per frame, for physics interactions
        private void FixedUpdate()
        {
            if (!_pc.IsGrounded()) // If the player's jumping, move.
            {
                transform.position += Vector3.left * _pc._speed * Time.fixedDeltaTime;

                DestroyCheck(); // Check if we're off the screen.
            }
        }

        // If we're off the screen, destroy ourselves.
        private void DestroyCheck()
        {
            if (transform.position.x < -10f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}