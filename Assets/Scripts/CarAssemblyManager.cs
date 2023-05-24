using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarAssemblyManager : MonoBehaviour
{
    public GameObject[] assemblyObjects;
    public Transform assemblySpot;

    public float forwardOffset = 1f;
    public float lowerOffset = 0.5f;

    private TextMeshProUGUI debug;
    private Canvas ui;

    private GameObject arCamera;

    public static int currentObjectIndex = -1;
    private GameObject currentAssemblyObject;
    private bool isObjectPlaced = false;

    private void Start()
    {
        arCamera = GameObject.FindGameObjectWithTag("MainCamera");
        debug = GameObject.FindGameObjectWithTag("Debug").GetComponent<TextMeshProUGUI>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<Canvas>();

        ui.transform.Find("Up").GetComponent<Button>().onClick.AddListener(() => MoveUp());
        ui.transform.Find("Down").GetComponent<Button>().onClick.AddListener(() => MoveDown());
        ui.transform.Find("RotateL").GetComponent<Button>().onClick.AddListener(() => RotateLeft());
        ui.transform.Find("RotateR").GetComponent<Button>().onClick.AddListener(() => RotateRight());

        currentObjectIndex = 0;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && !isObjectPlaced)
            {
                if (currentObjectIndex < assemblyObjects.Length)
                {
                    SpawnAssemblyObject();
                    isObjectPlaced = true;
                }
            }
        }
    }

    private void SpawnAssemblyObject()
    {
        Vector3 spawnPosition = arCamera.transform.position + (arCamera.transform.forward * forwardOffset);
        spawnPosition.y -= lowerOffset;

        currentAssemblyObject = Instantiate(assemblyObjects[currentObjectIndex], spawnPosition, Quaternion.identity);
        debug.text = "object instantiate";
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
            }
            else
            {
                debug.text = "remove object";
                Destroy(currentAssemblyObject);
                isObjectPlaced = false;
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
        debug.text = "placing object";
        currentAssemblyObject.transform.position = assemblySpot.position;
        currentAssemblyObject.transform.rotation = assemblySpot.rotation;
        currentObjectIndex++;
        ui.transform.Find("Tutorial").gameObject.SetActive(true);
        isObjectPlaced = false;
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