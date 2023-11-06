using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class Pano360Portal : XRBaseInteractable
{
    string panoSceneName = "Pano-360-scene";
    public Material skyboxCubemap;
    public List<Pano360Portal> connectedPortals;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void transfer()
    {
        //disable every object except pano manager and xr player rig
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //GameObject panoPortalsManager = GameObject.FindGameObjectWithTag("pano-manager");
        List<GameObject> rootObjectsToKeepEnabled = new List<GameObject>();
        rootObjectsToKeepEnabled.Add(player);
        //rootObjectsToKeepEnabled.Add(panoPortalsManager);        
        List<GameObject> disabledObjects;
        DisableOtherObjectsInScene(rootObjectsToKeepEnabled, out disabledObjects);
        //save previous camera skybox
        Material previousSkybox = RenderSettings.skybox;
        //Camera playerCamera = player.GetComponentInChildren<Camera>();
        //UniversalAdditionalCameraData cameraData = playerCamera.GetUniversalAdditionalCameraData();
        //set material as camera skybox
        RenderSettings.skybox = skyboxCubemap;
        //enable pano platform - blur around platform

        //disable locomotion
        //set exit interactable
        //TODO: manipulate vignette
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

    protected override void OnSelectEntered(SelectEnterEventArgs interactor)
    {
        Debug.Log("triggered pano portal");

        this.transfer();

        base.OnSelectEntered(interactor);
    }
}
