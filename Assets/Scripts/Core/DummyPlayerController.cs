using UnityEngine;

/*
 * Needs to be attached to a GameObject with a Rigidbody2D and
 * a BoxCollider2D component.
 * ----------------------
 * MADE TO BE USED IN CONJUNCTION WITH THE TRAJECTORY MANAGER & PLAYER CONTROLLER CLASSES
 * ----------------------
 * A barebones version of the player that can be launched by the trajectory
 * manager at lightning-fast speeds, so it can simulate the player jumping.
 * The dummy's position is then used by the trajectory manager to plot
 * a predicted trajectory line.
 */

namespace PhunkyPhrogs.Core
{
    public class DummyPlayerController : MonoBehaviour
    {
        // The values we're stealing from our player, to simulate them.
        [HideInInspector] public float _speed, _originalGravity;

        // We need to know what should stop us when we jump.
        private LayerMask _jumpableSurfaces;

        // References to our physics components, for jumping and ground checking.
        private Rigidbody2D _rigidbody2D;
        private BoxCollider2D _boxCollider2D;
        private Phrog _phrog;

        // Start is called before the first frame update
        private void Awake()
        {
            // Initialise our components.
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _phrog = FindObjectOfType<Phrog>();
            _speed = _phrog._speed;
            _jumpableSurfaces = _phrog._jumpableSurfaces;
            _originalGravity = _phrog.GetComponent<Rigidbody2D>().gravityScale;
        }

        // Check if we're grounded, using a BoxCast.
        private bool IsGrounded()
        {
            return Physics2D.BoxCast(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0f, Vector2.down, .1f, _jumpableSurfaces);
        }

        // If we're in the air, move to the right. If we're falling, apply
        // the falling gravity scale.
        public void UpdatePosition()
        {
            if (!IsGrounded())
            {
                transform.position += _speed * Time.fixedDeltaTime * Vector3.right;
            }
            if (_rigidbody2D.velocity.y < 0)
            {
                _rigidbody2D.gravityScale = _phrog._fallGravity;
            }
        }
        
        // Reset our gravity scale & velocity.
        public void ResetValues()
        {
            _rigidbody2D.velocity = Vector3.zero;
            _rigidbody2D.gravityScale = _originalGravity;
        }

        // Makes the dummy player jump!
        // Take our fallingGravity variable from whoever's calling the function,
        // then apply our jumpforce.
        public void Jump()
        {
            _rigidbody2D.AddForce(_phrog._jumpForce);
        }
    }
}