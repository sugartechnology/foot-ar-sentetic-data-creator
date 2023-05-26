using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MoverIcon : MonoBehaviour
{

    [SerializeField]
    private int _direction = 0;

    [SerializeField]
    private  Transform _box;


    private void Update()
    {
        AlignWithBox();
    
    }


    public void Move(Vector3 moveAmount)
    {
        if (_box != null)
            ScaleBox(moveAmount);
        else
            transform.position += moveAmount;
    }

    void ScaleBox(Vector3 moveAmount)
    {
        Vector3 d = (_direction == 0) ? _box.transform.forward : _box.transform.right;

        float mdistance = Vector3.Dot(moveAmount, d);
        float sdir = Vector3.Dot((transform.position - _box.transform.position).normalized, d) > 0 ? 1 : -1;

        _box.transform.position += d * 0.5f * mdistance;
        _box.transform.localScale += d * mdistance * sdir;
    }


    void AlignWithBox()
    {
        if (_box == null)
            return;

        if (_direction == 0)
        {
            transform.position = _box.transform.position + _box.forward * (_box.transform.lossyScale.z + transform.lossyScale.y) * 0.5f;
            transform.localScale = new Vector3(_box.transform.localScale.x, 0.1f, 0.2f);
        }

        if (_direction == 1)
        {
            transform.position = _box.transform.position + -_box.right * (_box.transform.lossyScale.x + transform.lossyScale.x) * 0.5f;
            transform.localScale = new Vector3(0.1f, _box.transform.localScale.z, 0.2f);
        }
    }


}
