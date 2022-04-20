using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTile : MonoBehaviour
{
    private Material _mat;
    private float _moveOffset;

    private void OnEnable()
    {
        _mat = GetComponent<MeshRenderer>().material;
        _moveOffset = 0;
    }

    private void Update()
    {
        _moveOffset += 0.003f;
        _mat.SetTextureOffset("_MainTex", new Vector2(_moveOffset * 0.1f, 0));
    }
}
