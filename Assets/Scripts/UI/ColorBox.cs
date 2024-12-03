using UnityEngine;

public class ColorBox : MonoBehaviour  {
    public Transform box;
    public Transform outline;

    private void OnEnable() {
        box.localPosition = new Vector2(Screen.width / 2 - 250, 0.5f);
        box.LeanMoveLocalX(box.localPosition.x - 262.5f, 0.5f).setEaseOutExpo().delay = 0.1f;

        outline.localPosition = new Vector2(Screen.width / 2 - 260, 0.5f);
        outline.LeanMoveLocalX(outline.localPosition.x - 262.5f, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public void CloseEdit() {
        box.LeanMoveLocalX(box.localPosition.x + 262.5f, 0.5f).setEaseInExpo();
        outline.LeanMoveLocalX(outline.localPosition.x + 262.5f, 0.5f).setEaseInExpo().setOnComplete(OnComplete);
    }

    private void OnComplete() {
        box.gameObject.SetActive(false);
        outline.gameObject.SetActive(false);
    }
}