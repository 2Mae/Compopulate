using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compopulate
{
    public class CompopulateWindow : EditorWindow
    {
        ListView listView;

        Label label;

        public Session session;
        IMGUIContainer imgui;

        public bool warnIfNull = false;
        public bool interuptOnPlay = true;

        [MenuItem("Window/Compopulate %g")]
        public static void ShowWindow() { GetWindow<CompopulateWindow>().OnShow(); }
        public void OnShow()
        {
            RefreshSession();
        }

        private void OnEnable()
        {
            session = new Session(); //Debug.Log("Created new session");
            titleContent = new GUIContent("Compopulate");
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
            rootVisualElement.Add(label = new Label());
            label.style.unityTextAlign = TextAnchor.UpperRight;
            InitiateListView();
            RefreshSession();
            Undo.undoRedoPerformed += OnUndoRedo;

            rootVisualElement.RegisterCallback<MouseUpEvent>(HandleRightClick);
        }

        private void HandleRightClick(MouseUpEvent evt)
        {
            if (evt.button != (int)MouseButton.RightMouse)
                return;

            var targetElement = evt.target as VisualElement;
            if (targetElement == null)
                return;

            DropDown(targetElement);
        }

        private void DropDown(VisualElement targetElement)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Refresh (R)"), false, RefreshSession);
            menu.AddItem(new GUIContent("Process all (Ctrl+Space)"), false, ProcessAll);
            menu.AddItem(new GUIContent("Process selected (Space)"), false, ProcessSelected);
            menu.AddItem(new GUIContent("Show selected in hierarchy (P)"), false, PingSelected);
            menu.AddItem(new GUIContent($"Warn if null"), warnIfNull, () =>
            {
                warnIfNull = !warnIfNull;
                listView.Refresh();
            });
            menu.AddItem(new GUIContent($"Interupt play"), interuptOnPlay, () => { interuptOnPlay = !interuptOnPlay; });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Create some test objects"), false, CreateSomeTestObjects);


            // Get position of menu on top of target element.
            var menuPosition = new Vector2(targetElement.layout.xMin, targetElement.layout.yMin);
            //menuPosition = Vector2.zero;
            var menuRect = new Rect(menuPosition, Vector2.zero);

            menu.DropDown(menuRect);
        }

        public void CreateSomeTestObjects()
        {
            Tests.TestComponent.CreateSomeTestObjects();
            RefreshSession();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            //Todo: Fix undo redo behaviour.
            RefreshSession();
        }

        private void OnFocus()
        {
            listView.Focus();
        }
        private void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode == KeyCode.R)
            {
                RefreshSession();
            }

            if (e.keyCode == KeyCode.M)
            {
                e.PreventDefault();
                DropDown(rootVisualElement);
            }

            if (e.keyCode == KeyCode.Space)
            {
                if (e.actionKey)
                {
                    ProcessAll();
                }
                else
                {
                    ProcessSelected();
                }
            }

            if (e.keyCode == KeyCode.P || e.keyCode == KeyCode.RightArrow)
            {
                PingSelected();
            }
        }

        private void PingSelected()
        {
            EditorGUIUtility.PingObject(GetSelectedField().script);
            Selection.activeGameObject = GetSelectedField().script.gameObject;
        }

        private void ProcessSelected()
        {
            if (GetSelectedField().processed) { return; }

            session.ProcessField(GetSelectedField());
            listView.Refresh();
            if (listView.selectedIndex + 1 < listView.childCount) { listView.selectedIndex++; }
        }

        private void ProcessAll()
        {
            session.ProcessAll();
            listView.Refresh();
            listView.selectedIndex = 0;
        }

        private Field GetSelectedField()
        {
            return ((Field)listView.selectedItem);
        }

        private void RefreshSession()
        {
            session.Refresh();
            if (session.fields.Count > 0)
            {
                listView.selectedIndex = 0;
            }
            listView.Refresh();
            if (session.fields.Count == 0)
            {
                label.text = "No fields found. Right click or (M) for menu";
            }
            else
            {
                label.text = "Right click or (M) for menu";
            }
        }
        public void InitiateListView()
        {
            const int itemHeight = 20;
            Func<VisualElement> makeItem = () => new FieldView();
            Action<VisualElement, int> bindItem = (e, i) => (e as FieldView).Bind(this, session.fields, i);
            listView = new ListView(session.fields, itemHeight, makeItem, bindItem);
            listView.style.flexGrow = 1.0f;
            rootVisualElement.Add(listView);
        }
    }
}