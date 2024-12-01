using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageHandler : MonoBehaviour
{
    [SerializeField] private GameObject debug;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject ground;
    [SerializeField] public GameObject createdPrefab;
    [SerializeField] private float yOffset = 0.05f;
    [SerializeField] private bool hasPlaced = false;
    [SerializeField] public ARTrackedImage arTrackedImage;
    [SerializeField] private ARTrackedImageManager imageManager;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] public Vector3 pos;
    [SerializeField] public Vector3 pos2;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start() {
        imageManager = GetComponent<ARTrackedImageManager>();
        raycastManager = GetComponent<ARRaycastManager>();
    }

    public void OnTrackedImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs) {
        if(hasPlaced == false) {
            foreach(var image in eventArgs.added) {
                arTrackedImage = image;
                if(arTrackedImage != null) {
                    var temp = Instantiate(ground, arTrackedImage.transform);
                    temp.transform.localPosition = new Vector3(0f, yOffset, 0f);
                    pos = temp.transform.position;

                    // var temp = Instantiate(debug, arTrackedImage.transform);
                    // temp.transform.localPosition = new Vector3(0, yOffset, 0);
                    // pos = temp.transform.position;

                    // createdPrefab = Instantiate(prefab);
                    // createdPrefab.transform.position = pos;
                    // pos2 = createdPrefab.transform.localPosition;

                    // Destroy(temp.gameObject);
                    hasPlaced = true;
                }
                
                // Debug.Log($"Tracked New Image: {image.referenceImage.name} | Tracking State: {image.trackingState}");
                // var tempObj = Instantiate(prefab);
                // tempObj.transform.parent = image.gameObject.transform;

                //tempObj.AddComponent<ARAnchor>();
                //tempObj.transform.localPosition = new Vector3(tempObj.transform.position.x, yOffset, tempObj.transform.position.z);
                
                //DebugImageTop.transform.localPosition = new Vector3(image.transform.position.x, image.transform.position.y + yOffset, image.transform.position.z);
                // DebugImageTop.GetComponent<MeshRenderer>().enabled = true;
                // DebugImageTop.AddComponent<ARAnchor>();
                // GameObject parentObj = Instantiate(virtualObjPrefab);
                // parentObj.transform.parent = image.gameObject.transform;
                // parentObj.transform.localPosition = new Vector3(0, yOffset, 0);
                // parentObj.AddComponent<ARAnchor>();
                // Destroy(parentObj.GetComponent<ARAnchor>());

                // hasPlaced = true;    
            }
        }
    }
}