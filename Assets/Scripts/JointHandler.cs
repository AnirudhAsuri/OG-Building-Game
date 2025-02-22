using UnityEngine;
using System.Collections.Generic;

public class JointHandler : MonoBehaviour
{
    private List<GameObject> jointHelpers = new List<GameObject>();
    private List<FixedJoint> fixedJoints = new List<FixedJoint>();
    private Rigidbody rb;
    private BuildingSystem buildingSystem;
    private bool isSnapped = false;  // Track if snapped

    private Vector3[] cornerOffsets = new Vector3[]
    {
        new Vector3(0.5f, 0f, 0.5f),   // Top Right
        new Vector3(-0.5f, 0f, 0.5f),  // Top Left
        new Vector3(0.5f, 0f, -0.5f),  // Bottom Right
        new Vector3(-0.5f, 0f, -0.5f)  // Bottom Left
    };

    private float snapRadius = 0.3f;  // Reduced snap distance

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        buildingSystem = FindObjectOfType<BuildingSystem>(); // Get reference to the BuildingSystem
        if (CompareTag("Board")) CreateFixedJoints();  // Only for boards
    }

    void Update()
    {
        UpdateJointPositions(); // Ensure joints update every frame
    }

    void CreateFixedJoints()
    {
        for (int i = 0; i < cornerOffsets.Length; i++)
        {
            GameObject jointHelper = new GameObject("JointHelper_" + i);
            jointHelper.transform.SetParent(transform);  // Attach to the board
            jointHelper.transform.localPosition = cornerOffsets[i]; // Set correct local position
            jointHelper.transform.localRotation = Quaternion.identity; // Prevent unwanted rotation

            FixedJoint joint = jointHelper.AddComponent<FixedJoint>();
            joint.connectedBody = null;  // Not connected yet

            jointHelpers.Add(jointHelper);
            fixedJoints.Add(joint);
        }
    }

    void UpdateJointPositions()
    {
        for (int i = 0; i < jointHelpers.Count; i++)
        {
            if (jointHelpers[i] != null)
            {
                jointHelpers[i].transform.localPosition = cornerOffsets[i]; // Keep them fixed at the corners
            }
        }
    }

    public void TrySnapToObjects()
    {
        if (buildingSystem.GetCurrentObject() != gameObject) return; // Only allow snapping for the object being placed

        Debug.Log("Right-clicked: Attempting to snap joints!");

        foreach (FixedJoint joint in fixedJoints)
        {
            Collider[] nearbyObjects = Physics.OverlapSphere(joint.transform.position, snapRadius); // Use smaller radius
            foreach (Collider col in nearbyObjects)
            {
                if (col.gameObject == gameObject) continue; // Ignore self

                JointHandler otherJointHandler = col.GetComponent<JointHandler>();
                Rigidbody otherRb = col.GetComponent<Rigidbody>();

                if (otherRb != null && otherJointHandler != null)
                {
                    foreach (FixedJoint otherJoint in otherJointHandler.fixedJoints)
                    {
                        if (otherJoint.connectedBody == null)
                        {
                            joint.connectedBody = otherRb;
                            otherJoint.connectedBody = rb; // Two-way connection
                            isSnapped = true; // Mark as snapped
                            Debug.Log($"Joint connected to {otherRb.gameObject.name}");

                            buildingSystem.SetCurrentObject(null); // Prevent movement
                            return;
                        }
                    }
                }
            }
        }
    }

    public void TryUnSnap()
    {
        if (!isSnapped) return; // If not snapped, do nothing

        Debug.Log("Right-clicked: Unsnap!");

        foreach (FixedJoint joint in fixedJoints)
        {
            joint.connectedBody = null; // Break connection
        }

        isSnapped = false;
        buildingSystem.SetCurrentObject(gameObject); // Allow movement again
    }

    public void LockJoints()
    {
        rb.isKinematic = false;  // Enable physics
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        // Remove joints that are still unconnected
        fixedJoints.RemoveAll(joint => joint.connectedBody == null);
        Debug.Log("Unconnected joints removed.");
    }

    public bool IsSnapped()
    {
        return isSnapped;
    }

    public void ConnectAllPossibleJoints()
    {
        foreach (FixedJoint joint in fixedJoints)
        {
            Collider[] nearbyObjects = Physics.OverlapSphere(joint.transform.position, snapRadius);
            foreach (Collider col in nearbyObjects)
            {
                if (col.gameObject == gameObject) continue; // Ignore self

                JointHandler otherJointHandler = col.GetComponent<JointHandler>();
                Rigidbody otherRb = col.GetComponent<Rigidbody>();

                if (otherRb != null && otherJointHandler != null)
                {
                    foreach (FixedJoint otherJoint in otherJointHandler.fixedJoints)
                    {
                        if (otherJoint.connectedBody == null)
                        {
                            joint.connectedBody = otherRb;
                            otherJoint.connectedBody = rb; // Two-way connection
                            isSnapped = true; // Mark as snapped
                        }
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Color for joints

        if (jointHelpers != null)
        {
            foreach (GameObject helper in jointHelpers)
            {
                if (helper != null)
                {
                    Vector3 jointPos = helper.transform.position;
                    Gizmos.DrawSphere(jointPos, 0.1f); // Small sphere to mark joint position
                }
            }
        }

        // Draw red lines for connected joints
        if (fixedJoints != null)
        {
            foreach (FixedJoint joint in fixedJoints)
            {
                if (joint != null && joint.connectedBody != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(joint.transform.position, joint.connectedBody.position); // Line to connected object
                }
            }
        }
    }
}
