
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BrickLogic : MonoBehaviour
{
    public static BrickLogic Instance;

    [Header("Brick")]
    public List<Brick> BrickList;
    public Brick PrefabBrick;

    [Header("Materials")]
    public List<Material> ColorList;
    public Material SelectedMaterial;
    public Material TransparentMat;

    [Header("Debug")]
    [SerializeField] public Vector3 _screenPosition;
    [SerializeField] public Vector3 _worldPosition;
    [SerializeField] public bool isDragActive;
    [SerializeField] public bool isOutside;

    public Brick CurrentBrick;
    protected bool PositionOk;
    protected Vector3 tempBrickPosition;

    void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        SelectedMaterial = ColorList[0];
    }

    void CheckOutside() {
        if(CurrentBrick != null) {
            isOutside = DragController.Instance._lastDragged.isOutside;
            if(isOutside) CurrentBrick.GetComponent<Renderer>().enabled = true;
            else CurrentBrick.GetComponent<Renderer>().enabled = false;
        }
    }

    void Update() {
        isDragActive = DragController.Instance._isDragActive;
        if(Input.GetMouseButtonUp(0) && CurrentBrick != null && isOutside == false ) {
            Destroy(CurrentBrick.gameObject);
            CurrentBrick = null;
            PrefabBrick = null;
        }

        if(isDragActive == true) {
            CheckOutside();

            if(Input.GetMouseButtonDown(0)) {
                _screenPosition = Input.mousePosition;
            }
            else if(Input.touchCount > 0) {
                _screenPosition = Input.GetTouch(0).position;
            }
            else return;

            if(CurrentBrick != null) {
                Ray ray = Camera.main.ScreenPointToRay(_screenPosition + Vector3.up * 0.1f);
                if(Physics.Raycast(ray, out var hitInfo, float.MaxValue, LegoLogic.LayerMaskLego)) {
                    tempBrickPosition = SnapToGrid(hitInfo.point);
                    var placePosition = tempBrickPosition;
                    PositionOk = false;
                    
                    for(int i = 0; i < 10; i++) {
                        var collider = Physics.OverlapBox(placePosition + CurrentBrick.transform.rotation * CurrentBrick.Collider.center, CurrentBrick.Collider.size / 2f, CurrentBrick.transform.rotation, LegoLogic.LayerMaskLego);
                        PositionOk = collider.Length == 0;
                        if(PositionOk) break;
                        else placePosition.y += LegoLogic.Grid.y;
                    }

                    if(PositionOk) CurrentBrick.transform.position = placePosition;
                    else CurrentBrick.transform.position = tempBrickPosition;
                }
            }

            if(Input.GetMouseButtonUp(0) && CurrentBrick != null && PositionOk) {
                CurrentBrick.Collider.enabled = true;
                CurrentBrick.SetMaterial(SelectedMaterial);
                //var rot = CurrentBrick.transform.rotation;
                CurrentBrick = null;
                PrefabBrick = null;
                return;
            }

            if(Input.GetKeyDown(KeyCode.E)) {
                CurrentBrick.transform.Rotate(Vector3.up, 90);
            }
        }
    }

    public void SetNextBrick(string brickName) {
        SwitchBrick(brickName);
        CurrentBrick = Instantiate(PrefabBrick);
        CurrentBrick.Collider.enabled = false;
        CurrentBrick.SetMaterial(TransparentMat);
    }

    void SwitchBrick(string brickName){
        switch(brickName) {
            case "2x4":
                PrefabBrick = BrickList[0];
                break;
            case "2x2":
                PrefabBrick = BrickList[1];
                break;
        }
    }

    public Vector3 SnapToGrid(Vector3 input) {
        var bound = CurrentBrick.GetComponent<MeshFilter>().mesh.bounds;
        Vector3 Grid = Vector3.zero;
        switch(CurrentBrick.tag) {
            case "2x4":
                Grid = new Vector3(bound.size.x / 4, 0.09f, bound.size.z / 2);
                break;
            case "2x2":
                Grid = new Vector3(bound.size.x / 2, 0.09f, bound.size.z / 2);
                break;
        }
        //Vector3 Grid = new Vector3(bound.size.x / 4, 0.09f, bound.size.z / 2);
        return new Vector3(Mathf.Round(input.x / Grid.x) * Grid.x,
                            Mathf.Round(input.y / Grid.y) * Grid.y,
                            Mathf.Round(input.z / Grid.z) * Grid.z);
    }
}
