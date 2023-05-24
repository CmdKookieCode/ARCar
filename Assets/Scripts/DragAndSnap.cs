using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DragAndSnap : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private float snapDistance = 0.1f;

    private ARRaycastManager raycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    private bool isPlaced = false;
    private bool isDragging = false;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    GameObject spawnedObject;
    private TextMeshProUGUI debug;


    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        targetPosition = GameObject.Find("Car Body").transform.position;
        debug = GameObject.FindGameObjectWithTag("Debug").GetComponent<TextMeshProUGUI>();
        debug.text = "Debug set";

    }

    void Update()
    {
        if (!isPlaced && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Raycast from the touch position to detect AR plane
            var touchPosition = Input.GetTouch(0).position;
            if (raycastManager.Raycast(touchPosition, m_Hits))
            {
                // Instantiate the object at the hit position on the detected plane
                var hitPose = m_Hits[0].pose;
                spawnedObject = Instantiate(objectPrefab, hitPose.position, hitPose.rotation);
                isPlaced = true;
                debug.text = "Spawned object";
            }
        }

        if (isPlaced && !isDragging)
        {
            // Enable dragging for the placed object
            if (spawnedObject != null)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && IsTouchingObject(Input.GetTouch(0).position, spawnedObject))
                {
                    // Start dragging the object
                    isDragging = true;
                    debug.text = "Set dragging to true";
                    initialPosition = spawnedObject.transform.position;
                }
            }
        }

        if (isDragging)
        {
            if (spawnedObject != null)
            {
                // Update the position of the object based on user input (e.g., touch or mouse position)
                var touchPosition = Input.GetTouch(0).position;
                if (raycastManager.Raycast(touchPosition, m_Hits))
                {
                    spawnedObject.transform.position = m_Hits[0].pose.position;
                    debug.text = "Updating pos of spawnedObject";
                }
            }
        }

        if (isDragging && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (spawnedObject != null)
            {
                // Check if the object is close enough to the target position for snapping
                float distance = Vector3.Distance(spawnedObject.transform.position, targetPosition);
                if (distance < snapDistance)
                {
                    // Snap the object to the target position
                    spawnedObject.transform.position = targetPosition;
                    debug.text = "Snapped";
                }
                else
                {
                    // Reset the object to its initial position
                    spawnedObject.transform.position = initialPosition;
                }
                isDragging = false;
            }
        }
    }

    bool IsTouchingObject(Vector2 touchPosition, GameObject gameObject)
    {
        var ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var hitting = hit.collider.gameObject == gameObject;
            debug.text = "Touching object";
            return hitting;
        }
        return false;
    }
}
