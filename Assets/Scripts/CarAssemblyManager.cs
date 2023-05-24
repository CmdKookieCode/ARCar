using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CarAssemblyManager : MonoBehaviour
{
    public GameObject[] assemblyObjects;
    public Material[] mats;

    public Transform assemblySpot;

    private Canvas ui;

    private ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    public static int currentObjectIndex = -1;
    private GameObject currentAssemblyObject;
    private GameObject currentPlaceholderObject;

    private void Start()
    {
        m_RaycastManager = GameObject.Find("AR Session Origin").GetComponent<ARRaycastManager>();

        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<Canvas>();

        SetupButtons();

        currentObjectIndex = 0;
        ui.transform.Find("Tutorial").gameObject.SetActive(true);
    }

    private void SetupButtons()
    {
        GameObject controlsPanel = GameObject.FindGameObjectWithTag("Controls");
        Button[] buttons = controlsPanel.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
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

    void Update()
    {

        if (!TryGetTouchPosition(out Vector2 touchPos))
        {
            return;
        }
        if (m_RaycastManager.Raycast(touchPos, m_Hits, TrackableType.PlaneWithinPolygon))
        {
            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                var hitPose = m_Hits[0].pose;

                if (Input.GetTouch(0).phase == TouchPhase.Began && currentAssemblyObject == null)
                {
                    if (currentObjectIndex < assemblyObjects.Length)
                    {
                        SpawnAssemblyObject(hitPose.position);
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved && currentAssemblyObject != null)
                {
                    Vector3 newPosition = new Vector3(hitPose.position.x, currentAssemblyObject.transform.position.y, hitPose.position.z);
                    currentAssemblyObject.transform.position = newPosition;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended && currentAssemblyObject != null)
                {
                    ConfirmPlacement();
                }
            } 
        }
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

    private void SpawnAssemblyObject(Vector3 touchPos)
    {
        currentAssemblyObject = Instantiate(assemblyObjects[currentObjectIndex], touchPos, Quaternion.identity);
        currentPlaceholderObject = Instantiate(assemblyObjects[currentObjectIndex], transform.position, transform.rotation);

        foreach(Transform transform in currentPlaceholderObject.transform)
        {
            transform.GetComponent<Renderer>().sharedMaterials = mats;
        }
        currentPlaceholderObject.GetComponent<Renderer>().sharedMaterials = mats;

    }

    private void ConfirmPlacement()
    {
        if (currentAssemblyObject != null)
        {
            if (IsObjectInCorrectPosition())
            {
                PlaceObjectInAssemblySpot();
                Destroy(currentPlaceholderObject);
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
        currentAssemblyObject.transform.position = assemblySpot.position;
        currentAssemblyObject.transform.rotation = assemblySpot.rotation;
        currentAssemblyObject.transform.parent = gameObject.transform;
        currentObjectIndex++;
        if(currentObjectIndex == assemblyObjects.Length)
        {
            gameObject.GetComponent<CarController>().enabled = true; ;
        }
        currentAssemblyObject = null;
        ui.transform.Find("Tutorial").gameObject.SetActive(true);
        
    }

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