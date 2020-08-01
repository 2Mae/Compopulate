using UnityEditor;
using UnityEngine;

namespace Compopulate
{
    public static class Icons
    {
        public static Texture check => EditorGUIUtility.IconContent("CollabDeleted Icon").image;
        public static Texture empty => EditorGUIUtility.IconContent("CollabChangesDeleted Icon").image;
        public static Texture emptyWarn => EditorGUIUtility.IconContent("CollabChangesConflict Icon").image;

        public static Texture conflictVal => EditorGUIUtility.IconContent("CollabConflict").image;
        public static Texture pencil => EditorGUIUtility.IconContent("CollabEdit Icon").image;

        public static Texture conflictNull => EditorGUIUtility.IconContent("CollabError").image;
        public static Texture conflictNullFixed => EditorGUIUtility.IconContent("CollabChanges Icon").image;
        public static Texture dropdownTriangle => EditorGUIUtility.IconContent("icon dropdown").image;
        public static Texture blank => EditorGUIUtility.IconContent("d_RectTransformBlueprint").image;
        public static Texture cog => EditorGUIUtility.IconContent("Settings").image;
    }
}