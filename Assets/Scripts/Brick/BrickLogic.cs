using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class BrickLogic : MonoBehaviour
{
    public static BrickLogic Instance;

    [Header("Guide")]
    [SerializeField] public GameObject guideBrick;
    [SerializeField] public GameObject tempGuideBrick;

    [Header("Brick")]
    public List<Brick> TempBrickList;
    public List<Brick> CreatedBrickList;
    public Brick PrefabBrick;
    public Brick SelectedBrick;

    [Header("Materials")]
    public List<Material> ColorList;
    public Material SelectedColor;
    public Material HighlightMat;
    public Material TransparentMat;

    [Header("Debug")]
    [SerializeField] public Vector3 _screenPosition;
    [SerializeField] public Vector3 _worldPosition;
    [SerializeField] public bool isDragActive;
    [SerializeField] public bool isOutside;
    [SerializeField] public int id;
    [SerializeField] protected Brick CurrentBrick;
    [SerializeField] public bool hasPlaced;
    [SerializeField] public Transform groundPos;
    protected bool PositionOk;
    protected Vector3 tempBrickPosition;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        SelectedColor = ColorList[0];
    }

    void CheckOutside() {
        if(CurrentBrick != null) {
            isOutside = DragController.Instance._lastDragged.isOutside;
            if(isOutside) CurrentBrick.GetComponent<Renderer>().enabled = true;
            else CurrentBrick.GetComponent<Renderer>().enabled = false;
        }
    }

    private void Update() {
        isDragActive = Input.GetMouseButton(0) | Input.touchCount > 0;
        _screenPosition = DragController.Instance._screenPosition;
        CheckOutside();
        CheckInside();
        CheckDrag();
        CheckDrop();
    }

    void CheckInside() {
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && CurrentBrick != null && isOutside == false ) {
            Destroy(CurrentBrick.gameObject);
            CurrentBrick = null;
            PrefabBrick = null;
        }
    }

    void CheckDrag() {
        if(isDragActive && CurrentBrick != null) {
            Ray ray = Camera.main.ScreenPointToRay(_screenPosition + Vector3.up * 0.1f);
            if(Physics.Raycast(ray, out var hitInfo, float.MaxValue, LegoLogic.LayerMaskLego | LegoLogic.LayerMaskGround)) {
                tempBrickPosition = SnapToGrid(hitInfo.point);
                var placePosition = tempBrickPosition;
                PositionOk = false;
                for(int i = 0; i < 10; i++) {
                    var collider = Physics.OverlapBox(placePosition + CurrentBrick.transform.rotation * CurrentBrick.GetComponent<Brick>().Collider.center, CurrentBrick.GetComponent<Brick>().Collider.size / 2f, CurrentBrick.transform.rotation, LegoLogic.LayerMaskLego);
                    PositionOk = collider.Length == 0;
                    if(PositionOk) break;
                    else placePosition.y += LegoLogic.Grid.y;
                }
                if(PositionOk) CurrentBrick.transform.position = placePosition;
                else CurrentBrick.transform.position = tempBrickPosition;
            }
        }
    }

    void CheckDrop() {
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && CurrentBrick != null && PositionOk) {
            CurrentBrick.GetComponent<Brick>().init = true;
            CurrentBrick.GetComponent<Brick>().SetCollider(true);
            CurrentBrick.GetComponent<Brick>().SetMaterial(SelectedColor);
            CurrentBrick.GetComponent<Brick>().SetID(id);
            CreatedBrickList.Add(CurrentBrick);
            id++;
            CurrentBrick = null;
            PrefabBrick = null;
        }
    }

    public void SetNextBrick(string brickName) {
        SwitchBrick(brickName);
        CurrentBrick = Instantiate(PrefabBrick);
        CurrentBrick.transform.parent = GameObject.Find("Ground").transform;
        CurrentBrick.GetComponent<Brick>().SetCollider(false);
        CurrentBrick.GetComponent<Brick>().SetMaterial(TransparentMat);
    }

    void SwitchBrick(string brickName){
        switch(brickName) {
            case "2x4":
                PrefabBrick = TempBrickList[0];
                break;
            case "2x2":
                PrefabBrick = TempBrickList[1];
                break;
            case "1x4":
                PrefabBrick = TempBrickList[2];
                break;
            case "1x2":
                PrefabBrick = TempBrickList[3];
                break;
            case "1x1":
                PrefabBrick = TempBrickList[4];
                break;
            case "F2x4":
                PrefabBrick = TempBrickList[5];
                break;      
        }
    }

    public Vector3 SnapToGrid(Vector3 input) {
        var bound = CurrentBrick.GetComponent<MeshFilter>().mesh.bounds;
        Vector3 Grid = Vector3.zero;
        switch(CurrentBrick.tag) {
            case "2x4":
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
                Grid = new Vector3(bound.size.x * 0.05f / 1, 0.09f * 0.05f, bound.size.z * 0.05f / 1);
                break;
        }
        return new Vector3(Mathf.Round(input.x / Grid.x) * Grid.x,
                            Mathf.Round(input.y / Grid.y) * Grid.y,
                            Mathf.Round(input.z / Grid.z) * Grid.z);
    }

    public void DeselectBrick() {
        if(SelectedBrick != null) {
            SelectedBrick.GetComponent<LeanSelectableByFinger>().Deselect();
            SelectedBrick = null;
        }
    }

    public void MoveObject() {
        if(SelectedBrick != null) SelectedBrick.isValid = true;
    }

    public void RotateObject() {
        if(SelectedBrick != null) {
            SelectedBrick.isValid = false;
            SelectedBrick.transform.Rotate(Vector3.up, 90);
        }
    }

    public void ColorObject(int colorID) {
        if(SelectedBrick != null) {
            SelectedBrick.isValid = false;
            SelectedBrick.SetMaterial(ColorList[colorID]);
            SelectedBrick.SetMaterialID(colorID);
        }
    }

    public void DeleteObject() {
        if(SelectedBrick != null) {
            SelectedBrick.isValid = false;
            CreatedBrickList.RemoveAt(SelectedBrick.id);
            Destroy(SelectedBrick.gameObject);
            SelectedBrick = null;
        }
    }

    public void CreateGuide() {
        if(hasPlaced) {
            tempGuideBrick = Instantiate(guideBrick, groundPos);
        }
    }

    public void DeleteGuide() {

    }
}
