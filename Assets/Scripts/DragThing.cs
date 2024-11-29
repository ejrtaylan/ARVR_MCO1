using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragThing : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public bool _resetPosOnRelease = true;
    Vector3 _startPos;

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    } 

    public void OnBeginDrag(PointerEventData eventData) {
        if(_resetPosOnRelease) _startPos = transform.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);

        var hit = hits.FirstOrDefault(t => t.gameObject.CompareTag("Droppable"));
    }
}