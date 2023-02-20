using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhunkyPhrogs.Core;

namespace PhunkyPhrogs.Environment
{
    public class SectionMover : MonoBehaviour
    {
        private Phrog _phrog;

        // Start is called before the first frame
        private void Start()
        {
            _phrog = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Phrog>();
        }

        // FixedUpdate is called once per frame, for physics interactions
        private void FixedUpdate()
        {
            if (!_phrog.grounded) // If the player's jumping, move.
            {
                transform.position += _phrog._speed * Time.fixedDeltaTime * Vector3.left;

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