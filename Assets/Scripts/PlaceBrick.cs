using UnityEngine;
using System.Collections;

public class PlaceBrick : MonoBehaviour 
{
    public Brick PrefabBrick;
    public Material TransparentMat;
    public Material BrickMat;
    public Vector3 DebugVector;
    public Vector3 DebugSize;
    protected Brick CurrentBrick;
    protected bool PositionOk;
    protected Vector3 tempBrickPosition;

    [Range(0.1f, 10f)] public float CameraDistance;
    [Range(0.1f, 10f)] public float Scaler = 2f;

    public void SetNextBrick() {
        CurrentBrick = Instantiate(PrefabBrick);
    }

    // void Update() {
    //     if(CurrentBrick != null) {
    //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + Vector3.up * 0.1f);
    //         var bound = CurrentBrick.GetComponent<MeshFilter>().mesh.bounds;
    //         if(Physics.Raycast(ray, out var hitInfo, float.MaxValue, LegoLogic.LayerMaskLego)) {
    //             tempBrickPosition = SnapToGrid(hitInfo.point);
    //             DebugVector = tempBrickPosition;
    //             var placePosition = tempBrickPosition;
    //             PositionOk = false;
    //             for(int i = 0; i < 10; i++) {
    //                 var collider = Physics.OverlapBox(placePosition + CurrentBrick.transform.rotation * CurrentBrick.Collider.center, CurrentBrick.Collider.size / Scaler, CurrentBrick.transform.rotation, LegoLogic.LayerMaskLego);
    //                 PositionOk = collider.Length == 0;
    //                 if(PositionOk) break;
    //                 else placePosition.y += LegoLogic.Grid.y;
    //             }

    //             if(PositionOk) CurrentBrick.transform.position = placePosition;
    //             else CurrentBrick.transform.position = tempBrickPosition;
    //         }
    //     }

    //     if(Input.GetKeyDown(KeyCode.F) && CurrentBrick != null && PositionOk) {
    //         CurrentBrick.Collider.enabled = true;
    //         CurrentBrick.SetMaterial(BrickMat);
    //         var rot = CurrentBrick.transform.rotation;
    //         CurrentBrick = null;
    //         SetNextBrick();
    //         CurrentBrick.transform.rotation = rot;
    //     }

    //     if(Input.GetKeyDown(KeyCode.E)) {
    //         CurrentBrick.transform.Rotate(Vector3.up, 90);
    //     }
    // }

    // public void SetNextBrick() {
    //     CurrentBrick = Instantiate(PrefabBrick);
    //     CurrentBrick.Collider.enabled = false;
    //     CurrentBrick.SetMaterial(TransparentMat);
    //     DebugSize = CurrentBrick.GetComponent<MeshFilter>().mesh.bounds.size;
    // }

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