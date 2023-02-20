using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Needs to be attached to a GameObject with a Rigidbody2D and
 * a BoxCollider2D component.
 * ----------------------
 * MADE TO BE USED IN CONJUNCTION WITH THE TRAJECTORY MANAGER CLASS
 * ----------------------
 * A barebones version of the player that can be launched by the trajectory
 * manager at lightning-fast speeds, so it can simulate the player jumping.
 * The dummy's position is then used by the trajectory manager to plot
 * a predicted trajectory line.
 */

namespace PhunkyPhrogs.TrajectoryLine
{
    public class DummyPlayerController : MonoBehaviour
    {
        // We need to know what should stop us when we jump.
        [SerializeField] private LayerMask _jumpableSurfaces;

        // References to our physics components, for jumping and ground checking.
        private Rigidbody2D _rigidbody2D;
        private BoxCollider2D _boxCollider2D;

        // The gravity scale we fall with.
        private float _fallingGravity;

        // Start is called before the first frame update
        private void Start()
        {
            // Initialise our components.
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Check if we're grounded, using a BoxCast.
        private bool IsGrounded()
        {
            return Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0f, Vector2.down, .1f, _jumpableSurfaces);
        }

        // If we're in the air, move to the right. If we're falling, apply
        // the falling gravity scale.
        public void UpdatePosition(float speed)
        {
            if (!IsGrounded() && !(transform.position.y < -5))
            {
                transform.position += Vector3.right * speed * Time.fixedDeltaTime;
            }
            if (_rigidbody2D.velocity.y < 0)
            {
                _rigidbody2D.gravityScale = _fallingGravity;
            }
        }

        // Makes the dummy player jump!
        // Take our fallingGravity variable from whoever's calling the function,
        // then apply our jumpforce.
        public void Jump(Vector3 jumpForce, float fallingGravity)
        {
            _fallingGravity = fallingGravity;
            _rigidbody2D.AddForce(jumpForce);
        }
    }
}