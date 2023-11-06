using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoInfo
{
    public string name;
    public Material skyboxCubemap;
    public Vector3 position;
    public List<string> connectedPanos = new List<string>();
}

public class PanoPortalsManager : MonoBehaviour
{
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
}
