using System;
using System.Reflection;
using UnityEngine;

namespace Compopulate
{
    public class Field
    {
        public Component script = null;
        public FieldInfo fieldInfo = null;
        public bool processed;
        public bool confirmed => before == after;
        public Component before;
        public Component after;

        public Check preCheck;
        public Result result;

        public enum Check
        {
            AvailableValue, ConfirmedValue, ConfirmedNull, ConflictingValue, ConflictingNull, BrokenCheck
        }

        public enum Result
        {
            UnProcessed, Skipped, Override, Applied
        }



        public Field(Component script, FieldInfo fieldInfo)
        {
            this.script = script;
            this.fieldInfo = fieldInfo;

            before = value;
            after = script.GetComponent(fieldInfo.FieldType);

            preCheck = GetCheck(before, after);
        }

        public Check GetCheck(Component A, Component B)
        {
            Check check;
            if (A == null && B != null)
            {
                check = Check.AvailableValue;
            }
            else if (A == null && B == null)
            {
                check = Check.ConfirmedNull;
            }
            else if (A != null && B != null && A == B)
            {
                check = Check.ConfirmedValue;
            }
            else if (A != null && B != null && A != B)
            {
                check = Check.ConflictingValue;
            }
            else if (A != null && B == null)
            {
                check = Check.ConflictingNull;
            }
            else
            {
                check = Check.BrokenCheck;
            }

            return check;
        }

        public Component value
        {
            get { return (Component)fieldInfo.GetValue(script); }
            set { fieldInfo.SetValue(script, value); }
        }
    }
}
