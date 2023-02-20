using UnityEngine;

namespace PhunkyPhrogs.Core
{
    public class InputManager : MonoBehaviour
    {
        private Phrog _phrog;
        private TrajectoryManager _trajectoryManager;
        private Vector2 _dragStart, _dragEnd;

        // Awake is called before any other calls
        private void Awake()
        {
            _phrog = GetComponent<Phrog>();
            _trajectoryManager = FindObjectOfType<TrajectoryManager>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && _phrog.grounded)                     // if we've just started tapping, and we're on the ground...
            {                                                                       // {
                _dragStart = Input.mousePosition;                                   //    Set the start of our drag vector.
                _trajectoryManager.InitSimScene();                                  //    Create a simulated scene with all platforms for our Trajectory Manager.
            }                                                                       // }
            if (Input.GetMouseButton(0) && _phrog.grounded)                         // While our finger's down, and we're on the ground...
            {                                                                       // {
                _dragEnd = Input.mousePosition;                                     //    Set the end of our drag vector.
                _phrog.GetJumpForce(_dragStart, _dragEnd);                          //    Calculate our jump force.
                _trajectoryManager.SimulateHop(this.transform, _phrog._jumpForce);  //    Given our jump force, ask the Trajectory manager to draw a predicted arc.
                _trajectoryManager.EnableLine(true);                                //    Enable the Trajectory Manager's line renderer.
            }                                                                       // }
            if (Input.GetMouseButtonUp(0) && _phrog.grounded)                       // When our finger releases, and we're on the ground...
            {                                                                       // {
                _phrog.Jump();                                                      //    Jump!
                _trajectoryManager.EnableLine(false);                               //    Hide the trajectory line.
            }                                                                       // }
        }
    }
}
