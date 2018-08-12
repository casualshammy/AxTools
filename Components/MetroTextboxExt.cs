using MetroFramework.Controls;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace Components
{
    public partial class MetroTextboxExt : MetroTextBox
    {
        private readonly TextBox internalTextBox;

        public MetroTextboxExt()
        {
            InitializeComponent();
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type baseType = GetType().BaseType;
            if (baseType != null)
            {
                FieldInfo field = baseType.GetField("baseTextBox", bindFlags);
                if (field != null)
                {
                    internalTextBox = (TextBox)field.GetValue(this);
                }
            }
        }

        public HorizontalAlignment TextAlign
        {
            get => internalTextBox.TextAlign;
            set => internalTextBox.TextAlign = value;
        }

        public bool ReadOnly
        {
            get => internalTextBox.ReadOnly;
            set => internalTextBox.ReadOnly = value;
        }
    }
}