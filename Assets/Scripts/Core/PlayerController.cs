using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using PhunkyPhrogs.TrajectoryLine;

namespace PhunkyPhrogs.Core
{
    public class PlayerController : MonoBehaviour
    {
        [Space]
        public float _speed;
        public float _fallGravity = 1.5f;
        [SerializeField] private float _maxJumpForce = 600;
        [SerializeField] private AnimationCurve _swipeCurve;
        [HideInInspector] public float _originalGravity;
        [HideInInspector] public float _distance;

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
            _boxCollider = GetComponent<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _trajectoryManager = GameObject.FindObjectOfType<TrajectoryManager>();
            _originalGravity = _rigidbody2D.gravityScale;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsGrounded())                                   // Find out if we're on the ground. If we are...
            {                                                   // {
                if (Input.GetMouseButton(0))                    //    If the mouse button's down...
                {                                               //    {
                    UpdateTouchData();                          //       Find out if we've dragged along the touchscreen, and by how much
                    GetJumpForce();                             //       Update the amount of force we'll apply when we jump, based on touch input.
                    DrawPredictedTrajectory();                  //       Based on that force, get our Trajectory manager to draw a predicted trajectory line.
                }                                               //     }
                                                                //
                if (Input.GetMouseButtonUp(0))                  //    If we've released our finger...
                {                                               //    {
                    Jump();                                     //       JUMP!
                    _jumpForce = Vector2.zero;                  //       Reset our jump force.
                    _trajectoryManager.EnableLine(false);       //       Hide the trajectory line.
                }                                               //    }
            }                                                   // }
            else                                                // Otherwise (if we're in the air)...
            {                                                   // {
                _distance += _speed * Time.deltaTime;           //    Increase our distance travelled.
                if (_rigidbody2D.velocity.y < 0)                //    If we're falling...
                {                                               //    {
                    _rigidbody2D.gravityScale = _fallGravity;   //       Switch to our falling gravity scale.
                }                                               //    }
            }                                                   // }
        }

        // Gets the start and end position when we click and drag. Called every frame.
        private void UpdateTouchData()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _dragStart = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                _dragEnd = Input.mousePosition;
            }
        }

        // Get the trajectory manager to draw where it thinks we'll jump.
        private void DrawPredictedTrajectory()
        {
            if (Input.GetMouseButtonDown(0)) // When we first tap...
            {
                _trajectoryManager.InitSimScene();   // Create a simulated scene with all platforms for our Traj Manager.
                _trajectoryManager.EnableLine(true); // Enable the Trajectory Manager's line renderer.
            }
            if (Input.GetMouseButton(0)) // While we're tapping & holding...
            {
                // Given our jump force, ask the Trajectory manager to draw a predicted arc.
                _trajectoryManager.SimulateHop(this.transform, _jumpForce, _speed, _originalGravity, _fallGravity);
            }
        }

        // Given the click and drag data, calculates how much force to apply to the player.
        private void GetJumpForce()
        {
            // Find the length of our click & drag (in pixels on the screen)
            float dragScreenPercent = Mathf.Abs((_dragEnd - _dragStart).magnitude) / Screen.height;

            // By running that number through our swipe curve, find out how
            // much of the maximum jump force we're applying (0 to 100%)
            float amountOfForce = _swipeCurve.Evaluate(dragScreenPercent);

            _jumpForce = new(0, amountOfForce * _maxJumpForce); // Apply that % to our max jump force.
        }

        // JUMP!
        private void Jump()
        {
            _rigidbody2D.AddForce(_jumpForce);
        }

        // Check if the player is grounded using a BoxCast. If we are, reset the gravity scale.
        public bool IsGrounded()
        {
            // This BoxCast will only trigger when it hits an object on the layer designated in 'jumpableSurfaces'.
            bool grounded = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, .1f, _jumpableSurfaces);
            _rigidbody2D.gravityScale = (grounded) ? _originalGravity : _rigidbody2D.gravityScale;
            return grounded;
        }
    }
}