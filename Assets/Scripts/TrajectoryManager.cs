using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Needs to be attached to a GameObject with a LineRenderer component.
 * ----------------------
 * MADE TO BE USED IN CONJUNCTION WITH THE DUMMY PLAYER CONTROLLER CLASS
 * ----------------------
 * Creates a new Scene at runtime, with scene paramaters that set its physics
 * system to NOT MOVE unless we call scene.Simulate(time) .
 * 
 * When we want to simulate a jump, we copy the position of every platform on-screen
 * into our simulation scene. (This is done with the CreateSimObjects() function)
 * 
 * We have a 'dummy player' in our Scene: a GameObject with physics components
 * identical to our player. We can call the Jump() function to apply a simulated
 * jump force, then iterate through a bunch of physics simulations as quickly as
 * possible, faster than real-time.
 * 
 * We then get the path the 'dummy' takes and apply it to our line renderer.
 */

public class TrajectoryManager : MonoBehaviour
{
    // The amount of physics simulations we iterate our simulation scene through.
    [SerializeField] int _steps = 20;

    // References to our 'dummy player' and our LineRenderer.
    [SerializeField] private GameObject _dummyPlayer;
    private LineRenderer _arcRenderer;

    // Variables for our simulation scene.
    private PhysicsScene2D _physicsSim;
    Scene _simScene;

    private void Start()
    {
        // Get references to our LineRenderer component.
        _arcRenderer = GetComponent<LineRenderer>();

        CreateSceneParameters _param = new CreateSceneParameters(LocalPhysicsMode.Physics2D);   // Define the parameters of a new scene. This lets us have our own separate physics.
        _simScene = SceneManager.CreateScene("Simulation", _param);                             // Create a new scene and implement the parameters we just created.
        _physicsSim = _simScene.GetPhysicsScene2D();                                            // Assign the physics of the scene so we can simulate on our own time.
        _arcRenderer.positionCount = _steps;                                                    // Set the amount of points our drawn line will have.
    }

    // 1) Destroys every object in our simulation scene, except the 'dummy'.
    // 2) We then copy the 'dummy' and every platform on-screen into the
    //    simulated scene, so our 'dummy' can collide with them.
    public void InitSimScene()
    {
        // Destroy every object in the simulation scene, except the 'dummy'.
        foreach (GameObject GO in _simScene.GetRootGameObjects())
        {
            if(!GO.Equals(_dummyPlayer))
            {
                Destroy(GO);
            }
        }

        // Copy our dummy player into the simulation scene
        SceneManager.MoveGameObjectToScene(_dummyPlayer, _simScene);

        // Copy every platform into the simulation scene.
        foreach (GameObject section in GameObject.FindGameObjectsWithTag("Section"))
        {
            foreach (Transform child in section.transform)
            {
                var newGameObject = Instantiate(child, child.transform.position, child.transform.rotation);
                newGameObject.GetComponent<SpriteRenderer>().enabled = false;
                SceneManager.MoveGameObjectToScene(newGameObject.gameObject, _simScene);
            }
        }
    }


    // Resets the dummy player's position, make the dummy player jump,
    // then iterate our physics scene the number of times found in '_steps'.
    Vector3 _lastForce = Vector3.zero;
    public void SimulateHop(Transform playerTransform, Vector3 force, float speed, float originalGravity, float fallingGravity)
    {
        // Reset the dummy player.
        _dummyPlayer.transform.position = playerTransform.position;
        _dummyPlayer.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        _dummyPlayer.GetComponent<Rigidbody2D>().gravityScale = originalGravity;
        float tempSpeed = speed;

        // Only simulate a jump if the force used is different from last time.
        if (_lastForce != force)
        {
            // Make the dummy player jump.
            DummyPlayerController dummyController = _dummyPlayer.GetComponent<DummyPlayerController>();
            dummyController.Jump(force, fallingGravity);

            // Iterate our physics scene the number of times found in '_steps',
            // then set the position of our dummy player as a point in our LineRenderer.
            for (int i = 0; i < _steps; i++)
            {
                _physicsSim.Simulate(Time.fixedDeltaTime);
                dummyController.UpdatePosition(speed);
                _arcRenderer.SetPosition(i, _dummyPlayer.transform.position);
            }
        }
        _lastForce = force;
    }

    // Turn the trajectory line on/off
    public void EnableLine(bool enabled)
    {
        _arcRenderer.gameObject.SetActive(enabled);
    }
}