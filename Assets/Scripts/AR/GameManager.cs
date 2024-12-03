using System;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject debug;
    [SerializeField] private GameObject ground;
    [SerializeField] private bool hasPlaced;
    [SerializeField] public float xOffset;
    [SerializeField] public float yOffset;
    [SerializeField] public float zOffset;

    public Vector3 pos;
    public Vector3 pos2;

    private Action<Texture2D> onScreenshotTaken;

    void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Update() {
        if(GameObject.FindGameObjectWithTag("Debug") != null) CreateDebug();
    }

    void CreateDebug() {
        if(!hasPlaced) {
            var temp = GameObject.FindGameObjectWithTag("Debug");
            pos = temp.transform.position;
            pos2 = new Vector3(pos.x + xOffset, pos.y + yOffset, pos.z + zOffset);

            //var tempObj = Instantiate(debug, pos2, Quaternion.identity);
            var groundObj = Instantiate(ground, pos2, Quaternion.identity);

            hasPlaced = true;
            //var tempObj = Instantiate(debug);
            //tempObj.transform.position = temp.transform.position;
        }
    }
}
