using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("Edit Settings")]
    [SerializeField] public bool isEdit;
    [SerializeField] public GameObject EditPanel;
    [SerializeField] public Button exitButton;
    [SerializeField] public GameObject ColorPanel;
    [SerializeField] public GameObject ColorOutline;
    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] public TextMeshProUGUI text2;
 
    void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        DisableColor();
        text.text = "";
    }

    private void Update() {
        CheckEditMode();
        DebugUI();
    }

    void CheckEditMode() {
        if(isEdit) {
            EditPanel.SetActive(true);
        }
        else {
            EditPanel.SetActive(false);
        }
    }

    public void EnableEdit() { isEdit = true; }
    public void DisableEdit() {
        BrickLogic.Instance.DeselectBrick();
        isEdit = false; 
    }

    public void MoveObject() {
        BrickLogic.Instance.MoveObject();
    }

    public void RotateObject() {
        BrickLogic.Instance.RotateObject();
    }

    public void EnableColor() {
        ColorPanel.SetActive(true);
        ColorOutline.SetActive(true);
    }

    public void DisableColor() {
        ColorPanel.SetActive(false);
        ColorOutline.SetActive(false);
    }

    public void ColorObject(int colorID) {
        BrickLogic.Instance.ColorObject(colorID);
    }

    public void DeleteObject() {
        BrickLogic.Instance.DeleteObject();
        isEdit = false;
    }

    public void DebugUI () {
        text.text = $"Position: {GameManager.Instance.pos}";
        text2.text = $"Local Position: {GameManager.Instance.pos2}";
    }

    public void SaveBricks() {
        BrickLogic.Instance.SaveData();
    }

    public void LoadBricks() {
        //BrickLogic.Instance.LoadData();
    }
}
