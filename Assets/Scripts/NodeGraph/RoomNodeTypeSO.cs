using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeSO", menuName = "Scriptable Objects/Dungeon/RoomNodeTypeSO", order = 0)]
public class RoomNodeTypeSO : ScriptableObject
{
    public string RoomNodeTypeName;
   
    [Header("")]
    public bool DisplayerInNodeGraphEditor = true;
    
    [Header("これは廊下です")]
    public bool IsCorridor;

    [Header("これは廊下です　南北方向")]
    public bool IsCorridorNS;

    [Header("これは廊下です　東西方向")]
    public bool IsCorridorEW;

    [Header("これは入り口")]
    public bool IsEntrance;
    
    [Header("これはBoss部屋")]
    public bool IsBossRoom;
    
    [Header("これはなにもない　None")]
    public bool IsNone;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtility.ValidateCheckEmptyString(this, nameof(RoomNodeTypeName), RoomNodeTypeName);
    }
#endif
}