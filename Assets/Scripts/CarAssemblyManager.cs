using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CarAssemblyManager : MonoBehaviour
{
    /* This script is responsible for spawning and assembling the different car parts.
     * You will immediately notice there are some errors. This is because we left some code out for you to fill in.
     * Fill in every "TO DO" with the tips we provided and ask for help if you are stuck.*/ 

    // List with all the parts we need to assemble
    public GameObject[] assemblyObjects;

    // List of materials for semi transparent parts
    public Material[] mats;

    /* Every object in a Scene has a Transform. It's used to store and manipulate the position, rotation and scale of the object. 
     * Here we assign the transform of the car foundation prefab so we can get it's position later. */
    public Transform assemblySpot;

    // Our canvas that has all the ui elements.
    private Canvas ui;

    // Variables so we can cast a ray from the tapped location and see where it hits.
    private ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    /* An integer (number that is not a fraction; a whole number) to keep the current index of our assemblyObjects list.
     * We need this index later so we can call to a specific object in the list.
     * Take a look at the tutorial script, where this variable is used too, see if you can figure out why it should be declared -1. */
    public static int currentObjectIndex = -1;

    /* These game objects are used to keep track of the part we are currently placing.
     * In this script, the parts a user is placing are called assembly objects.
     * The parts that are there for user guidance are called placeholder objects. */
    private GameObject currentAssemblyObject;
    private GameObject currentPlaceholderObject;


    /* The Start function is called on initialization of this GameObject. To put in simpler words: it is called when the car foundation has spawned.
     * We use this function to initialize some of our variables. */
    private void Start()
    {
        // We search the RaycastManager and UI in our scene and set them as their variables respectively.
        m_RaycastManager = GameObject.Find("AR Session Origin").GetComponent<ARRaycastManager>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<Canvas>();

        // We call the function that sets up our UI buttons.
        SetupButtons();

        // Set the current object index so it starts at the first item. 
        currentObjectIndex = 0;

        // Lastly we also set the GameObject of our tutorial to active.
        ui.transform.Find("Tutorial").gameObject.SetActive(true);
    }

    /* This function adds an event listener for each control button. 
     * An event listeners is like an observer, which is used to subscribe it's own trigger function that is called when the event takes place. */
    private void SetupButtons()
    {
        // Search our controls panel in the scene and assign it's children (the actual buttons) to a list.
        GameObject controlsPanel = GameObject.FindGameObjectWithTag("Controls");
        Button[] buttons = controlsPanel.GetComponentsInChildren<Button>();

        // Assign OnClick event listeners to each button.
        foreach (Button button in buttons)
        {
            /* A switch case is like an if-else-if statement.
             * The switch statement is used to test the equality of a variable against several values specified in the test cases.
             * Here we want to check the button name, in case the button name is "Up", we add an OnClick event listener that handles the "Up" button being pressed.
             * In other words: if the button name is "Up", add an event listener to that button that calls the MoveUp() function. */
            switch (button.name)
            {
                case "Up":
                    button.onClick.AddListener(MoveUp);
                    break;
                case "Down":
                    button.onClick.AddListener(MoveDown);
                    break;
                case "RotateL":
                    button.onClick.AddListener(RotateLeft);
                    break;
                case "RotateR":
                    button.onClick.AddListener(RotateRight);
                    break;
            }
        }
    }

    // The Update function is automatically called every frame
    void Update()
    {
        // Check if we touched the screen
        if (!TryGetTouchPosition(out Vector2 touchPos))
        {
            return;
        }
        //Check if our touch hit a plane on the ground.
        if (m_RaycastManager.Raycast(touchPos, m_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Check if we are not touching a UI element.
            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                // Put the pose (representation of position and rotation in 3D space) of our intersection point in a variable.
                var hitPose = m_Hits[0].pose;

                /* The touch phase refers to the action the finger has taken on the most recent frame update.
                 * Since a touch is tracked over its "lifetime" by the device, the start and end of a touch and movements in between can be reported on the frames they occur.
                 * Example: TouchPhase.Began means that a finger touched the screen.  */

                // If a finger touched the screen and we do not have our current assembly object set, we need to spawn it.
                if (Input.GetTouch(0).phase == TouchPhase.Began && currentAssemblyObject == null)
                {
                    // Check if we have not placed every object.
                    if (currentObjectIndex < assemblyObjects.Length)
                    {
                        /* Call the function that spawns our object at the touched location. 
                         * Remember that we made a variable containing information about the position of our touch*/
                        SpawnAssemblyObject(hitPose.position);
                    }
                }
                // If a finger moved on the screen and we have a current assembly object, we need to move that object with the finger.
                else if (Input.GetTouch(0).phase == TouchPhase.Moved && currentAssemblyObject != null)
                {
                    Vector3 newPosition = new Vector3(hitPose.position.x, currentAssemblyObject.transform.position.y, hitPose.position.z);
                    currentAssemblyObject.transform.position = newPosition;
                }
                // If a finger was lifted from the screen and we have a current assembly object, we need to confirm it's placement.
                if (Input.GetTouch(0).phase == TouchPhase.Ended && currentAssemblyObject != null)
                {
                    ConfirmPlacement();
                }
            } 
        }
    }

    // This is a function used to find our touch position.
    bool TryGetTouchPosition(out Vector2 touchPos)
    {
        /* First we find out if the screen has been touched. 
         * If so, assign our touch position to the position of our input and return true so we know there has been a touch.
         * Else, use default touch position and return false. */
        if (Input.touchCount > 0)
        {
            touchPos = Input.GetTouch(0).position;
            return true;
        }
        touchPos = default;
        return false;
    }

    // This is a function for spawning the parts that need to be assembled.
    private void SpawnAssemblyObject(Vector3 touchPos)
    {
        /* Instantiate the GameObject in our assemblyObjects list with the index of our currentObjectIndex, at our touch position and with default rotation. 
         * Set this as our current assembly object. */
        currentAssemblyObject = Instantiate(assemblyObjects[currentObjectIndex], touchPos, Quaternion.identity);

        /* Instantiate the same object, but using the position and rotation of the car foundation. This works because the parts are in the correct place by origin.
         * Set this as our current placeholder object. */
        currentPlaceholderObject = Instantiate(assemblyObjects[currentObjectIndex], transform.position, transform.rotation);

        /* We want to change the material of each placeholder object so it looks semi-transparent.
         * Each part consists of multiple GameObjects so we need to change the material of each child object of the current placeholder object.
         * For example: Go to your Prefabs folder under Assets in Unity and double click the Windows prefab.
         * In your Hierarchy and Scene, you can see that the Windows prefab actually consists of 4 objects (called children), each with different materials. */
        foreach(Transform transform in currentPlaceholderObject.transform)
        {
            transform.GetComponent<Renderer>().sharedMaterials = mats;
        }

        // The mazda logo prefab is just one GameObject, so we need to set the materials of the placeholder object itself too. 
        currentPlaceholderObject.GetComponent<Renderer>().sharedMaterials = mats;
    }

    // A function that checks whether the current assembly object is on the correct position & rotation
    private void ConfirmPlacement()
    {
        // If the current assembly object is null, we do not need to check it's position and rotation.
        if (currentAssemblyObject != null)
        {
            /* Call functions to check position and rotation of our current assembly object.
             * If both cases are true, call function that places the car part. 
             * Destroy the placeholder object so it is no longer in our scene. */
            if (IsObjectInCorrectPosition() && IsObjectHavingCorrectRotation())
            {
                PlaceObjectInAssemblySpot();
                Destroy(currentPlaceholderObject);
            }
        }
    }

    // Function that checks whether the distance between the current assembly object and our assembly spot (the car foundation) is small enough.
    private bool IsObjectInCorrectPosition()
    {
        float distanceThreshold = 0.1f;
        float distance = Vector3.Distance(currentAssemblyObject.transform.position, assemblySpot.position);

        return distance <= distanceThreshold;
    }

    // Function that checks whether the difference in rotation between the current assembly object and our assembly spot (the car foundation) is small enough.
    private bool IsObjectHavingCorrectRotation()
    {
        float rotationThreshold = 10f;
        float yRotation = currentAssemblyObject.transform.eulerAngles.y;
        float angleDifference = assemblySpot.transform.eulerAngles.y - yRotation;

        return angleDifference <= rotationThreshold;
    }

    // Function that places the current assembly object at it's correct position.
    private void PlaceObjectInAssemblySpot()
    {
        // Set the position and rotation of the object to the position and rotation of the assembly spot.
        currentAssemblyObject.transform.position = assemblySpot.position;
        currentAssemblyObject.transform.rotation = assemblySpot.rotation;

        // Make the current assembly object a child of the car foundation. Go to the CarController script to find out why.
        currentAssemblyObject.transform.parent = gameObject.transform;

        // Add 1 to our current object index so we spawn the succeeding part next time we tap.
        currentObjectIndex++;

        /* We need to check whether or not we have placed all our parts and if so, activate a script that completes the assembly.
         * This is easily achievable by comparing the length of our asseblyObjects list to our currentObjectIndex.
         * The last item in a list has an index of the length of this list -1.
         * Exapmle: if a list has 3 items, the index of the last item would be 2, since indexes start at 0.
         * So if our index equals 2 in this example, we still have not placed our last object in the list.
         * Try to fill in the correct operators based on this information. This site lists the different operators you can use https://helpdesk.castoredc.com/introduction-to-calculations-the-basics/using-the-if-else-logic */
        if (currentObjectIndex == assemblyObjects.Length)
        {
            gameObject.GetComponent<CarController>().enabled = true; ;
        }

        // Make our current assembly object empty so we can assign a new one on the next tap.
        currentAssemblyObject = null;

        /* Same story as before but in reverse: as long as we have not placed every object, set our tutorial panel active again.
         * Try to fill in the correct operators based on previous information. */
        if (currentObjectIndex < assemblyObjects.Length)
        {
            ui.transform.Find("Tutorial").gameObject.SetActive(true);
        }        
    }

    // Functions that handle each of the control buttons clicks. They manipulate the position of the current assembly object.
    private void MoveUp()
    {
        if (currentAssemblyObject != null)
        {
            currentAssemblyObject.transform.Translate(Vector3.up * 0.025f);
        }
    }

    private void MoveDown()
    {
        if (currentAssemblyObject != null)
        {
            currentAssemblyObject.transform.Translate(Vector3.down * 0.025f);
        }
    }

    private void RotateLeft()
    {
        if (currentAssemblyObject != null)
        {
            currentAssemblyObject.transform.Rotate(Vector3.up, -10f);
        }
    }

    private void RotateRight()
    {
        if (currentAssemblyObject != null)
        {
            currentAssemblyObject.transform.Rotate(Vector3.up, 10f);
        }
    }
}