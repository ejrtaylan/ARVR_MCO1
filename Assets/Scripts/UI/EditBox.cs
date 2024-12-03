using UnityEngine;

public class EditBox : MonoBehaviour  {
    public Transform box;

    private void OnEnable() {
        box.localPosition = new Vector2(Screen.width / 2,0);
        box.LeanMoveLocalX(box.localPosition.x - 250f, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public void CloseEdit() {
        box.LeanMoveLocalX(box.localPosition.x + 250f, 0.5f).setEaseInExpo().setOnComplete(OnComplete);
    }

    private void OnComplete() {
        UIController.Instance.isEdit = false;
    }
}