using UnityEditor;
using UnityEngine;

namespace Compopulate
{
    public class OnPlay
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            if (!EditorWindow.GetWindow<CompopulateWindow>().interuptOnPlay) { return; }

            Session session = new Session();
            session.Refresh();
            for (int i = 0; i < session.fields.Count; i++)
            {
                if (!session.fields[i].confirmed)
                {
                    EditorApplication.ExitPlaymode();
                    EditorApplication.playModeStateChanged += Changed;
                    break;
                }
            }
        }

        private static void Changed(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.playModeStateChanged -= Changed;
                CompopulateWindow.ShowWindow();
            }
        }
    }
}