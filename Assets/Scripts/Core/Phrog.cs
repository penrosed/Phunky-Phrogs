using UnityEngine;
using PhunkyPhrogs.TrajectoryLine;

namespace PhunkyPhrogs.Core
{
    public class Phrog : MonoBehaviour
    {
        [Space]
        public float _speed = 3f;
        public float _fallGravity = 4f;
        [SerializeField] private float _maxJumpForce = 600;
        [SerializeField] private AnimationCurve _swipeCurve;
        [HideInInspector] public float _originalGravity;
        [HideInInspector] public float _distance;
        [HideInInspector] public bool grounded;

        [Space] // The layer that contains all collidable surfaces.
        public LayerMask _jumpableSurfaces;

        // The start and end vectors for when we click & drag, and the
        // force applied when we jump.
        [HideInInspector] public Vector2 _jumpForce;

        // References to our physics components & trajectory manager.
        private BoxCollider2D _boxCollider;
        private Rigidbody2D _rigidbody2D;

        // Awake is called before any other calls
        private void Awake()
        {
            // Makes it no longer chug on mobile. Spent 4 hours figuring this one line out. FML.
            Application.targetFrameRate = 60;

            // Get component and object references.
            _boxCollider = GetComponent<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();

            // Remember our original gravity!
            _originalGravity = _rigidbody2D.gravityScale;
        }

        // Fixed update is called at fixed intervals. Used for real-time Physics calculations.
        private void FixedUpdate()
        {
            if (!grounded)                                                          // if we're in the air...
            {                                                                       // {
                _distance += _speed * Time.fixedDeltaTime;                          //    Increase our distance travelled.
                if (_rigidbody2D.velocity.y < 0)                                    //    If we're falling...
                {                                                                   //    {
                    _rigidbody2D.gravityScale = _fallGravity;                       //        Switch to our falling gravity scale.
                }                                                                   //    }
            }                                                                       // }
        }

        // Given the click and drag data, calculates how much force to apply to the player.
        public void GetJumpForce(Vector2 dragStart, Vector2 dragEnd)
        {
            // Find the length of our click & drag (in pixels on the screen)
            float dragScreenPercent = Mathf.Abs((dragEnd - dragStart).magnitude) / Screen.height;

            // By running that number through our swipe curve, find out how
            // much of the maximum jump force we're applying (0 to 100%)
            float amountOfForce = _swipeCurve.Evaluate(dragScreenPercent);

            _jumpForce = new(0, amountOfForce * _maxJumpForce); // Apply that % to our max jump force.
        }

        // JUMP!
        public void Jump()
        {
            _rigidbody2D.AddForce(_jumpForce);
            grounded = false;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!grounded) // Only do this once, when 'grounded' is set.
            {
                // This BoxCast will only trigger when it hits an object on the layer designated in 'jumpableSurfaces'.
                grounded = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, .05f, _jumpableSurfaces);
            }
            else
            {
                // Reset the gravity scale.
                _rigidbody2D.gravityScale = _originalGravity;
            }
        }
    }
}