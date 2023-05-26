using System;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraRenderEvent : MonoBehaviour
{
    private Camera cam;
    public event Action OnPostRenderEvent;
    
    
    void Start()
    {
        cam = GetComponent<Camera>();
    }


    private void OnPostRender()
    {
        OnPostRenderEvent?.Invoke();
    }
}