using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class TestPlaneDiplay : MonoBehaviour {

    // The test plane GameObject to Instantiate
    public GameObject testPlanePrefab;

    // The displayed testPlane instance
    private GameObject testPlaneInstance;

    public float rotationSpeed = 3f;

    private bool rotateLeft = false;
    private bool rotateRight = false;

    private Mesh mesh;

    private Vector3[] verts;


    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if(testPlaneInstance == null)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds;

        if(Frame.Raycast(Screen.width / 2, Screen.height / 2, raycastFilter, out hit))
        {
            Vector3 pos = hit.Pose.position;
            testPlaneInstance.transform.position = pos;
        }

        if (rotateRight)
        {
            testPlaneInstance.transform.Rotate(0, rotationSpeed, 0);
        }
        else if (rotateLeft)
        {
            testPlaneInstance.transform.Rotate(0, -rotationSpeed, 0);
        }

	}

    public void DisplayTestPlane(DetectedPlane plane, BoxCollider collider)
    {
        // If an instance already exists, destroy it
        if(testPlaneInstance != null)
        {
            DestroyImmediate(testPlaneInstance);
        }

        GameObject generatedTestPlane = GenerateTestPlane(collider);

        Vector3 pos = plane.CenterPose.position;
        pos.y += 0.1f;
        Vector3 relativePos = GameObject.FindGameObjectWithTag("MainCamera").transform.position - pos;
        Quaternion lookAtRot = Quaternion.LookRotation(relativePos);
        Quaternion rot = new Quaternion(0, lookAtRot.y, 0, 0);

        testPlaneInstance = Instantiate(generatedTestPlane, pos, rot, transform);

    }

    private GameObject GenerateTestPlane(BoxCollider collider)
    {
        // Find the points of the bottom of the collider to create the test plane

        Vector3[] vertices = new Vector3[4];

        Vector3 pointMin = collider.bounds.min;
        Vector3 pointMax = collider.bounds.max;
        vertices[0] = pointMin;
        vertices[1] = new Vector3(pointMax.x, pointMin.y, pointMin.z);
        vertices[2] = new Vector3(pointMin.x, pointMin.y, pointMax.z);
        vertices[3] = new Vector3(pointMax.x, pointMin.y, pointMax.z);

        verts = vertices;

        // Generate the mesh
        testPlanePrefab.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Generated Test Plane";
        mesh.vertices = vertices;

        //Add the triangles
        int[] tri = new int[6];

        // Lower left triangle
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        // Upper right triangle
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        // Set the correct normals
        Vector3[] normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        mesh.normals = normals;

        // Set up correct uv
        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

        return testPlanePrefab;
    }

    public void DestroyTestPlane()
    {
        if(testPlaneInstance != null)
        {
            DestroyImmediate(testPlaneInstance);
        }
    }

    public void LeftRotateDown()
    {
        if (!rotateRight)
        {
            rotateLeft = true;
        }
    }

    public void LeftRotateUp()
    {
        rotateLeft = false;
    }

    public void RightRotateDown()
    {
        if (!rotateLeft)
        {
            rotateRight = true;
        }
    }

    public void RightRotateUp()
    {
        rotateRight = false;
    }

}
