using UnityEngine;
using System.Collections.Generic;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField] private GameObject boardPrefab;
    [SerializeField] private GameObject wheelPrefab;

    private GameObject currentObject;
    private bool isPlacing = false;
    private bool isPlacingBoard = true;
    private float raycastDistance = 5f;

    private List<GameObject> placedObjects = new List<GameObject>();
    private Quaternion targetRotation = Quaternion.identity;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isPlacingBoard = !isPlacingBoard;
            if (currentObject != null)
            {
                Destroy(currentObject);
                currentObject = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            isPlacing = !isPlacing;
            if (!isPlacing && currentObject != null)
            {
                Destroy(currentObject);
                currentObject = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LockObjects();
        }

        if (isPlacing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentObject == null)
                {
                    PlaceObject();
                }
                else
                {
                    FinalizeObject();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateObject();
            }

            if (currentObject != null)
            {
                FollowMouse();
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right-click for snapping and unsnapping
        {
            if (currentObject != null)
            {
                JointHandler jointHandler = currentObject.GetComponent<JointHandler>();
                if (jointHandler != null)
                {
                    if (jointHandler.IsSnapped()) jointHandler.TryUnSnap(); // Unsnap if already snapped
                    else jointHandler.TrySnapToObjects(); // Snap if not snapped
                }
            }
        }
    }

    void PlaceObject()
    {
        currentObject = Instantiate(isPlacingBoard ? boardPrefab : wheelPrefab);
        currentObject.AddComponent<JointHandler>(); // Assign JointHandler script

        Collider objectCollider = currentObject.GetComponent<Collider>();
        Rigidbody objectRigidbody = currentObject.GetComponent<Rigidbody>();

        if (objectCollider != null) objectCollider.enabled = true; // Enable colliders immediately when placing
        if (objectRigidbody != null) objectRigidbody.isKinematic = true; // Keep physics disabled until locked

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 placePosition = ray.origin + ray.direction * raycastDistance;
        currentObject.transform.position = placePosition;

        placedObjects.Add(currentObject);
    }

    void FinalizeObject()
    {
        currentObject = null;
    }

    void LockObjects()
    {
        Debug.Log("Locking objects...");

        foreach (var obj in placedObjects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            Collider col = obj.GetComponent<Collider>();

            if (rb != null) rb.isKinematic = false; // Enable physics
            if (col != null) col.enabled = true; // Ensure collider stays enabled

            Debug.Log($"Locked: {obj.name}");
        }

        placedObjects.Clear();
    }

    void FollowMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 placePosition = ray.origin + ray.direction * raycastDistance;
        currentObject.transform.position = placePosition;
        currentObject.transform.rotation = targetRotation;
    }

    void RotateObject()
    {
        if (currentObject == null) return;
        targetRotation *= Quaternion.Euler(0f, 0f, 90f);
        currentObject.transform.rotation = targetRotation;
    }

    public GameObject GetCurrentObject()
    {
        return currentObject;
    }

    public void SetCurrentObject(GameObject obj)
    {
        currentObject = obj;
    }
}
