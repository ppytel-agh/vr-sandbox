using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanoInfo
{
    public string name;
    public Material skyboxCubemap;
    public Vector3 position;
    public List<string> connectedPanos = new List<string>();
}

public class PanoPortalsManager : MonoBehaviour
{
    public GameObject exit;
    List<GameObject> disabledObjects;
    Material previousSkybox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<PanoInfo> getPanosInfo()
    {
        List<PanoInfo> panosList = new List<PanoInfo>();
        foreach(Pano360Portal panoPortal in this.GetComponentsInChildren<Pano360Portal>())
        {
            PanoInfo panoInfo = new PanoInfo();
            panosList.Add(panoInfo);
            panoInfo.name = panoPortal.gameObject.name;
            panoInfo.skyboxCubemap = panoPortal.skyboxCubemap;
            panoInfo.position = panoPortal.transform.position;
            foreach (Pano360Portal connectedPortal in panoPortal.connectedPortals)
            {
                panoInfo.connectedPanos.Add(connectedPortal.gameObject.name);
            }
        }
        return panosList;
    }

    public void enterPano(Material skyboxCubemap)
    {
        //disable every object except pano manager and xr player rig
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Transform playerRoot = player.transform;

        while (playerRoot.parent != null)
        {
            playerRoot = playerRoot.parent;
        }
        //GameObject panoPortalsManager = GameObject.FindGameObjectWithTag("pano-manager");
        List<GameObject> rootObjectsToKeepEnabled = new List<GameObject>();
        rootObjectsToKeepEnabled.Add(this.gameObject);
        rootObjectsToKeepEnabled.Add(playerRoot.gameObject);
        //rootObjectsToKeepEnabled.Add(panoPortalsManager);        
        
        DisableOtherObjectsInScene(rootObjectsToKeepEnabled, out this.disabledObjects);
        //save previous camera skybox
        this.previousSkybox = RenderSettings.skybox;
        //Camera playerCamera = player.GetComponentInChildren<Camera>();
        //UniversalAdditionalCameraData cameraData = playerCamera.GetUniversalAdditionalCameraData();
        //set material as camera skybox
        RenderSettings.skybox = skyboxCubemap;
        //enable pano platform - blur around platform

        //disable locomotion
        //set exit interactable
        this.exit.SetActive(true);
        //place exit above player
        this.exit.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2.5f, player.transform.position.z);

        //TODO: manipulate vignette
    }

    public void exitPano()
    {
        //reenable disabled objects
        foreach (GameObject obj in this.disabledObjects)
        {
            obj.SetActive(true);
        }

        //disable exit interactable
        this.exit.SetActive(false);

        //return previous skybox
        RenderSettings.skybox = this.previousSkybox;
    }

    void DisableOtherObjectsInScene(List<GameObject> rootObjectsToKeepEnabled, out List<GameObject> disabledObjects)
    {
        // Find all GameObjects in the scene
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        disabledObjects = new List<GameObject>();

        // Iterate through all objects
        foreach (GameObject obj in rootObjects)
        {
            // Check if the object should be kept enabled
            if (!rootObjectsToKeepEnabled.Contains(obj) && obj.activeSelf)
            {
                disabledObjects.Add(obj);
                obj.SetActive(false);
            }
        }
    }
}
