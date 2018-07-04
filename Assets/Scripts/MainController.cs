using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

public class MainController : MonoBehaviour {

    // The first-person camera being used to render the passthrough camera image.
    public Camera FirstPersonCamera;

    // A prefab for tracking and visualizing detected planes.
    public GameObject DetectedPlanePrefab;

    // A game object parenting UI for displaying the "searching for planes" snackbar.
    public GameObject SearchingForPlaneUI;

    // The game object displaying the test plane
    public TestPlaneDiplay testPlaneDisplay;

    // The game object controlling the furniture
    public FurnitureController furnitureController;

    // A game object for displaying the Rotate and Place buttons
    public GameObject PlaceBarUI;
    public GameObject leftRotateButton;
    public GameObject rightRotateButton;
    public Button placeButton;

    // The panel for the Main Menu
    public GameObject mainMenu;

    // A list to hold all planes ARCore is tracking in the current frame. This object is used accross the application to avoid per-frame allocation.
    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

    // True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    private bool m_isQuitting = false;

    // The item selected in the Main Menu
    public GameObject selectedObject;

    // True while the testPlane is displayed
    private bool testPlaneDisplayed = false;

    private bool showPlaceBarUI = false;

    private bool furniturePlaced = false;

    public bool itemChosen = false;

    private GameObject tempSelectedObjectInstance;

    private BoxCollider selectedObjectCollider;

    // Update is called once per frame
    void Update () {

        _UpdateApplicationLifecycle();

        if(selectedObject != null)
        {
            if (!furniturePlaced)
            {
                mainMenu.SetActive(false);
                // Hide SnackBar when currently tracking at least one plane
                Session.GetTrackables<DetectedPlane>(m_AllPlanes);
                bool showSearchingUI = true;
                for (int i = 0; i < m_AllPlanes.Count; i++)
                {
                    if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                    {
                        showSearchingUI = false;
                        showPlaceBarUI = true;
                        if (!testPlaneDisplayed)
                        {
                            TestPlaneOn(m_AllPlanes[i]);
                        }
                        break;
                    }
                }

                SearchingForPlaneUI.SetActive(showSearchingUI);
                PlaceBarUI.SetActive(showPlaceBarUI);

                TrackableHit hit;
                TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

                if (Frame.Raycast(Screen.width / 2, Screen.height / 2, raycastFilter, out hit))
                {
                    placeButton.gameObject.SetActive(true);
                }
                else
                {
                    placeButton.gameObject.SetActive(false);
                }
            }
        

        }
        
	}

    private void TestPlaneOn(DetectedPlane plane)
    {
        testPlaneDisplay.DisplayTestPlane(plane, selectedObjectCollider);
        testPlaneDisplayed = true;
    }

    // Check and update the application life cycle

    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the back button is pressed
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking
        if(Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_isQuitting)
        {
            return;
        }

        // Quit if ARCore is unable to connect and give Unity some time for the toast to appear
        if(Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_isQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_isQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    public void ItemChosen(GameObject item)
    {
        itemChosen = true;
        selectedObject = item;
        mainMenu.SetActive(false);
        tempSelectedObjectInstance = Instantiate(selectedObject);
        //tempSelectedObjectInstance.transform.GetChild(0).gameObject.SetActive(false);
        foreach(Transform child in tempSelectedObjectInstance.transform)
        {
            child.gameObject.SetActive(false);
        }
        selectedObjectCollider = tempSelectedObjectInstance.GetComponent<BoxCollider>();
    }

    // Places the furniture and removes the test plane
    public void PlaceFurniture()
    {
        if(!furniturePlaced)
        {
            FurnitureIsPlaced();
        }
        else
        {
            FurnitureIsNotPlaced();
        }
        
    }

    private void FurnitureIsPlaced()
    {
        Quaternion testPlaneRot = GameObject.FindGameObjectWithTag("TestPlane").transform.rotation;
        furnitureController.SpawnSelectedFurniture(selectedObject, testPlaneRot);
        testPlaneDisplay.DestroyTestPlane();
        testPlaneDisplayed = false;
        furniturePlaced = true;

        // Removes the rotation buttons and changes the place button for a remove button
        leftRotateButton.SetActive(false);
        rightRotateButton.SetActive(false);
        placeButton.GetComponent<Image>().color = Color.red;
        placeButton.GetComponentInChildren<Text>().text = "X";
    }

    private void FurnitureIsNotPlaced()
    {
        leftRotateButton.SetActive(true);
        rightRotateButton.SetActive(true);
        placeButton.GetComponent<Image>().color = Color.green;
        placeButton.GetComponentInChildren<Text>().text = "O";
        furniturePlaced = false;
        furnitureController.RemoveFurniture();
    }


    public void GoBackToMenu()
    {
        DestroyImmediate(tempSelectedObjectInstance);
        FurnitureIsNotPlaced();
        testPlaneDisplay.DestroyTestPlane();
        testPlaneDisplayed = false;
        itemChosen = false; 
        mainMenu.SetActive(true);
        selectedObject = null;
        SearchingForPlaneUI.SetActive(false);
        PlaceBarUI.SetActive(false);
    }

    // Show an Android toast message
    // <param name="message"> Message string to show in the toast </param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if(unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

    // Quit the application
    private void _DoQuit()
    {
        Application.Quit();
    }

}
