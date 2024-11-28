using UnityEngine;
using System.Collections;

public class PlaceBrick : MonoBehaviour 
{
    public Brick PrefabBrick;
    public Material TransparentMat;
    public Material BrickMat;

    public Vector3 DebugVector;

    protected Brick CurrentBrick;
    protected bool PositionOk;

    [Range(0.1f, 10f)] public float CameraDistance;
    [Range(0.1f, 10f)] public float Scaler = 2f;

    void Start() {
        SetNextBrick();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(CurrentBrick.Collider.center, CurrentBrick.Collider.size / 2);
    }

    void Update() {
        if(CurrentBrick != null) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + Vector3.up * 0.1f * CameraDistance);
            if(Physics.Raycast(ray, out var hitInfo, float.MaxValue, LegoLogic.LayerMaskLego)) {
                var position = LegoLogic.SnapToGrid(hitInfo.point);
                DebugVector = position;
                var placePosition = position;
                PositionOk = false;
                for(int i = 0; i < 10; i++) {
                    var collider = Physics.OverlapBox(placePosition + CurrentBrick.transform.rotation * CurrentBrick.Collider.center, CurrentBrick.Collider.size / Scaler, CurrentBrick.transform.rotation, LegoLogic.LayerMaskLego);
                    PositionOk = collider.Length == 0;
                    if(PositionOk) break;
                    else placePosition.y += LegoLogic.Grid.y;
                }

                if(PositionOk) CurrentBrick.transform.position = placePosition;
                else CurrentBrick.transform.position = position;
            }
        }

        if(Input.GetKeyDown(KeyCode.F) && CurrentBrick != null && PositionOk) {
            CurrentBrick.Collider.enabled = true;
            CurrentBrick.SetMaterial(BrickMat);
            var rot = CurrentBrick.transform.rotation;
            CurrentBrick = null;
            SetNextBrick();
            CurrentBrick.transform.rotation = rot;
        }

        if(Input.GetKeyDown(KeyCode.E)) {
            CurrentBrick.transform.Rotate(Vector3.up, 90);
        }
    }

    public void SetNextBrick() {
        CurrentBrick = Instantiate(PrefabBrick);
        CurrentBrick.Collider.enabled = false;
        CurrentBrick.SetMaterial(TransparentMat);
    }


}