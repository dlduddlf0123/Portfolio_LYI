using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class mapWindow : EditorWindow {

    /*string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;*/
    
    [MenuItem ("Window/mapWindow")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(mapWindow));
    }
    private void OnGUI()
    {
        // GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        //myString = EditorGUILayout.TextField("Text Field", myString);

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle",);
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup();

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Rotate",groupEnabled);

        //회전 버튼
        GUILayout.Label("Rotate", EditorStyles.boldLabel);
        if (GUI.Button(new Rect(5, 30, 50, 20), "◀")) { CMap.Instance.LeftRotate(); }
        if (GUI.Button(new Rect(80, 30, 50, 20), "▶")) { CMap.Instance.RightRotate(); }
        //EditorGUILayout.EndToggleGroup();

        //그리드 활성/비활성화 버튼
        if (GUI.Button(new Rect(Screen.width-90, 5, 80, 20), "Grid")) {  CMap.Instance.ToggleGrid(); }

        //큐브 상하 이동 버튼
        GUI.Label(new Rect(5, 60, 100, 30), "GridFloor",EditorStyles.boldLabel);
        //if (GUI.Button(new Rect(5, 80, 50, 20), "▲")) { CMap.Instance.FloorUp(); }
        //if (GUI.Button(new Rect(80, 80, 50, 20), "▼")) { CMap.Instance.FloorDown(); }

        if (GUI.Button(new Rect(5, 80, 50, 20), "▲")) { CMap.Instance.GridUp(); }
        if (GUI.Button(new Rect(80, 80, 50, 20), "▼")) { CMap.Instance.GridDown(); }

        GUI.Label(new Rect(150, 80, 50, 20), CMap.Instance.GridFloor().ToString(), EditorStyles.boldLabel);

    }
}
