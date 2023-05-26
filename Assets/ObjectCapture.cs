using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCapture : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Left Key Points")]
    public Transform[] leftKeyPoints;

    [SerializeField]
    [Tooltip("Right Key Points")]
    public Transform[] rightKeyPoints;


    public Material[] materialsToManipulate;


}
