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
        public ListView listView;

        public Session session;
        IMGUIContainer imgui;

        public bool warnIfNull = false;
        public bool interuptOnPlay = true;

        public VisualElement toolbar;

        public VisualElement menu;

        [MenuItem("Window/Compopulate %g")]
        public static void ShowWindow() { GetWindow<CompopulateWindow>().OnShow(); }
        public void OnShow()
        {
            RefreshSession();
        }

        void InitialiseToolbar()
        {
            toolbar = new VisualElement();
            toolbar.style.flexDirection = FlexDirection.Row;
            toolbar.style.unityTextAlign = TextAnchor.MiddleCenter;
            toolbar.style.borderBottomColor = Color.gray;
            toolbar.style.borderBottomWidth = 1;
            toolbar.style.backgroundColor = new Color(1, 1, 1, 0.2f);
            toolbar.style.height = 20;
            toolbar.Add(menu = new ToolbarElement(() => DropDown(), "Menu"));
            toolbar.RegisterCallback<FocusEvent>(ToolbarFocus);

            rootVisualElement.Add(toolbar);
        }

        private void ToolbarFocus(FocusEvent evt)
        {
            Debug.Log("fdsaf");
        }

        public class ToolbarElement : VisualElement
        {
            Action action;
            public ToolbarElement(Action action, string labelText = "", Texture icon = null)
            {
                this.action = action;
                style.flexDirection = FlexDirection.Row;
                style.borderRightColor = Color.gray;
                style.borderRightWidth = 1;

                if (labelText != "")
                {
                    Label l = new Label(labelText);
                    l.style.paddingLeft = l.style.paddingRight = 3;
                    Add(l);
                }

                if (icon != null)
                {
                    Image image = new Image() { image = icon };
                    Add(image);
                }

                RegisterCallback<MouseDownEvent>(Click);
                RegisterCallback<MouseEnterEvent>(Enter);
                RegisterCallback<MouseLeaveEvent>(Leave);
            }

            private void Click(MouseDownEvent evt)
            {
                action.Invoke();
            }

            private void Leave(MouseLeaveEvent evt)
            {
                style.backgroundColor = new Color(0, 0, 0, 0);
            }

            private void Enter(MouseEnterEvent evt)
            {
                style.backgroundColor = new Color(0, 0, 0, 0.1f);
            }
        }

        private void OnEnable()
        {

            session = new Session(); //Debug.Log("Created new session");
            titleContent = new GUIContent("Compopulate");

            InitialiseToolbar();
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
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
        }

        private void DropDown()
        {
            var genericMenu = new GenericMenu();

            genericMenu.AddItem(new GUIContent("Refresh (R)"), false, RefreshSession);
            genericMenu.AddItem(new GUIContent("Process all (Ctrl+Space)"), false, ProcessAll);
            genericMenu.AddItem(new GUIContent("Process selected (Space)"), false, ProcessSelected);
            genericMenu.AddItem(new GUIContent("Show selected in hierarchy (P)"), false, PingSelected);
            genericMenu.AddItem(new GUIContent($"Warn if null"), warnIfNull, () =>
            {
                warnIfNull = !warnIfNull;
                listView.Refresh();
            });
            genericMenu.AddItem(new GUIContent($"Interupt play"), interuptOnPlay, () => { interuptOnPlay = !interuptOnPlay; });
            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("Create some test objects"), false, CreateSomeTestObjects);

            var menuPosition = new Vector2(menu.layout.xMin, menu.layout.yMax);
            var menuRect = new Rect(menuPosition, menu.layout.size);

            genericMenu.DropDown(menuRect);
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
                DropDown();
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
            if (GetSelectedField() == null || GetSelectedField().processed) { return; }

            session.ProcessField(GetSelectedField());
            listView.Refresh();
            if (listView.selectedIndex + 1 < listView.childCount) { listView.selectedIndex++; }
        }

        private void ProcessAll()
        {
            if (session.fields.Count != 0)
            {
                session.ProcessAll();
                listView.Refresh();
                listView.selectedIndex = 0;
            }

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