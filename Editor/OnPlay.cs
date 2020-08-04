using UnityEditor;
using UnityEngine;

namespace Compopulate
{
    public class OnPlay
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            SettingsObject settings = SettingsHandler.Load();

            if (!settings.checkBeforePlay)
            {
                return;
            }

            Session session = new Session();
            session.Refresh();
            for (int i = 0; i < session.fields.Count; i++)
            {
                //Todo: Make this not an abomination.
                Field field = session.fields[i];
                bool confirmed = (field.preCheck == Field.Check.ConfirmedValue ||
                ((field.allowNull || !settings.warnIfNull) && field.preCheck == Field.Check.ConfirmedNull));
                if (!confirmed)
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