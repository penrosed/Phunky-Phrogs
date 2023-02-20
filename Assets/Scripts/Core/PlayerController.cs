using UnityEngine;
using PhunkyPhrogs.TrajectoryLine;

namespace PhunkyPhrogs.Core
{
    public class PlayerController : MonoBehaviour
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
        [SerializeField] private LayerMask _jumpableSurfaces;

        // The start and end vectors for when we click & drag, and the
        // force applied when we jump.
        private Vector2 _dragStart, _dragEnd;
        private Vector2 _jumpForce;

        // References to our physics components & trajectory manager.
        private BoxCollider2D _boxCollider;
        private Rigidbody2D _rigidbody2D;
        private TrajectoryManager _trajectoryManager;

        // Awake is called before any other calls
        private void Awake()
        {
            // Makes it no longer chug on mobile. Spent 4 hours figuring this one line out. FML.
            Application.targetFrameRate = 60;

            // Get component and object references.
            _boxCollider = GetComponent<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _trajectoryManager = GameObject.FindObjectOfType<TrajectoryManager>();

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

        // Update is called once per frame
        void Update()
        {
            if (grounded)                                   // Find out if we're on the ground. If we are...
            {                                               // {
                if (Input.GetMouseButtonDown(0))            //    if we've just started tapping...
                {                                           //    {
                    _dragStart = Input.mousePosition;       //       Set the start of our drag vector
                }                                           //    }
                if (Input.GetMouseButton(0))                //    While our finger's down...
                {                                           //    {
                    _dragEnd = Input.mousePosition;         //       Set the end of our drag vector
                                                            //       Update the amount of force we'll apply when we jump, based on touch input.
                    _jumpForce = GetJumpForce(_dragStart, _dragEnd, _maxJumpForce, _swipeCurve);
                    DrawPredictedTrajectory();              //       Based on that force, get our Trajectory manager to draw a predicted trajectory line.
                }                                           //    }
                                                            //
                if (Input.GetMouseButtonUp(0))              //    If we've released our finger...
                {                                           //    {
                    Jump(_jumpForce);                       //       JUMP!
                    _trajectoryManager.EnableLine(false);   //       Hide the trajectory line.
                }                                           //    }
            }                                               // }
        }

        // Get the trajectory manager to draw where it thinks we'll jump.
        private void DrawPredictedTrajectory()
        {
            if (Input.GetMouseButtonDown(0))        // When we first tap...
            {                                       // }
                _trajectoryManager.InitSimScene();  //    Create a simulated scene with all platforms for our Traj Manager.
            }                                       // }

            // Given our jump force, ask the Trajectory manager to draw a predicted arc.
            _trajectoryManager.SimulateHop(this.transform, _jumpForce, _speed, _originalGravity, _fallGravity);

            _trajectoryManager.EnableLine(true);    // Enable the Trajectory Manager's line renderer.
        }

        // Given the click and drag data, calculates how much force to apply to the player.
        private Vector2 GetJumpForce(Vector2 dragStart, Vector2 dragEnd, float maxJumpForce, AnimationCurve swipeCurve)
        {
            // Find the length of our click & drag (in pixels on the screen)
            float dragScreenPercent = Mathf.Abs((dragEnd - dragStart).magnitude) / Screen.height;

            // By running that number through our swipe curve, find out how
            // much of the maximum jump force we're applying (0 to 100%)
            float amountOfForce = swipeCurve.Evaluate(dragScreenPercent);

            return new(0, amountOfForce * maxJumpForce); // Apply that % to our max jump force.
        }

        // JUMP!
        private void Jump(Vector2 jumpForce)
        {
            _rigidbody2D.AddForce(jumpForce);
            grounded = false;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!grounded) // Only do this once, when 'grounded' is set.
            {
                // This BoxCast will only trigger when it hits an object on the layer designated in 'jumpableSurfaces'.
                grounded = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, .1f, _jumpableSurfaces);

                if (grounded) // If we're grounded...
                {
                    // Reset the gravity scale.
                    _rigidbody2D.gravityScale = _originalGravity;

                    // Reset the touch/drag input.
                    _dragStart = _dragEnd = Input.mousePosition;
                }
            }
        }
    }
}