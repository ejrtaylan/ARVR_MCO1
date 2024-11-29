using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditLogic : MonoBehaviour
{
    [Header("Brick")]
    public Brick SelectedBrick;

    [Header("Materials")]
    public List<Material> ColorList;
    public Material SelectedMaterial;

    [Header("Debug")]
    [SerializeField] public Vector3 _screenPosition;

    void Update() {
        SelectObject();
    }

    void CheckPos() {
        if(Input.GetMouseButton(0)) {
            _screenPosition = Input.mousePosition;
        }
        else if(Input.touchCount > 0) {
            _screenPosition = Input.GetTouch(0).position;
        }
        else return;
    }

    void SelectObject() {
        CheckPos();
        Ray ray = Camera.main.ScreenPointToRay(_screenPosition);
        if(Physics.Raycast(ray, out var hitInfo, float.MaxValue)) {
            if(hitInfo.collider.gameObject.layer == LayerMask.GetMask("Lego")) {
                SelectedBrick = hitInfo.collider.gameObject.GetComponent<Brick>();
                SelectedBrick.GetComponent<Renderer>().material = ColorList[0];
            }
        }
    }
}
