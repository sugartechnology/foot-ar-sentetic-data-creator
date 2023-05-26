using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;




public class Screenshot : MonoBehaviour
{
    // Grab the camera's view when this variable is true.
    private bool grabScreenshot;
    private bool takeScreenshot;
    // Cache variable for our unlit shader
    private Shader unlitTexture;
    [SerializeField]
    [Tooltip("Assign the camera that is taking the screenshot")]
    private CameraRenderEvent cam;

    // Start is called before the first frame update
    [SerializeField]
    [Tooltip("*********")]
    private Image image;
    private RectTransform imageRect;

    // Start is called before the first frame update
    [SerializeField]
    [Tooltip("*********")]
    private Image maskImage;
    private RectTransform maskImageRect;


    // Start is called before the first frame update
    [SerializeField]
    [Tooltip("Detail Texture Camera")]
    private Camera arCamera;

    [SerializeField]
    [Tooltip("Mask Texture Camera")]
    private Camera maskCamera;


    public Image debugButton;

    private RenderTexture imageRT;
    private RenderTexture maskRT;



    [SerializeField]
    [Tooltip("Selected Object Capture")]
    private ObjectCapture[] captures;


    public ARRaycastManager raycastManager;

    private RenderTexture defaultTexture;

    private Vector2 aspectRatio = Vector2.one;

    void Start()
    {
        if (cam == null)
        {
            // Not the most ideal search, Cameras should be tagged for search, or referenced.
            cam = GameObject.FindObjectOfType<CameraRenderEvent>();
        }
        if (cam != null)
        {
            //Subscribe to the Render event from the camera
            cam.OnPostRenderEvent += OnPostRender;
        }
        // cache a reference to the Unlit shader
        //unlitTexture = Shader.Find("Unlit/Texture");

        imageRT = CreateRenderTexture(Screen.width, Screen.height, null);
        imageRect = image.GetComponent<RectTransform>();
        imageRect.sizeDelta = new Vector2(imageRT.width, imageRT.height);

        maskRT = CreateRenderTexture(Screen.width, Screen.height, maskCamera);
        maskImageRect = maskImage.GetComponent<RectTransform>();
        maskImageRect.sizeDelta = new Vector2(maskRT.width, maskRT.height);

       
        aspectRatio = GetAspectRatio(Screen.width, Screen.height);
    }



