using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Mover : MonoBehaviour
{

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Transform captureObject;

    [SerializeField]
    private ARRaycastManager _raycastManager;

    private int touchCount = 0;
    private Transform _hit;
    private Plane _plane;
    private Vector3 _prevPosition;


    private void Update()
    {
       

        if (!Move())
        {
            Replace();
        }
    }


    private void Replace()
    {
        if (Input.touchCount > 0)
        {

            Vector2 screenPosition = Input.GetTouch(0).position;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            _raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds);
            if (hits.Count > 0)
            {
                var cursorPosition = hits[0].pose.position;
                captureObject.gameObject.SetActive(true);
                captureObject.position = cursorPosition;
            }
        }
    }


    private bool Move()
    {


        bool result = false;
        if (touchCount == 0 && Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {

            Vector2 nTouchPosition = Input.touches.Length > 0 ? Input.touches[0].position : Input.mousePosition;

            Ray ray = _camera.ScreenPointToRay(nTouchPosition);
            int layerMask = LayerMask.GetMask("Icon");
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, layerMask))
            {
                _hit = raycastHit.collider.transform;
                _plane = new Plane(Vector3.up, raycastHit.point);
                _prevPosition = raycastHit.point;
            }
            result = true;
        }


        if (_hit)
        {

            float distance;

            Vector2 nTouchPosition = Input.touches.Length > 0 ? Input.touches[0].position : Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(nTouchPosition);

            Vector3 currentPosition = _prevPosition;
            if (_plane.Raycast(ray, out distance))
            {
                currentPosition = ray.GetPoint(distance);
                var move = currentPosition - _prevPosition;
                var mIcon = _hit.gameObject.GetComponentInChildren<MoverIcon>();
                mIcon.Move(move);
            }


            _prevPosition = currentPosition;
            result = true;
        }



        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && touchCount == 0))
            _hit = null;

        touchCount = Input.touchCount;
        return result;
    }
}
