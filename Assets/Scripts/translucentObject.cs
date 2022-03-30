using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class translucentObject : MonoBehaviour
{
   public struct St_ObstacleRendererInfo
    {
        public int InstanceId;
        public MeshRenderer Mesh_Renderer;
        public Material OrinMaterial;
    }

    private Dictionary<int, St_ObstacleRendererInfo> Dic_SaveObjectInfo = new Dictionary<int, St_ObstacleRendererInfo>();
    private List<St_ObstacleRendererInfo> Lst_TransparentedRenderer = new List<St_ObstacleRendererInfo>();
    private RaycastHit[] HitObject;


    public GameObject _player;
    public GameObject _camera;

    public Material[] origin_Material = new Material[5];
    public Material[] translucent_Material = new Material[5];


    public void Update()
    {
        Camera_TransparentProcess_Operation();
    }

    void Camera_TransparentProcess_Operation()
    {
        if (_player == null) return;

        if(Lst_TransparentedRenderer.Count > 0)
        {
            for(int i=0;i< Lst_TransparentedRenderer.Count; ++i) {
                Lst_TransparentedRenderer[i].Mesh_Renderer.material = Lst_TransparentedRenderer[i].OrinMaterial;
            }

            Lst_TransparentedRenderer.Clear();
        }
        Vector3 DirToCam = (_camera.transform.position - _player.transform.position).normalized;
        float Distance = (_camera.transform.position - _player.transform.position).magnitude;
      
        HitRayTransparentObject(_player.transform.position, DirToCam, Distance);

    }


    void HitRayTransparentObject(Vector3 start, Vector3 direction, float distance)
    {
        HitObject = Physics.RaycastAll(start, direction, distance);

        

        for(int i = 0; i < HitObject.Length; ++i)
        {
            Debug.LogFormat("{0}", HitObject[i].collider.name);
            int instanceid = HitObject[i].collider.GetInstanceID();
            if (!Dic_SaveObjectInfo.ContainsKey(instanceid))
            {
                MeshRenderer obsRenderer = HitObject[i].collider.gameObject.GetComponent<MeshRenderer>();
                St_ObstacleRendererInfo rendererInfo = new St_ObstacleRendererInfo();
                rendererInfo.InstanceId = instanceid;
                rendererInfo.Mesh_Renderer = obsRenderer;
                rendererInfo.OrinMaterial = obsRenderer.material;


                Dic_SaveObjectInfo[instanceid] = rendererInfo;
            }

            for(int j = 0; j < origin_Material.Length; ++j)
            {
                if(Dic_SaveObjectInfo[instanceid].Mesh_Renderer.material == origin_Material[j])
                {
                    Dic_SaveObjectInfo[instanceid].Mesh_Renderer.material = translucent_Material[j];
                }
            }
            Lst_TransparentedRenderer.Add(Dic_SaveObjectInfo[instanceid]);
        }


    }

}
