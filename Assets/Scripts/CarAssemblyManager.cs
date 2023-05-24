using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CarAssemblyManager : MonoBehaviour
{
    public GameObject[] assemblyObjects;

    public Transform assemblySpot;

    private TextMeshProUGUI debug;
    private Canvas ui;

    private Camera arCamera;
    private ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    private Vector2 touchPos;

    public static int currentObjectIndex = -1;
    private GameObject currentAssemblyObject;
    private GameObject currentPlaceholderObject;

    public Material[] mats;

    private void Start()
    {
        arCamera = GameObject.Find("AR Camera").GetComponent<Camera>();
        m_RaycastManager = GameObject.Find("AR Session Origin").GetComponent<ARRaycastManager>();
        debug = GameObject.FindGameObjectWithTag("Debug").GetComponent<TextMeshProUGUI>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<Canvas>();

        ui.transform.Find("Up").GetComponent<Button>().onClick.AddListener(() => MoveUp());
        ui.transform.Find("Down").GetComponent<Button>().onClick.AddListener(() => MoveDown());
        ui.transform.Find("RotateL").GetComponent<Button>().onClick.AddListener(() => RotateLeft());
        ui.transform.Find("RotateR").GetComponent<Button>().onClick.AddListener(() => RotateRight());

        currentObjectIndex = 0;
    }
    bool TryGetTouchPosition(out Vector2 touchPos)
    {
        if (Input.touchCount > 0)
        {
            touchPos = Input.GetTouch(0).position;
            return true;
        }
        touchPos = default;
        return false;
    }

    void Update()
    {

        if (!TryGetTouchPosition(out Vector2 touchPos))
        {
            return;
        }
        if (m_RaycastManager.Raycast(touchPos, m_Hits, TrackableType.PlaneWithinPolygon))
        {
            if (m_Hits[0].trackable.gameObject.tag != "UI")
            {
                var hitPose = m_Hits[0].pose;

                if (Input.GetTouch(0).phase == TouchPhase.Began && currentAssemblyObject == null)
                {
                    if (currentObjectIndex < assemblyObjects.Length)
                    {
                        //SpawnPlaceholderObject();
                        SpawnAssemblyObject(hitPose.position);
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved && currentAssemblyObject != null)
                {
                    currentAssemblyObject.transform.position = hitPose.position;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended && currentAssemblyObject != null)
                {
                    ConfirmPlacement();
                }
            } 
        }
    }

    private void SpawnAssemblyObject(Vector3 touchPos)
    {
        currentAssemblyObject = Instantiate(assemblyObjects[currentObjectIndex], touchPos, Quaternion.identity);
        currentPlaceholderObject = Instantiate(assemblyObjects[currentObjectIndex], transform.position, transform.rotation);
        currentPlaceholderObject.GetComponent<Renderer>().sharedMaterials = mats;
        debug.text = "Assembly object instantiated";

    }

    public void ConfirmPlacement()
    {
        if (currentAssemblyObject != null)
        {
            debug.text = "not null";
            if (IsObjectInCorrectPosition())
            {
                debug.text = "correct position";
                PlaceObjectInAssemblySpot();
                Destroy(currentPlaceholderObject);
            }
            else
            {
                debug.text = "Incorrect placement";
            }
        }
    }

    private bool IsObjectInCorrectPosition()
    {
        float distanceThreshold = 0.1f;
        float distance = Vector3.Distance(currentAssemblyObject.transform.position, assemblySpot.position);

        return distance <= distanceThreshold;
    }

    private void PlaceObjectInAssemblySpot()
    {
        debug.text = "Object assembled";
        currentAssemblyObject.transform.position = assemblySpot.position;
        currentAssemblyObject.transform.rotation = assemblySpot.rotation;
        currentObjectIndex++;

        currentAssemblyObject = null;
        ui.transform.Find("Tutorial").gameObject.SetActive(true);

    }

    public void MoveUp()
    {
        if (currentAssemblyObject != null)
        {
            debug.text = "up";
            currentAssemblyObject.transform.Translate(Vector3.up * 0.025f);
        }
    }

    public void MoveDown()
    {
        if (currentAssemblyObject != null)
        {
            debug.text = "down";
            currentAssemblyObject.transform.Translate(Vector3.down * 0.025f);
        }
    }

    public void MoveLeft()
    {
        if (currentAssemblyObject != null)
        {
            debug.text = "left";
            currentAssemblyObject.transform.Translate(Vector3.left * 0.025f);
        }
    }

    public void MoveRight()
    {
        if (currentAssemblyObject != null)
        {
            debug.text = "right";
            currentAssemblyObject.transform.Translate(Vector3.right * 0.025f);
        }
    }

    public void MoveFurther()
    {
        if (currentAssemblyObject != null)
        {
            debug.text = "further";
            currentAssemblyObject.transform.Translate(Vector3.forward * 0.025f);
        }
    }

    public void MoveCloser()
    {
        if (currentAssemblyObject != null)
        {
            debug.text = "closer";
            currentAssemblyObject.transform.Translate(Vector3.back * 0.025f);
        }
    }

    public void RotateLeft()
    {
        if (currentAssemblyObject != null)
        {
            debug.text = "left";
            currentAssemblyObject.transform.Rotate(Vector3.up, -10f);
        }
    }

    public void RotateRight()
    {
        if (currentAssemblyObject != null)
        {
            debug.text = "right";
            currentAssemblyObject.transform.Rotate(Vector3.up, 10f);
        }
    }
}