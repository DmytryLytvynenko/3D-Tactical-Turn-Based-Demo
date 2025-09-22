using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player : Character
{
    public static Player InstancePlayer { get; set; }
    public static bool UsingSkill { get; set; }

    [Header("Death Screen")]
    [SerializeField] private GameObject UIBlockPanel;
    [SerializeField] private Transform DeathScreen;
    [SerializeField] private Volume volume;
    [SerializeField] private float dofLerpTime;
    private DepthOfField depthOfField;
    public static bool PlayerISDead = false;
    protected override void Awake()
    {
        volume.profile.TryGet(out DepthOfField _depthOfField);
        depthOfField = _depthOfField;
        UsingSkill = false;
        base.Awake();
        IsPlayer = true;
        player = this;
        InstancePlayer = this;
    }
    protected override void OnPlayerMadeStep() 
    {
        //print("Player step");
    }
    public override void OnStep(ref bool needToChangeState) 
    {
        
    }
    public override async Task Die()
    {
        PlayerISDead = true;
        UIBlockPanel.SetActive(true);
        characterTile.OnTileDeoccupiedByCharater();
        GetComponent<HealthControll>().Dead = true;
        animationController.SetBool(CharacterAnimParameters.Dead, true);
        await Task.Delay(3000);
        await Utils.FadeInDepthOfField(depthOfField, dofLerpTime);
        DeathScreen.gameObject.SetActive(true);
        await Utils.ScaleUpObject(DeathScreen, Vector3.one);
    }
    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
        if (drawDistance && drawDistanceTarget != null)
        {
            GUIStyle debugTextStyle = new GUIStyle();
            debugTextStyle.fontSize = 16;
            debugTextStyle.normal.textColor = Color.black;
            string distance1 = (transform.position - drawDistanceTarget.position).magnitude.ToSafeString();
            Vector3 pos = (transform.position + drawDistanceTarget.position) / 2;
            pos.y += 0.2f;
            Gizmos.DrawLine(transform.position, drawDistanceTarget.position);
            Handles.Label(pos, distance1, debugTextStyle);
        }
    }
}
