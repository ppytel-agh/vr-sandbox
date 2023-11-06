using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pano360Director : MonoBehaviour
{
    string metaSceneName;
    public float exitUpAngle = 20.0f;
    public List<PanoInfo> panosInfo;
    string activePano;
    Dictionary<string, Vector3> connectedPanosDirections = new Dictionary<string, Vector3>();
    public Transform connectedScenesIndicators;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray mousePointRay = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            Debug.Log(mousePointRay.direction);
            float angleBetweenPointedAndUp = Vector3.Angle(Vector3.up, mousePointRay.direction);
            Debug.Log(angleBetweenPointedAndUp);
            if (angleBetweenPointedAndUp < this.exitUpAngle)
            {
                this.exitToMetascene();
            }
            else
            {
                foreach (KeyValuePair<string, Vector3> connectedPanoDirection in this.connectedPanosDirections)
                {
                    float angleBetweenPointerAndPano = Vector3.Angle(connectedPanoDirection.Value, mousePointRay.direction);
                    if (angleBetweenPointerAndPano < this.exitUpAngle)
                    {
                        this.setActivePano(connectedPanoDirection.Key);
                    }
                }
            }

        }
    }

    public void setMetaScene(string sceneName)
    {
        this.metaSceneName = sceneName;
    }

    void setSkybox(Material skyboxCubemap)
    {
        GameObject.FindGameObjectWithTag("pano-skybox").GetComponent<MeshRenderer>().material = skyboxCubemap;
    }

    public void setPanosInfo(List<PanoInfo> panosInfo)
    {
        this.panosInfo = panosInfo;
    }


    public void setActivePano(string panoName)
    {
        foreach (PanoInfo panoInfo in this.panosInfo)
        {
            if (panoInfo.name == panoName)
            {
                this.activePano = panoName;
                this.setSkybox(panoInfo.skyboxCubemap);
                this.connectedPanosDirections.Clear();
                foreach (Transform indicator in this.connectedScenesIndicators)
                {
                    GameObject.Destroy(indicator.gameObject);
                }

                foreach (string connectedPanoName in panoInfo.connectedPanos)
                {
                    if (connectedPanoName != this.activePano)//prevent self reference
                    {
                        foreach (PanoInfo connectedPanoInfo in this.panosInfo)
                        {
                            if (connectedPanoInfo.name == connectedPanoName)
                            {
                                Vector3 positionDiff = connectedPanoInfo.position - panoInfo.position;
                                positionDiff.Normalize();
                                connectedPanosDirections.Add(connectedPanoName, positionDiff);

                                GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                indicator.transform.localScale = 0.05f * Vector3.one;
                                indicator.transform.position = positionDiff;
                                indicator.transform.parent = this.connectedScenesIndicators;
                            }
                        }
                    }
                }
                break;
            }
        }
    }

    public void exitToMetascene()
    {
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(sceneLoadCoroutine(this.metaSceneName));
    }

    IEnumerator sceneLoadCoroutine(string destinationSceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destinationSceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        foreach (Pano360Portal panoPortal in GameObject.FindGameObjectWithTag("pano-manager").GetComponentsInChildren<Pano360Portal>())
        {
            if (panoPortal.gameObject.name == this.activePano)
            {
                GameObject.FindGameObjectWithTag("Player").transform.position = panoPortal.transform.position;
                break;
            }
        }

        Cursor.visible = false;

        //set player position to match pano portal and rotation to match camera in panorama space
        Destroy(this.gameObject);
    }
}
