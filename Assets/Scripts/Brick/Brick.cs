using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;
using UnityEngine.EventSystems;

public class Brick : MonoBehaviour
{
    [HideInInspector] public BoxCollider Collider;
    [SerializeField] public int id;
    [SerializeField] public bool isSelected;
    [SerializeField] public bool init = false;
    [SerializeField] public bool isValid = true;
    [SerializeField] public bool isDrag;
    [SerializeField] public Material mainMaterial;
    [SerializeField] public int materialID;
    [SerializeField] public Vector3 _screenPosition;

    protected bool PositionOk;
    
    public void Awake() {
        Collider = GetComponent<BoxCollider>();
        mainMaterial = GetComponent<Renderer>().material;
    }

    void Update() {
        isSelected = GetComponent<LeanSelectableByFinger>().IsSelected;
        isDrag = Input.GetMouseButton(0) | Input.touchCount == 1;

        if(init) StateHandler();
    }

    void StateHandler() {
        if(isSelected) {
            if(isDrag && isValid && !IsPointerOverUIObject()) DragBrick();
        }
    }

    void DragBrick() {
        _screenPosition = DragController.Instance._screenPosition;
        Ray ray = Camera.main.ScreenPointToRay(_screenPosition + Vector3.up * 0.1f);
        if(Physics.Raycast(ray, out var hitInfo, float.MaxValue, LegoLogic.LayerMaskLego | LegoLogic.LayerMaskGround)) {
            var tempBrickPosition = SnapToGrid(hitInfo.point);
            var placePosition = tempBrickPosition;
            PositionOk = false;
            
            for(int i = 0; i < 10; i++) {
                var collider = Physics.OverlapBox(placePosition + transform.rotation * Collider.center, Collider.size / 2f, transform.rotation, LegoLogic.LayerMaskLego);
                PositionOk = collider.Length == 0;
                if(PositionOk) break;
                else placePosition.y += LegoLogic.Grid.y;
            }

            if(PositionOk) transform.position = placePosition;
            else transform.position = tempBrickPosition;
        }
    }

    public void SetID(int value) {
        id = value;
    }

    public void SetMaterial(Material mat) {
        mainMaterial = mat;
        this.gameObject.GetComponent<Renderer>().material = mainMaterial;
    }

    public void SetMaterialID(int id) {
        materialID = id;
    }

    public void SetCollider(bool value) { 
        this.gameObject.GetComponent<BoxCollider>().enabled = value; 
    }

    public void SelectBrick() {
        GetComponent<Renderer>().material = BrickLogic.Instance.HighlightMat;
        GetComponent<BoxCollider>().enabled = false;

        UIController.Instance.EnableEdit();
        BrickLogic.Instance.SelectedBrick = this;
    }

    public void DeselectBrick() {
        GetComponent<LeanSelectableByFinger>().Deselect();
        GetComponent<Renderer>().material = mainMaterial;
        GetComponent<BoxCollider>().enabled = true;
    }

    public Vector3 SnapToGrid(Vector3 input) {
        var bound = gameObject.GetComponent<MeshFilter>().mesh.bounds;
        Vector3 Grid = Vector3.zero;
        switch(gameObject.tag) {
             case "2x4":
            case "F2x4":
                Grid = new Vector3(bound.size.x * 0.05f / 4, 0.09f * 0.05f, bound.size.z * 0.05f / 2);
                break;
            case "2x2":
                Grid = new Vector3(bound.size.x * 0.05f / 2, 0.09f * 0.05f, bound.size.z * 0.05f / 2);
                break;
            case "1x4":
                Grid = new Vector3(bound.size.x * 0.05f / 4, 0.09f * 0.05f, bound.size.z * 0.05f / 1);
                break;
            case "1x2":
                Grid = new Vector3(bound.size.x * 0.05f / 2, 0.09f * 0.05f, bound.size.z * 0.05f / 1);
                break;
            case "1x1":
                Grid = new Vector3(bound.size.x * 0.05f, 0.09f / 0.05f, bound.size.z * 0.05f);
                break;
        }
        return new Vector3(Mathf.Round(input.x / Grid.x) * Grid.x,
                            Mathf.Round(input.y / Grid.y) * Grid.y,
                            Mathf.Round(input.z / Grid.z) * Grid.z);
    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(GetComponent<BoxCollider>().bounds.center, GetComponent<BoxCollider>().bounds.size);

        var oldMatrix = Gizmos.matrix;
        // create a matrix which translates an object by "position", rotates it by "rotation" and scales it by "halfExtends * 2"
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.GetComponent<BoxCollider>().bounds.extents * 2);
        // Then use it one a default cube which is not translated nor scaled
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldMatrix;
    }
}