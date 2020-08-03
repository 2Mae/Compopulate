using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Compopulate
{
    public class Session
    {
        public List<Field> fields = new List<Field>();
        public void Refresh() { Refresh(GameObject.FindObjectsOfType<Component>()); }
        public void Refresh(Component[] components)
        {
            fields.Clear();
            for (int i = components.Length - 1; i >= 0; i--)//FORR since list is reverse to hierarchy order by default.
            {
                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

                FieldInfo[] fields = components[i].GetType().GetFields(bindingFlags);
                for (int j = 0; j < fields.Length; j++)
                {
                    var fieldInfo = fields[j];

                    Att compopulateAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(Att)) as Att;

                    if (compopulateAttribute != null)
                    {
                        bool hasSerializeField = (Attribute.GetCustomAttribute(fieldInfo, typeof(SerializeField)) != null);
                        bool notHideInInspector = (Attribute.GetCustomAttribute(fieldInfo, typeof(HideInInspector)) == null);

                        if (notHideInInspector && (hasSerializeField || fieldInfo.IsPublic))
                        {
                            this.fields.Add(new Field(components[i], fieldInfo));
                        }
                        else
                        {
                            Debug.LogWarning($"Ineffectual use of {compopulateAttribute.GetType()}: {components[i].GetType()}.{fieldInfo.Name}\nOnly works with public or [SerialiseField]");
                        }
                    }
                }
            }
        }
        public void ProcessAll()
        {
            Undo.SetCurrentGroupName("Compopulate All");
            int group = Undo.GetCurrentGroup();
            for (int i = 0; i < fields.Count; i++)
            {
                if (!fields[i].processed)
                {
                    ProcessField(fields[i]);
                }
            }
            Undo.CollapseUndoOperations(group);
        }

        public void ProcessField(Field field)
        {
            if (field.processed)
            {
                Debug.LogWarning("Field already processed");
                return;
            }

            Type type = field.fieldInfo.FieldType;
            Component newValue = field.script.GetComponent(type);
            if (newValue != field.after)
            {
                Debug.LogWarning("The expected value has changed!");
            }
            else if (field.value != newValue)
            {
                Undo.RecordObject(field.script, $"Compopulate: {field.script}");
                field.value = newValue;
            }

            field.processed = true;
        }
    }
}
