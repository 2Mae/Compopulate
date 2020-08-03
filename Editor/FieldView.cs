using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Compopulate
{
    class FieldView : VisualElement
    {
        CompopulateWindow window;
        bool warnIfNull => window.warnIfNull;
        public Field field;
        //----
        public Label itemNumber;
        public Image icon1;
        public Image icon2;
        public Label text;
        public Label postCheckLabel;
        //----
        //Todo: Move these to Field class?
        string fieldName => field.fieldInfo.Name;
        string objectName => field.script.gameObject.name;
        string fieldType => FromLastDot(field.fieldInfo.FieldType.ToString());
        string scriptType => FromLastDot(field.script.GetType().ToString());

        public FieldView(CompopulateWindow window)
        {
            this.window = window;
        }

        public string FromLastDot(string input)
        {
            int i = input.LastIndexOf('.');
            return input.Substring(i + 1);
        }

        public void Bind(CompopulateWindow window, List<Field> items, int i)
        {
            this.window = window;
            field = items[i];
            itemNumber.text = i.ToString();
            text.text = $"{field.script.gameObject.scene.name}:{objectName}:{scriptType}.{fieldName}({fieldType}) = {field.preCheck}";
            icon1.image = GetImageFromCheck(field.preCheck);

            if (field.processed)
            {

                Field.Check postCheck = field.GetCheck(field.value, field.after);
                icon2.image = GetImageFromCheck(postCheck);
                icon1.style.opacity = 0.4f;
                text.style.opacity = this.postCheckLabel.style.opacity = 0.6f;
                if (postCheck != field.preCheck)
                {
                    postCheckLabel.text = $" -> {postCheck}";
                }
                else
                {
                    postCheckLabel.text = " (no change)";
                }
            }
        }

        private Texture GetImageFromCheck(Field.Check p)
        {
            Texture image = Icons.blank;
            switch (p)
            {
                case Field.Check.ConflictingNull:
                    image = Icons.conflictNull;
                    break;
                case Field.Check.ConflictingValue:
                    image = Icons.conflictVal;
                    break;
                case Field.Check.ConfirmedValue:
                    image = Icons.check;
                    break;
                case Field.Check.ConfirmedNull:
                    if (warnIfNull)
                    {
                        image = Icons.emptyWarn;
                    }
                    else
                    {
                        image = Icons.empty;
                    }
                    break;
                case Field.Check.AvailableValue:
                    image = Icons.pencil;
                    break;
            }
            return image;
        }

        public FieldView()
        {
            Add(itemNumber = new Label("#"));
            itemNumber.style.width = 20;
            Add(icon1 = CreateImage(Icons.blank));
            Add(icon2 = CreateImage(Icons.blank));


            Add(text = new Label("[TEXT]"));
            Add(postCheckLabel = new Label(""));

            style.flexDirection = FlexDirection.Row;
        }

        Image CreateImage(Texture texture)
        {
            Image image = new Image();
            image.image = Icons.blank;
            image.scaleMode = ScaleMode.ScaleToFit;
            image.style.width = 16;
            return image;
        }
    }
}