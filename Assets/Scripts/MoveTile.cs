using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTile : MonoBehaviour
{
    private Material _mat;
    private float _moveOffset;

    public static bool CanMoveTile { get; set; }//캐릭터가 맵에 들어왔을때만 타일을 움직이게 함
    private void Awake()
    {
        _mat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        //todo: 캐릭터가 2층에 도착했을 때 작동시키기
        if (CanMoveTile)
        {
            _moveOffset += 0.003f;
            _mat.SetTextureOffset("_MainTex", new Vector2(_moveOffset * 0.1f, 0));
        }
    }
}
