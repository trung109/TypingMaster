using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TypingMaster.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    public partial class VirtualKeyboard : UserControl
    {
        private DispatcherTimer highlightTimer;

        public static readonly DependencyProperty CurrentCharProperty =
            DependencyProperty.Register(
                "CurrentChar",
                typeof(char),
                typeof(VirtualKeyboard),
                new PropertyMetadata('\0'));

        public static readonly DependencyProperty IsShiftPressedProperty =
            DependencyProperty.Register(
                "IsShiftPressed",
                typeof(bool),
                typeof(VirtualKeyboard),
                new PropertyMetadata(false, OnIsShiftPressedChanged));

        public static readonly DependencyProperty IsCapsLockActiveProperty =
            DependencyProperty.Register(
                "IsCapsLockActive",
                typeof(bool),
                typeof(VirtualKeyboard),
                new PropertyMetadata(false, OnIsCapsLockActiveChanged));

        public static readonly DependencyProperty IsShiftRequiredProperty =
            DependencyProperty.Register(
                "IsShiftRequired",
                typeof(bool),
                typeof(VirtualKeyboard),
                new PropertyMetadata(false, OnIsShiftRequiredChanged));

        public static readonly DependencyProperty IsLastKeyCorrectProperty =
            DependencyProperty.Register(
                "IsLastKeyCorrect",
                typeof(bool?),
                typeof(VirtualKeyboard),
                new PropertyMetadata(null, OnIsLastKeyCorrectChanged));

        public static readonly DependencyProperty KeyPressTriggerProperty =
            DependencyProperty.Register(
                "KeyPressTrigger",
                typeof(int),
                typeof(VirtualKeyboard),
                new PropertyMetadata(0, OnKeyPressTriggerChanged));

        private Border currentlyHighlightedBorder = null;
        private char lastHighlightedChar = '\0';

        public char CurrentChar
        {
            get { return (char)GetValue(CurrentCharProperty); }
            set { SetValue(CurrentCharProperty, value); }
        }

        public bool IsShiftPressed
        {
            get { return (bool)GetValue(IsShiftPressedProperty); }
            set { SetValue(IsShiftPressedProperty, value); }
        }

        public bool IsCapsLockActive
        {
            get { return (bool)GetValue(IsCapsLockActiveProperty); }
            set { SetValue(IsCapsLockActiveProperty, value); }
        }

        public bool IsShiftRequired
        {
            get { return (bool)GetValue(IsShiftRequiredProperty); }
            set { SetValue(IsShiftRequiredProperty, value); }
        }

        public bool? IsLastKeyCorrect
        {
            get { return (bool?)GetValue(IsLastKeyCorrectProperty); }
            set { SetValue(IsLastKeyCorrectProperty, value); }
        }

        public int KeyPressTrigger
        {
            get { return (int)GetValue(KeyPressTriggerProperty); }
            set { SetValue(KeyPressTriggerProperty, value); }
        }

        public VirtualKeyboard()
        {
            InitializeComponent();

            highlightTimer = new DispatcherTimer();
            highlightTimer.Interval = TimeSpan.FromMilliseconds(200);
            highlightTimer.Tick += HighlightTimer_Tick;
        }

        private void HighlightTimer_Tick(object sender, EventArgs e)
        {
            highlightTimer.Stop();
            ResetAllKeys();
        }

        private static void OnCurrentCharChanged (DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is VirtualKeyboard keyboard)
            {
                keyboard.HighlightKey((char)e.NewValue);
            }
        }

        private static void OnIsShiftPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VirtualKeyboard keyboard)
            {
                keyboard.UpdateShiftHighlight((bool)e.NewValue);
            }
        }

        private static void OnIsCapsLockActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VirtualKeyboard keyboard)
            {
                keyboard.UpdateCapsLockHighlight((bool)e.NewValue);
            }
        }

        private static void OnIsShiftRequiredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VirtualKeyboard keyboard)
            {
                bool isRequired = (bool)e.NewValue;

                if (isRequired && !keyboard.IsShiftPressed)
                {
                    keyboard.HighlightModifierKey("Shift", new SolidColorBrush(Color.FromRgb(245, 222, 165)), Brushes.Gray);
                }
                else if (!isRequired && !keyboard.IsShiftPressed)
                {
                    keyboard.HighlightModifierKey("Shift", null, null);
                }
            }
        }

        private static void OnIsLastKeyCorrectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VirtualKeyboard keyboard && e.NewValue != null)
            {
                keyboard.UpdateKeyHighlightColor();
            }
        }

        private static void OnKeyPressTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Trigger highlight whenever this changes
            if (d is VirtualKeyboard keyboard)
            {
                keyboard.HighlightKey(keyboard.CurrentChar);
            }
        }

        private void UpdateShiftHighlight(bool isPressed)
        {
            if (isPressed)
            {
                HighlightModifierKey("Shift", Brushes.Orange, Brushes.DarkOrange);
            }
            else
            {
                if (IsShiftRequired)
                {
                    HighlightModifierKey("Shift", new SolidColorBrush(Color.FromRgb(245, 222, 165)), Brushes.Gray);
                }
                else
                {
                    HighlightModifierKey("Shift", null, null);
                }
            }
        }

        private void UpdateCapsLockHighlight(bool isActive)
        {
            HighlightModifierKey("Caps Lock", isActive ? Brushes.Orange : null, Brushes.DarkOrange);
        }

        private void HighlightKey(char currentChar)
        {

            highlightTimer.Stop();

            bool isSameKey = (char.ToUpper(currentChar) == char.ToUpper(lastHighlightedChar));


            if (!isSameKey)
            {
                ResetAllKeys();
                currentlyHighlightedBorder = null;
            }

            if (currentChar == '\0') return;

            lastHighlightedChar = currentChar;

            if (isSameKey)
            {
                System.Diagnostics.Debug.WriteLine("Same key - updating color");
                UpdateKeyHighlightColor();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Different key - finding and highlighting");
                char upperChar = char.ToUpper(currentChar);
                string charStr = upperChar.ToString();
                HighlightKeyRecursive(this, charStr, currentChar);
            }

            System.Diagnostics.Debug.WriteLine($"HighlightKey called: char='{currentChar}', IsLastKeyCorrect={IsLastKeyCorrect}");


            highlightTimer.Start();
        }

        private void UpdateKeyHighlightColor()
        {
            if (currentlyHighlightedBorder == null) return;

            if (IsLastKeyCorrect == true)
            {
                currentlyHighlightedBorder.BorderBrush = Brushes.Green;
                currentlyHighlightedBorder.Background = Brushes.LightGreen;
                currentlyHighlightedBorder.BorderThickness = new Thickness(4);
            }
            else if (IsLastKeyCorrect == false)
            {
                currentlyHighlightedBorder.BorderBrush = Brushes.Red;
                currentlyHighlightedBorder.Background = Brushes.LightCoral;
                currentlyHighlightedBorder.BorderThickness = new Thickness(4);
            }
            else
            {
                currentlyHighlightedBorder.BorderBrush = Brushes.Orange;
                currentlyHighlightedBorder.Background = Brushes.LightYellow;
                currentlyHighlightedBorder.BorderThickness = new Thickness(4);
            }
        }

        private void HighlightKeyRecursive(DependencyObject parent, string charStr, char originalChar)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);


            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Border border)
                {
                    var textBlocks = FindAllTextBlocks(border);

                    bool isMatch = false;

                    foreach (var textBlock in textBlocks)
                    {
                        string text = textBlock.Text;

                        // Check if any TextBlock in this border matches
                        if ((text.Length == 1 && char.ToUpper(text[0]) == char.ToUpper(originalChar)) ||
                            (char.IsWhiteSpace(originalChar) && text == "SPACE"))
                        {
                            isMatch = true;
                            break;
                        }
                    }

                    if (isMatch)
                    {
                        currentlyHighlightedBorder = border;
                        UpdateKeyHighlightColor();
                        return;
                    }

                }

                HighlightKeyRecursive(child, charStr, originalChar);
            }
        }

        private void HighlightModifierKey(string keyText, Brush highlightColor, Brush borderHighlightColor)
        {
            HighlightModifierKeyRecursive(this, keyText, highlightColor, borderHighlightColor);
        }

        private void HighlightModifierKeyRecursive(DependencyObject parent, string keyText, Brush highlightColor, Brush borderHighlightColor)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Border border)
                {
                    var textBlock = FindAnyTextBlock(border);
                    if (textBlock != null && textBlock.Text == keyText)
                    {
                        if (highlightColor != null)
                        {
                            border.Background = highlightColor;
                            border.BorderBrush = borderHighlightColor ?? Brushes.Gray;
                            border.BorderThickness = new Thickness(2);
                        }
                        else
                        {
                            // Reset to default
                            border.Background = new SolidColorBrush(Color.FromRgb(0xD0, 0xD0, 0xD0));
                            border.BorderBrush = Brushes.Gray;
                            border.BorderThickness = new Thickness(2);
                        }
                    }
                }

                HighlightModifierKeyRecursive(child, keyText, highlightColor, borderHighlightColor);
            }
        }

        private TextBlock FindTextBlock(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount (parent);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild (parent, i);

                if (child is TextBlock textBlock)
                {
                    if (textBlock.FontSize == 16 || textBlock.FontWeight == FontWeights.Bold)
                    {
                        return textBlock;
                    }
                }

                var result = FindTextBlock(child);
                if (result != null) return result;
            }

            return null;
        }

        private TextBlock FindAnyTextBlock(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is TextBlock textBlock)
                {
                    return textBlock;
                }

                var result = FindAnyTextBlock(child);
                if (result != null) return result;
            }

            return null;
        }

        private List<TextBlock> FindAllTextBlocks(DependencyObject parent)
        {
            var textBlocks = new List<TextBlock>();
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is TextBlock textBlock)
                {
                    textBlocks.Add(textBlock);
                }
                else if (child is Grid || child is StackPanel)
                {
                    // Only go ONE level deeper for Grid/StackPanel containers
                    int grandChildCount = VisualTreeHelper.GetChildrenCount(child);
                    for (int j = 0; j < grandChildCount; j++)
                    {
                        var grandChild = VisualTreeHelper.GetChild(child, j);
                        if (grandChild is TextBlock tb)
                        {
                            textBlocks.Add(tb);
                        }
                    }
                }
            }

            return textBlocks;
        }

        private void ResetAllKeys()
        {
            ResetKeysRecursive(this);
        }

        private void ResetKeysRecursive(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Border border)
                {
                    bool isKeyboardKey = border.Tag != null ||
                                        border.Width == 320;

                    var textBlock = FindAnyTextBlock(border);
                    bool isModifierKey = textBlock != null &&
                                        (textBlock.Text == "Shift" ||
                                         textBlock.Text == "Caps Lock");

                    if (isKeyboardKey && !isModifierKey)
                    {
                        border.BorderBrush = Brushes.Gray;
                        border.BorderThickness = new Thickness(2);
                        border.Effect = null;

                        string tag = border.Tag?.ToString();
                        if (tag != null)
                        {
                            border.Background = GetFingerColor(tag);
                        }
                        else if (border.Width == 320)
                        {
                            border.Background = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
                        }
                    }
                }

                ResetKeysRecursive(child);
            }
        }

        private Brush GetFingerColor(string tag)
        {
            return tag switch
            {
                "LeftPinky" => new SolidColorBrush(Color.FromRgb(0xFF, 0xB3, 0xBA)),
                "LeftRing" => new SolidColorBrush(Color.FromRgb(0xBA, 0xE1, 0xFF)),
                "LeftMiddle" => new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xBA)),
                "LeftIndex" => new SolidColorBrush(Color.FromRgb(0xBA, 0xFF, 0xC9)),
                "RightIndex" => new SolidColorBrush(Color.FromRgb(0xBA, 0xFF, 0xC9)),
                "RightMiddle" => new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xBA)),
                "RightRing" => new SolidColorBrush(Color.FromRgb(0xBA, 0xE1, 0xFF)),
                "RightPinky" => new SolidColorBrush(Color.FromRgb(0xFF, 0xB3, 0xBA)),
                _ => Brushes.LightGray
            };
        }
    }
}
