using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tests
{
    public class TestComponent : MonoBehaviour
    {
        [Compopulate.Att] public MeshFilter testField;

        public static void CreateSomeTestObjects()
        {

            Undo.SetCurrentGroupName("Create Compopulate Test Objects");
            int group = Undo.GetCurrentGroup();
            
            var parent = new GameObject("CompopulateTest");
            Undo.RegisterCreatedObjectUndo(parent, parent.name);
            List<TestComponent> children = new List<TestComponent>();
            for (int i = 0; i < 5; i++)
            {
                var go = new GameObject($"OBJ{i}");
                go.transform.parent = parent.transform;

                TestComponent test = go.AddComponent<TestComponent>();
                children.Add(test);
                Undo.RegisterCreatedObjectUndo(go, go.name);
            }

            {
                // 0: A=0,B=0
                // no changes
                // 1: A=0,B=1
                children[1].gameObject.AddComponent<MeshFilter>();
                // 2: A=1,B=1
                children[2].testField = children[2].gameObject.AddComponent<MeshFilter>();
                // 3: A=1,B=0
                children[3].testField = children[2].testField;
                // 4: A=?,B=1
                children[4].testField = children[2].testField;
                children[4].gameObject.AddComponent<MeshFilter>();
            }

            Undo.CollapseUndoOperations(group);

            EditorGUIUtility.PingObject(children[0]);//Ping to unfold in hierarchy
        }
    }
}