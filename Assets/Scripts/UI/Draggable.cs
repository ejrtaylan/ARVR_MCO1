
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public bool isDragging;
    public bool isOutside;

    void OnTriggerStay2D(Collider2D other) {
        isOutside = false;
    }

    void OnTriggerExit2D(Collider2D other) {
        isOutside = true;
    }
}
