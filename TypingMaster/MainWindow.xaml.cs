using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TypingMaster.ViewModels;

namespace TypingMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TypingPracticeViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new TypingPracticeViewModel();
            this.DataContext = viewModel;


            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.PreviewKeyUp += MainWindow_PreviewKeyUp;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                viewModel.OnShiftPressed();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.CapsLock)
            {
                viewModel.OnCapsLockToggled();
                e.Handled = true;
                return;
            }

            char? keyChar = GetCharFromKey(e.Key);
            if (keyChar.HasValue)
            {
                viewModel.OnKeyPressed(keyChar.Value);
                e.Handled = true;
            }
        }

        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                viewModel.OnShiftReleased();
                e.Handled = true;
            }
        }

        private char? GetCharFromKey (Key key)
        {
            bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            if (key >= Key.A && key <= Key.Z)
            {
                char letter = key.ToString()[0];
                return isShiftPressed ? letter : char.ToLower(letter);
            }

            switch (key)
            {
                case Key.D0: return isShiftPressed ? ')' : '0';
                case Key.D1: return isShiftPressed ? '!' : '1';
                case Key.D2: return isShiftPressed ? '@' : '2';
                case Key.D3: return isShiftPressed ? '#' : '3';
                case Key.D4: return isShiftPressed ? '$' : '4';
                case Key.D5: return isShiftPressed ? '%' : '5';
                case Key.D6: return isShiftPressed ? '^' : '6';
                case Key.D7: return isShiftPressed ? '&' : '7';
                case Key.D8: return isShiftPressed ? '*' : '8';
                case Key.D9: return isShiftPressed ? '(' : '9';

                case Key.Space: return ' ';
                case Key.OemMinus: return isShiftPressed ? '_' : '-';
                case Key.OemPlus: return isShiftPressed ? '+' : '=';
                case Key.OemOpenBrackets: return isShiftPressed ? '{' : '[';
                case Key.OemCloseBrackets: return isShiftPressed ? '}' : ']';
                case Key.OemPipe: return isShiftPressed ? '|' : '\\';
                case Key.OemSemicolon: return isShiftPressed ? ':' : ';';
                case Key.OemQuotes: return isShiftPressed ? '"' : '\'';
                case Key.OemComma: return isShiftPressed ? '<' : ',';
                case Key.OemPeriod: return isShiftPressed ? '>' : '.';
                case Key.OemQuestion: return isShiftPressed ? '?' : '/';
                case Key.OemTilde: return isShiftPressed ? '~' : '`';

                default: return null;
            }

        }
    }
}