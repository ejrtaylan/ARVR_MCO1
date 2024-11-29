using UnityEngine;

public class Brick : MonoBehaviour
{
    [HideInInspector] public BoxCollider Collider;
    
    public void Awake() {
        Collider = GetComponent<BoxCollider>();
    }

    public void SetMaterial(Material mat) {
        this.gameObject.GetComponent<Renderer>().material = mat;
    }
}