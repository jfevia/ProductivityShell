using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;

namespace ProductivityShell.EditorMargin
{
    internal class TextControl : DockPanel
    {
        protected readonly TextBlock TextBlockValue;
        protected readonly Label LabelName;

        public TextControl(string name, string value = "Loading...")
        {
            LabelName = new Label();
            LabelName.Padding = new Thickness(3, 3, 0, 3);
            LabelName.FontWeight = FontWeights.Bold;
            LabelName.Content = $"{name}: ";
            LabelName.SetResourceReference(TextBlock.ForegroundProperty, EnvironmentColors.ComboBoxFocusedTextBrushKey);
            Children.Add(LabelName);

            TextBlockValue = new TextBlock();
            TextBlockValue.Padding = new Thickness(0, 3, 10, 3);
            TextBlockValue.Text = value;
            TextBlockValue.SetResourceReference(TextBlock.ForegroundProperty, EnvironmentColors.ComboBoxFocusedTextBrushKey);
            Children.Add(TextBlockValue);
        }

        public string Value
        {
            get { return TextBlockValue.Text; }
            set { TextBlockValue.Text = value; }
        }

        public void SetTooltip(string tooltip, bool preserveFormatting = false)
        {
            if (preserveFormatting)
            {
                var label = new Label();
                label.Content = tooltip;
                label.FontFamily = new FontFamily("Lucida Console");
                ToolTip = label;
            }
            else
            {
                ToolTip = tooltip;
            }
        }
    }
}