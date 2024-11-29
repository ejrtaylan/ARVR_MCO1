using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UITransition : MonoBehaviour
{
    [SerializeField] private float indicatorTimer = 1.0f;
    [SerializeField] private float maxIndicatorTimer = 1.0f;
    [SerializeField] private Image radialIndicatorUI = null;
    private bool shouldUpdate = false;
    public bool stop = true;
    public bool isComplete = false;
    public bool isSelect = false;

    public void Select() {
        shouldUpdate = true;
    }

    void Update() {
        if(isComplete == false) {
            Transition();
        }
        else {
            radialIndicatorUI.enabled = false;
            BrickLogic.Instance.SetNextBrick(gameObject.name);
            isComplete = false;
        }
    }

    void Transition() {
        if(shouldUpdate == true) {
            Debug.Log("Selected.");
            stop = false;
            isSelect = true;
            indicatorTimer += Time.deltaTime / 2;
            radialIndicatorUI.fillAmount = indicatorTimer;
            if(indicatorTimer >= maxIndicatorTimer) {
                indicatorTimer = maxIndicatorTimer;
                radialIndicatorUI.fillAmount = maxIndicatorTimer;
                isComplete = true;
            }
        }

        else {
            if(!stop) {
                Debug.Log("Deselected.");
                indicatorTimer -= Time.deltaTime;
                radialIndicatorUI.fillAmount = indicatorTimer;
                if(indicatorTimer <= 0) {
                    stop = true;
                    indicatorTimer = 0;
                    radialIndicatorUI.fillAmount = 0;
                }
            }
        }
    }

    public void Deselect() {
        shouldUpdate = false;
    }
}
