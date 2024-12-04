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

    void Start() {
        imageManager = GetComponent<ARTrackedImageManager>();
    }

    public void OnTrackedImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs) {
        if(hasPlaced == false) {
            foreach(var image in eventArgs.added) {
                arTrackedImage = image;
                if(arTrackedImage != null) {
                    var temp = Instantiate(ground, arTrackedImage.transform);
                    temp.transform.localPosition = new Vector3(0f, yOffset, 0f);
                    hasPlaced = true;
                    BrickLogic.Instance.hasPlaced = true;
                    BrickLogic.Instance.groundPos = arTrackedImage.transform;
                }
            }
        }
    }
}

