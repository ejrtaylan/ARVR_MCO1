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

    public class SaveData {

    }

    public class FileDataWithImage {
        [Serializable]
        public class Header { 
            public int jsonByteSize; 
        }
        public static void Save(string json, Texture2D screenshot) {
            byte[] jsonByteArray = Encoding.Unicode.GetBytes(json);
            byte[] screenshotByteArray = screenshot.EncodeToPNG();

            Header header = new Header {
                jsonByteSize = jsonByteArray.Length
            };
            string headerJson = JsonUtility.ToJson(header);
            byte[] headerJsonByteArray = Encoding.Unicode.GetBytes(headerJson);

            ushort headerSize = (ushort)headerJsonByteArray.Length;
            byte[] headerSizeByteArray = BitConverter.GetBytes(headerSize);

            List<byte> byteList = new List<byte>();
            
            byteList.AddRange(headerSizeByteArray);
            byteList.AddRange(headerJsonByteArray);
            byteList.AddRange(jsonByteArray);
            byteList.AddRange(screenshotByteArray);
            
            File.WriteAllBytes(Application.persistentDataPath + "/Save/SaveFile.bytesave", byteList.ToArray());
        }

        public static void Load() {
            byte[] byteArray = File.ReadAllBytes(Application.dataPath + "/Save/SaveFile.bytesave");
            List<byte> byteList = new List<byte>(byteArray);

            ushort headerSize = BitConverter.ToUInt16(new byte[] { byteArray[0], byteArray[1]}, 0);
            List<byte> headerByteList = byteList.GetRange(2, headerSize);
            string headerJson = Encoding.Unicode.GetString(headerByteList.ToArray());
            Header header = JsonUtility.FromJson<Header>(headerJson);

            List<byte> jsonByteList = byteList.GetRange(2 + headerSize, header.jsonByteSize);
            string gameDataJson = Encoding.Unicode.GetString(jsonByteList.ToArray());
            SaveData saveData = JsonUtility.FromJson<SaveData>(gameDataJson);

            int startIndex = 2 + headerSize + header.jsonByteSize;
            int endIndex = byteArray.Length - startIndex;
            List<byte> screenshotByteList = byteList.GetRange(startIndex, endIndex);
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture2D.LoadImage(screenshotByteList.ToArray());
        }
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

    private string GetSaveDataJSON() {
        return "";
    }

    private void endFrame(ScriptableRenderContext arg1, Camera[] arg2) {
        if(onScreenshotTaken != null) {
            RenderTexture renderTexture = Camera.main.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            RenderTexture.ReleaseTemporary(renderTexture);
            Camera.main.targetTexture = null;

            onScreenshotTaken(renderResult);
            onScreenshotTaken = null;
        }
    }

    public void Save() {
        string SAVE_FOLDER = Application.persistentDataPath;
        string json = GetSaveDataJSON();
        byte[] jsonByteArray = Encoding.Unicode.GetBytes(json);
        File.WriteAllText(SAVE_FOLDER + "/Save/SaveFile.save", json);

        TakeScreenshot(512, 512, (Texture2D screenshotTexture) => {
                byte[] screenshotByteArray = screenshotTexture.EncodeToPNG();
                List<byte> byteList = new List<byte>(jsonByteArray);
                byteList.AddRange(screenshotByteArray);
                File.WriteAllBytes(Application.persistentDataPath + "/Save/SaveFile.png", screenshotByteArray);
        });
    }

    public void Load() {
        string SAVE_FOLDER = Application.persistentDataPath;
        string json = File.ReadAllText(SAVE_FOLDER + "/Save/SaveFileGameData.save");
        byte[] byteArray = File.ReadAllBytes(SAVE_FOLDER + "/Save/");

        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
    }

    void TakeScreenshot(int width, int height, Action<Texture2D> onScreenshotTaken) {
        Camera.main.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        this.onScreenshotTaken = onScreenshotTaken;
    }
}
