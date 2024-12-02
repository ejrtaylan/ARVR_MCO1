using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class BrickLogic : MonoBehaviour
{
    public static BrickLogic Instance;

    [Header("Brick")]
    public List<GameObject> TempBrickList;
    public List<GameObject> CustomBrickList;
    public List<GameObject> CreatedBrickList;
    public GameObject PrefabBrick;
    public Brick SelectedBrick;
    public GameObject tempBrickHolder;

    [Header("Custom1")]
    public int num1;
    public List<Transform> transform1;
    public List<Vector3> bounds1;
    public List<Mesh> meshFilter1;
    public List<int> materialID1;
    public List<string> tag1;

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
    [SerializeField] public bool isCustom;
    [SerializeField] public int id;
    [SerializeField] public int customID;
    [SerializeField] protected GameObject CurrentBrick;
    protected bool PositionOk;
    protected Vector3 tempBrickPosition;
    protected Bounds customBound;

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
            if(!isCustom) {
                if(isOutside) CurrentBrick.GetComponent<Renderer>().enabled = true;
                else CurrentBrick.GetComponent<Renderer>().enabled = false;
            }
            else {
                foreach(Transform child in CurrentBrick.transform) {
                    if(isOutside) child.GetComponent<Renderer>().enabled = true;
                    else child.GetComponent<Renderer>().enabled = false;
                }
            } 
        }
    }

    private void Update() {
        isDragActive = Input.GetMouseButton(0) | Input.touchCount > 0;
        _screenPosition = DragController.Instance._screenPosition;
        CheckOutside();
        CheckInside();
        if(isCustom == false) CheckDrag();
        else CheckCustomDrag();
        CheckDrop();
    }

    void CheckInside() {
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && CurrentBrick != null && isOutside == false ) {
            Destroy(CurrentBrick.gameObject);
            CurrentBrick = null;
            PrefabBrick = null;
            isCustom = false;
        }
    }

    void CheckDrag() {
        if(isDragActive && CurrentBrick != null) {
            //Debug.Log("Dragging");
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

    void CheckCustomDrag() {
        if(isDragActive && CurrentBrick != null) {
            //Debug.Log("Dragging");
            Ray ray = Camera.main.ScreenPointToRay(_screenPosition + Vector3.up * 0.1f);
            if(Physics.Raycast(ray, out var hitInfo, float.MaxValue, LegoLogic.LayerMaskLego | LegoLogic.LayerMaskGround)) {
                tempBrickPosition = SnapToGridCustom(hitInfo.point);
                var placePosition = tempBrickPosition;
                PositionOk = false;
                for(int i = 0; i < 10; i++) {
                    var collider = Physics.OverlapBox(placePosition + CurrentBrick.transform.rotation * customBound.center, customBound.size / 2f, CurrentBrick.transform.rotation, LegoLogic.LayerMaskLego);
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
            if(!isCustom) {
                CurrentBrick.GetComponent<Brick>().init = true;
                CurrentBrick.GetComponent<Brick>().SetCollider(true);
                CurrentBrick.GetComponent<Brick>().SetMaterial(SelectedColor);
                CurrentBrick.GetComponent<Brick>().SetID(id);
                CreatedBrickList.Add(CurrentBrick);
                id++;
            }
            else {
                foreach(Transform child in CurrentBrick.transform) {
                    child.GetComponent<Brick>().init = true;
                    child.GetComponent<Brick>().SetCollider(true);
                    //child.GetComponent<Brick>().SetMaterial(SelectedColor);
                    child.GetComponent<Brick>().SetID(id);
                    CreatedBrickList.Add(child.gameObject);
                    id++;
                }
            }
            CurrentBrick = null;
            PrefabBrick = null;
            isCustom = false;
        }
    }

    public void SetNextBrick(string brickName) {
        //Debug.Log("Brick Created");
        SwitchBrick(brickName);
        if(!isCustom) {
            CurrentBrick = Instantiate(PrefabBrick);
            CurrentBrick.transform.parent = GameObject.Find("Ground").transform;
            CurrentBrick.GetComponent<Brick>().SetCollider(false);
            CurrentBrick.GetComponent<Brick>().SetMaterial(TransparentMat);
        }
        else SetInitChildrenBricks(brickName);
    }

    void SetInitChildrenBricks(string brickName) {
        switch(brickName) {
            case "Custom1":
                //Debug.Log("Creating Custom Brick.");
                CurrentBrick = Instantiate(tempBrickHolder);
                CurrentBrick.transform.parent = GameObject.Find("Ground").transform;
                // for(int i = 0; i < num1; i++) {
                //     var brick = new GameObject();
                //     brick.name = $"CustomBrick{i}";
                //     brick.transform.parent = CurrentBrick.transform;

                //     brick.transform.localPosition = transform1[i].localPosition;
                //     brick.transform.localRotation = transform1[i].localRotation;
                //     brick.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                //     brick.AddComponent<MeshFilter>();
                //     brick.GetComponent<MeshFilter>().sharedMesh = meshFilter1[i];

                //     brick.AddComponent<MeshRenderer>();
                //     brick.GetComponent<MeshRenderer>().material = ColorList[materialID1[i]];

                //     brick.AddComponent<BoxCollider>();
                //     brick.GetComponent<BoxCollider>().size = bounds1[i];

                //     brick.AddComponent<Brick>();

                //     brick.AddComponent<LeanSelectableByFinger>();
                //     brick.AddComponent<CustomBrickLogic>();

                //     brick.tag = tag1[i];
                //     brick.layer = LayerMask.NameToLayer("Lego");
                // }
                
                foreach(Transform child in CurrentBrick.transform) {
                    child.GetComponent<BoxCollider>().enabled = false;
                }
                break;
        }
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
            case "Custom1":
                isCustom = true;
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
                Grid = new Vector3(bound.size.x * 0.05f / 4, 0.09f * 0.05f, bound.size.z * 0.05f);
                break;
            case "1x2":
                Grid = new Vector3(bound.size.x * 0.05f / 2, 0.09f * 0.05f, bound.size.z * 0.05f);
                break;
        }
        return new Vector3(Mathf.Round(input.x / Grid.x) * Grid.x,
                            Mathf.Round(input.y / Grid.y) * Grid.y,
                            Mathf.Round(input.z / Grid.z) * Grid.z);
    }

    public Vector3 SnapToGridCustom(Vector3 input) {
        //var bound = CurrentBrick.GetComponent<MeshFilter>().mesh.bounds;
        //customBound = GetLocalBoundsForObject(CurrentBrick.gameObject);
        customBound = GetChildRendererBounds(CurrentBrick.gameObject);
        Debug.Log(customBound.size);
        Vector3 Grid = Vector3.zero;
        switch(CurrentBrick.tag) {
            case "Custom1":
                Grid = new Vector3(customBound.size.x * 0.05f, 0.09f * 0.05f, customBound.size.z * 0.05f);
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

    public void SaveBricks() {
        if(CreatedBrickList != null && customID <= 2) {
            customID++;
            switch(customID) {
                case 1:
                    SetCustom1();
                    break;
            }
        }
    }

    void SetCustom1() {
        foreach(var brick in CreatedBrickList) {
            num1++;
            transform1.Add(brick.transform);
            bounds1.Add(brick.GetComponent<BoxCollider>().size);
            meshFilter1.Add(brick.GetComponent<MeshFilter>().sharedMesh);
            materialID1.Add(brick.GetComponent<Brick>().materialID);
            tag1.Add(brick.tag);
        }
    }

    public void LoadBrick() {
        
    }

    static Bounds GetLocalBoundsForObject(GameObject go)
    {
        var referenceTransform = go.transform;
        var b = new Bounds(Vector3.zero, Vector3.zero);
        RecurseEncapsulate(referenceTransform, ref b);
        return b;
                    
        void RecurseEncapsulate(Transform child, ref Bounds bounds)
        {
            var mesh = child.GetComponent<MeshFilter>();
            if (mesh)
            {
                var lsBounds = mesh.sharedMesh.bounds;
                var wsMin = child.TransformPoint(lsBounds.center - lsBounds.extents);
                var wsMax = child.TransformPoint(lsBounds.center + lsBounds.extents);
                bounds.Encapsulate(referenceTransform.InverseTransformPoint(wsMin));
                bounds.Encapsulate(referenceTransform.InverseTransformPoint(wsMax));
            }
            foreach (Transform grandChild in child.transform)
            {
                RecurseEncapsulate(grandChild, ref bounds);
            }
        }
    }

    Bounds GetChildRendererBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1, ni = renderers.Length; i < ni; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            return bounds;
        }
        else
        {
            return new Bounds();
        }
    }
}
