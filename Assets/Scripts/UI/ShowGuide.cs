using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGuide : MonoBehaviour
{
    private Transform box;
    private void OnEnable() {
        box = GetComponent<Transform>();
        box.localPosition = new Vector2(-Screen.width/2 , 0);
        box.LeanMoveLocalX(box.localPosition.x + 525f, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public void CloseUI() {
        box.LeanMoveX(box.localPosition.x - 525f, 0.5f).setEaseInExpo().setOnComplete(OnComplete);
    }

    private void OnComplete() {
        this.gameObject.SetActive(false);
    }
}
