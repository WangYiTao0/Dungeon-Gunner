using System;
using UnityEditor;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle _roomNodeStyle;
    
    //node layout value
    private const float _nodeWidth = 160;
    private const float _nodeHeight = 75;
    private const int _nodePadding = 25;
    private const int _nodeBorder = 12;

    //エディターメニューの作成　pathを指定
    [MenuItem("Room Node Graph Editor", menuItem = "Dungeon/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        //エディターウィンドウの作成
         GetWindow<RoomNodeGraphEditor>("Room NodeGraph Editor");
    }

    private void OnEnable()
    {
        //Define node Layout style レイアウト スタイルの定義
        _roomNodeStyle = new GUIStyle();
        _roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        _roomNodeStyle.normal.textColor = Color.white;
        _roomNodeStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);
        _roomNodeStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);
    }

    /// <summary>
    /// エディターを描画する
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(new Vector2(100f,100f),new Vector2(_nodeWidth,_nodeHeight)),_roomNodeStyle);
        EditorGUILayout.LabelField("Node 1");
        GUILayout.EndArea();
        
        GUILayout.BeginArea(new Rect(new Vector2(300f,300f),new Vector2(_nodeWidth,_nodeHeight)),_roomNodeStyle);
        EditorGUILayout.LabelField("Node 1");
        GUILayout.EndArea();
    }
}
