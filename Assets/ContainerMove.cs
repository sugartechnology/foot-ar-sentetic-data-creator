using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerMove : MonoBehaviour
{
    [SerializeField]
    private Transform _child;

    [SerializeField]
    private float _zoomOutIn = .5f;

    [SerializeField]
    private float _sideOutIn = 1.0f;


    private Vector3 startPosition;
    private void Awake()
    {
        startPosition = _child.position;
    }

    private void Update()
    {

        _child.transform.position = startPosition + _child.transform.right *Random.Range(-_sideOutIn, _sideOutIn);
        _child.transform.position = _child.transform.position + _child.transform.up * Random.Range(-_zoomOutIn, _zoomOutIn);

        _child.transform.localEulerAngles = new Vector3(0f, 180 * Random.Range(-_zoomOutIn, _zoomOutIn), 0f);
    }
}