    private int captureIndex = 0;
    void Update(){

        if(Input.touchCount > 0){

            Vector2 screenPosition = Input.GetTouch(0).position;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds);
            if(hits.Count > 0){
                var cursorPosition = hits[0].pose.position;
                captures[captureIndex].gameObject.SetActive(true);
                captures[captureIndex].gameObject.transform.position = cursorPosition;
            }
        }
    }


    public void ChangeShot()
    {
        var cap = captures[captureIndex].gameObject.active;
        captures[captureIndex].gameObject.SetActive(false);
        captureIndex = (captureIndex + 1) % captures.Length;
        captures[captureIndex].gameObject.SetActive(cap);
    }


    private void FixedUpdate(){
        
        grabScreenshot = takeScreenshot;
        if(grabScreenshot){
            foreach (var capture in captures){
                foreach (var material in capture.materialsToManipulate){
                    var r = Random.Range(0.0f, 1.0f);
                    var g = Random.Range(0.0f, 1.0f);
                    var b = Random.Range(0.0f, 1.0f);
                    material.SetColor("_Color", new Color(r, g, b));

                    var metallic = Random.Range(0.0f, 1.0f);
                    material.SetFloat("_Metallic", metallic);

                    var glossiness = Random.Range(0.0f, 1.0f);
                    material.SetFloat("_Glossiness", glossiness);


                }
            }
        }
    }


    public void TakeScreenshot()
    {
        takeScreenshot = true;
    }

    public void StopScreenshot()
    {
        takeScreenshot = false;
    }

    private Vector2 GetAspectRatio(int screenShotWidth, int screenShotHeight){

            var aspectRatio = Vector2.one;
            //Calculate the aspect ratio and set the scale based upon the orientation of the device
            switch (Screen.orientation)
            {
                case ScreenOrientation.Portrait:
                case ScreenOrientation.PortraitUpsideDown:
                    aspectRatio.y /= (screenShotWidth / (float)screenShotHeight);
                    break;
                default:
                    aspectRatio.x /= (screenShotHeight / (float)screenShotWidth);
                    break;
            }
           
            return aspectRatio;
    }

    private RenderTexture CreateRenderTexture(int screenShotWidth, int screenShotHeight, Camera cam){
            
        var scale = GetAspectRatio(screenShotWidth, screenShotHeight);
            // store in image
        var width = (int)(256 * scale.x);
        var height = (int)(256 * scale.y);

        var rt = new RenderTexture(width, height, 0, RenderTextureFormat.Default);
        rt.Create();
        
        if(cam  != null){
            cam.targetTexture = rt;
        }
        return rt;
    }


    private ShoeData ConvertToLine(Transform[] transorms){
            
        var line = new ShoeData();
        var pointList = new List<Point>();
        line.points = pointList;

        Debug.Log("-------");
        for (int i = 0; i < transorms.Length; i++){
            var viewPoint = arCamera.WorldToViewportPoint(transorms[i].position);
            Debug.Log("viewPoint.x " + viewPoint.x + ", " + "viewPoint.y " + viewPoint.y);
            pointList.Add(new Point(viewPoint.x, viewPoint.y));
        }
        

        return line;
    }


    private List<ShoeData> ConvertToLineList(ObjectCapture capture){
        List<ShoeData> lineList = new List<ShoeData>();
         
        lineList.Add(ConvertToLine(capture.leftKeyPoints));
        lineList.Add(ConvertToLine(capture.rightKeyPoints));
        
        return lineList;
    }

    private bool setRenderTexture = false;
    private void ResetRenderTexture(){
            
        if(setRenderTexture){
            RenderTexture.active = defaultTexture;
        }else{
            defaultTexture = RenderTexture.active;
            setRenderTexture = true;
        }
    }


    private void AddIncremination(){
        var result = PlayerPrefs.GetInt("FileKeyName");
        if(result == null)
            result = 0;
        
        result++;
        PlayerPrefs.SetInt("FileKeyName", result);
    }

    private int GetInt(){
        return PlayerPrefs.GetInt("FileKeyName");
    }


    private void OnPostRender()
    {
        if (grabScreenshot)
        {

            AddIncremination();
            /**********************************/
            /***** Detail Save ******************/
            ResetRenderTexture();
            var screenShotImage = TakeScreenshot(Screen.width, Screen.height);
            screenShotImage = ResizeTexture2D(screenShotImage, (int)(256 * aspectRatio.x), (int)(256 * aspectRatio.y));
            //arCamera.targetTexture = null;

            Destroy(image.sprite);
            var imageRecs = new Rect(0, 0, screenShotImage.width, screenShotImage.height);
            image.sprite = Sprite.Create(screenShotImage, imageRecs, new Vector2(0.5f, 0.5f),1);

            var imageArray = screenShotImage.EncodeToJPG(100);  
            FileUtils.SaveFileAsBinary("/", GetInt() + "_image", imageArray);

            debugButton.color = Color.green;

            /**********************************/
            /***** Mask Save ******************/
            RenderTexture.active = maskRT;
            var screenShotMask = TakeScreenshot(maskRT.width, maskRT.height);
            
            Destroy(maskImage.sprite);
            var maskRec = new Rect(0, 0, maskRT.width, maskRT.height);
            maskImage.sprite = Sprite.Create(screenShotMask, maskRec, new Vector2(0.5f, 0.5f),1); 

            var maskAarray = screenShotMask.EncodeToJPG(100); 
            FileUtils.SaveFileAsBinary("/", GetInt() + "_mask", maskAarray);

            debugButton.color = Color.red;

            /*****************************/
            /***** File Save ******************/
            var id = GetInt();
            Debug.Log("id is " + id);
            var shoes = ConvertToLineList(captures[captureIndex]);
            var scene = new SceneData();
            scene.shoes = shoes;

            var ksonStr = JsonUtility.ToJson(scene);
            FileUtils.SaveFileAsText("/Assets/Data", id + "", ksonStr);

            debugButton.color = Color.blue;

        }
    }


    private Texture2D TakeScreenshot(int width, int height){

            var screenShotBase = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenShotBase.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShotBase.Apply();
            return screenShotBase;
    }


    private Texture2D ResizeTexture2D(Texture2D texture, int width, int height)
    {
        Texture2D result = new Texture2D(width, height, texture.format, false);
        Color[] rpixels = result.GetPixels(0);

        float incX = (1.0f / (float)width);
        float incY = (1.0f / (float)height);

        for (int px = 0; px < rpixels.Length; px++)
        {
            float x = incX * ((float)px % width);
            float y = incY * ((float)Mathf.Floor(px / width));
            rpixels[px] = texture.GetPixelBilinear(x, y);
        }
        result.SetPixels(rpixels, 0);
        result.Apply();

        return result;
    }
}