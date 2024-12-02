using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    public static DragController Instance;
    [SerializeField] public bool _isDragActive = false;
    [SerializeField] public bool hasDragged;
    [SerializeField] public Vector3 _screenPosition;
    [SerializeField] public Vector3 _worldPosition;
    [SerializeField] public Draggable _lastDragged;
    [SerializeField] public string itemName;

    void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update() {
        if(_isDragActive) {
            if(Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)) {
                Drop();
                return;
            }
        }

        if(Input.GetMouseButton(0)) {
            _screenPosition = Input.mousePosition;
        }
        else if(Input.touchCount > 0) {
            _screenPosition = Input.GetTouch(0).position;
        }
        else return;
    
        if(_isDragActive) {
            //Debug.Log("Dragging");
            Drag();
            CheckRaycast();
        }
        else {
            RaycastHit2D hit = Physics2D.Raycast(_screenPosition, Vector2.zero, Mathf.Infinity);
            if(hit.collider != null) {
                //Debug.Log(hit.collider.name);
                Draggable draggable =  hit.transform.gameObject.GetComponent<Draggable>();
                if(draggable != null) {
                    //Debug.Log("Hit");
                    itemName = hit.collider.name;
                    _lastDragged = draggable;
                    InitDrag();
                }
            }
        }

        _worldPosition = Camera.main.ScreenToWorldPoint(_screenPosition);
    }

    void InitDrag() {
        UpdateDragStatus(true);
        BrickLogic.Instance.SetNextBrick(_lastDragged.gameObject.name);
    }

    void Drag() {
        _lastDragged.transform.position = _screenPosition;
    }

    void CheckRaycast() {
        if(_lastDragged.isOutside == true) {
            _lastDragged.GetComponent<Image>().enabled = false;
        }
        else { 
            _lastDragged.GetComponent<Image>().enabled = true;
        }
    }

    void Drop() {
        UpdateDragStatus(false);
        _lastDragged.transform.localPosition = Vector3.zero;
        _lastDragged.GetComponent<Image>().enabled = true;

        _lastDragged = null;
        itemName = "";

        //hasDragged = true;
    }

    void UpdateDragStatus(bool isDragging) {
        _isDragActive = _lastDragged.isDragging = isDragging;
        _lastDragged.gameObject.layer = isDragging ? Layer.Dragging : Layer.Default;
    }
}