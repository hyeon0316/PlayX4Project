using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class translucentObject : MonoBehaviour
{

    /// <summary>
    /// 반투명화된 오브젝트를 저장할 구조체
    /// </summary>
   public struct St_ObstacleRendererInfo
    {
        public int InstanceId;
        public MeshRenderer Mesh_Renderer;
        public Material OrinMaterial;
       
    }

    /// <summary>
    /// 반투명화할 오브젝트를 저장하는 변수, 다중 저장을 방지하고록 오브젝트id 를 key 로 사용
    /// </summary>
    private Dictionary<int, St_ObstacleRendererInfo> Dic_SaveObjectInfo = new Dictionary<int, St_ObstacleRendererInfo>();
    /// <summary>
    /// 반투명화한 오브젝트를 저장했다가 불투명화 할때 사용하는 list
    /// </summary>
    private List<St_ObstacleRendererInfo> Lst_TransparentedRenderer = new List<St_ObstacleRendererInfo>();

    private List<RaycastHit> raycastHits = new List<RaycastHit>(); 

    private Dictionary<int, Color> Dic_SaveColor = new Dictionary<int, Color>();
    /// <summary>
    /// raycastall 를 사용했을때 저장할 변수
    /// </summary>
    private RaycastHit[] HitObject;

    private int hitCount;
    /// <summary>
    /// 변하는 색상값
    /// </summary>
    [SerializeField]
    private Color ChangeColor = new Color(1f,1f,1f,1f);

    /// <summary>
    /// 플레이어 , 카메라 위치를 받기위해 변수 선언
    /// </summary>
    public GameObject _player;
    public GameObject _camera;

    //미리 저장해둬서 변환할 마테리얼
    public Material[] origin_Material = new Material[5];
    public Material[] translucent_Material = new Material[5];




    public void Update()
    {
        Camera_TransparentProcess_Operation();
    }

    void Camera_TransparentProcess_Operation()
    {
        //플레이어가 없다면 실행 하지 않는다.
        if (_player == null) return;
        //반투명화 한것이 1개 이상이 있어야만 반투명화를 실행
        if(Lst_TransparentedRenderer.Count > 0)
        {

            for (int j = 0; j < Lst_TransparentedRenderer.Count; ++j)
            {
                int index = Lst_TransparentedRenderer[j].InstanceId;

                if(Dic_SaveColor[index].a >= 1f)
                {
                    Lst_TransparentedRenderer[j].Mesh_Renderer.material = Lst_TransparentedRenderer[j].OrinMaterial;

                    Lst_TransparentedRenderer.RemoveAt(j);
                }
                else
                {
                    MaterialPropertyBlock material = new MaterialPropertyBlock();
                    material.SetTexture("Albedo", Dic_SaveObjectInfo[index].OrinMaterial.mainTexture);
                    Dic_SaveColor[index] = Change_Color(Dic_SaveColor[index], false);
                    material.SetColor("_Color", Dic_SaveColor[index]);
                    Lst_TransparentedRenderer[j].Mesh_Renderer.SetPropertyBlock(material);
                }

            }
        }
        raycastHits.Clear();
        //카메라에서부터 플레이어의 방향과 거리를 구한다.

        SpriteRenderer renderer = _player.transform.GetChild(0).GetComponent<SpriteRenderer>();
        float width = (renderer.sprite.rect.width / renderer.sprite.pixelsPerUnit ) * _player.transform.localScale.x;
      //  float height = (renderer.sprite.rect.height / renderer.sprite.pixelsPerUnit ) * _player.transform.GetChild(0).transform.localScale.y;
        Vector3 PlayerRight = _player.transform.position + Vector3.right * (width * 0.5f);
        Vector3 PlayerLeft = _player.transform.position + Vector3.left * (width * 0.5f);
      //  Vector3 PlayerUp = _player.transform.position + Vector3.up * (height * 0.5f);
       // Vector3 PlayerDown = _player.transform.position + Vector3.down *( height * 0.5f);

        Vector3 DirToCam = (_camera.transform.position - _player.transform.position).normalized;
        float Distance = (_camera.transform.position - _player.transform.position).magnitude;
        //카메라와 플레이어 사이의 오브젝트 반투명화
        HitRayTransparentObject2(_player.transform.position, DirToCam, Distance);
        HitRayTransparentObject2(PlayerRight, DirToCam, Distance);
       HitRayTransparentObject2(PlayerLeft, DirToCam, Distance);
      //  HitRayTransparentObject2(PlayerUp, DirToCam, Distance);
       // HitRayTransparentObject2(PlayerDown, DirToCam, Distance);

    }

    /// <summary>
    /// 카메라와 플레이어 사이의 오브젝트 반투명화
    /// </summary>
    /// <param name="start">플레이어 위치, ray 시작지점</param>
    /// <param name="direction">방향</param>
    /// <param name="distance">거리</param>
    void HitRayTransparentObject(Vector3 start, Vector3 direction, float distance)
    {
        //플레이어와 카메라 사이의 오브젝트들을 변수에 저장
        HitObject = Physics.RaycastAll(start, direction, distance,LayerMask.GetMask("InteractionObj", "Wall", "Floor"));
        
        //오브젝트가 1개 이상 있다면 실행
        if(HitObject.Length  >= 1)
        {

            for (int i = 0; i < HitObject.Length; ++i)
            {
                //맞은 오브젝트의 id 값을 가져와서 저장 및 수정
                Debug.LogFormat("{0}", HitObject[i].collider.name);
                int instanceid = HitObject[i].collider.GetInstanceID();
                //Dic_SaveObjectInfo에 없다면 만들어 준다.
                if (!Dic_SaveObjectInfo.ContainsKey(instanceid))
                {
                    MeshRenderer obsRenderer = HitObject[i].collider.gameObject.GetComponent<MeshRenderer>();
                    St_ObstacleRendererInfo rendererInfo = new St_ObstacleRendererInfo();
                    rendererInfo.InstanceId = instanceid;
                    rendererInfo.Mesh_Renderer = obsRenderer;
                    rendererInfo.OrinMaterial = obsRenderer.material;
                   
                    Dic_SaveObjectInfo[instanceid] = rendererInfo;
                }

                int j;
                for (j = 0; j < origin_Material.Length; ++j)
                {//마테리얼 이름이 같은것을 찾아서 마테리얼을 반투명 전용 마테리얼로 변환한다.
                    if (Dic_SaveObjectInfo[instanceid].Mesh_Renderer.material.name.Contains(origin_Material[j].name))
                    {
                        Dic_SaveObjectInfo[instanceid].Mesh_Renderer.material = translucent_Material[j];

                    }

                }


                //적용 컬러를 점점 반투명화
                ChangeColor =  Change_Color(ChangeColor, true);
                //반투명화된 컬러를 적용
                Dic_SaveObjectInfo[instanceid].Mesh_Renderer.material.color = ChangeColor;

                //반투명화 리스트에 추가
                Lst_TransparentedRenderer.Add(Dic_SaveObjectInfo[instanceid]);
            }

        }
        else
        {
            //맞은 오브젝트가 1개 이하라면 changeColor 알파값을 올린다.
            ChangeColor = Change_Color(ChangeColor, false);
        }
    }

    void HitRayTransparentObject2(Vector3 start, Vector3 direction, float distance)
    {
        //플레이어와 카메라 사이의 오브젝트들을 변수에 저장
        HitObject = Physics.RaycastAll(start, direction, distance, LayerMask.GetMask("InteractionObj", "Wall", "Floor"));

        //오브젝트가 1개 이상 있다면 실행
        if (HitObject.Length >= 1)
        {
            
            for (int i = 0; i < HitObject.Length; ++i)
            {
                //맞은 오브젝트의 id 값을 가져와서 저장 및 수정
                Debug.LogFormat("{0}", HitObject[i].collider.name);
                int instanceid = HitObject[i].collider.GetInstanceID();
                //Dic_SaveObjectInfo에 없다면 만들어 준다.
                if (!Dic_SaveObjectInfo.ContainsKey(instanceid))
                {
                    MeshRenderer obsRenderer = HitObject[i].collider.gameObject.GetComponent<MeshRenderer>();
                    St_ObstacleRendererInfo rendererInfo = new St_ObstacleRendererInfo();
                    rendererInfo.InstanceId = instanceid;
                    rendererInfo.Mesh_Renderer = obsRenderer;
                    rendererInfo.OrinMaterial = obsRenderer.material;
                    Dic_SaveColor.Add(instanceid, Color.white);

                     Dic_SaveObjectInfo[instanceid] = rendererInfo;
                }

                if (!raycastHits.Contains(HitObject[i]))
                    raycastHits.Add(HitObject[i]);

                MaterialPropertyBlock material = new MaterialPropertyBlock();

                Color beforeColor = Dic_SaveObjectInfo[instanceid].Mesh_Renderer.material.color;

                int j;
                for (j = 0; j < origin_Material.Length; ++j)
                {//마테리얼 이름이 같은것을 찾아서 마테리얼을 반투명 전용 마테리얼로 변환한다.
                    if (Dic_SaveObjectInfo[instanceid].Mesh_Renderer.material.name.Contains(origin_Material[j].name) &&
                        !Dic_SaveObjectInfo[instanceid].Mesh_Renderer.material.name.Contains("Hide"))
                    {
                        Debug.Log("Change");
                        Dic_SaveObjectInfo[instanceid].Mesh_Renderer.material = translucent_Material[j];

                    }

                }

                Debug.LogFormat("color : {0}", beforeColor);

                material.SetTexture("Albedo", Dic_SaveObjectInfo[instanceid].OrinMaterial.mainTexture);
                Dic_SaveColor[instanceid] = Change_Color(Dic_SaveColor[instanceid], true);
                material.SetColor("_Color", Dic_SaveColor[instanceid]);
                
                Dic_SaveObjectInfo[instanceid].Mesh_Renderer.SetPropertyBlock(material);
                
                //반투명화 리스트에 추가
                if(!Lst_TransparentedRenderer.Contains(Dic_SaveObjectInfo[instanceid]))
                Lst_TransparentedRenderer.Add(Dic_SaveObjectInfo[instanceid]);
            }

        }
        
    }


    /// <summary>
    /// color 알파값을 변경하는 함수
    /// </summary>
    /// <param name="inputColor">변경전 color 값</param>
    /// <param name="down"> ture 일경우 알파값 감소, false 일경우 증가</param>
    /// <returns>변경된 color 값</returns>
    public Color Change_Color(Color inputColor , bool down = true)
    {

        Color color = inputColor;

        if (down)
        {
            if(color.a - 0.1f > 0.3f)
            {
                color = new Color(1f, 1f, 1f, color.a - 0.1f);
            }
        }
        else
        {
            if(color.a + 0.05f < 1.1f)
            {
                color = new Color(1f, 1f, 1f, color.a + 0.05f);
            }
        }

        return color;
    }


}
