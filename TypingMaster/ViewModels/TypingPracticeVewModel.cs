using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using TypingMaster.Models;

namespace TypingMaster.ViewModels
{
    public partial class TypingPracticeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string targetText = "Hello, Worlds!";

        [ObservableProperty]
        private ObservableCollection<Inline> formattedTextInlines;

        [ObservableProperty]
        private string userInput = "";

        [ObservableProperty]
        private int cursorPosition = 0;

        [ObservableProperty]
        private char currentChar = '\0';

        [ObservableProperty]
        private char targetChar = '\0';

        [ObservableProperty]
        private int keyPressTrigger = 0;

        [ObservableProperty]
        private int wpm = 0;

        [ObservableProperty]
        private double accuracy = 100.0;

        [ObservableProperty]
        private int errorCount = 0;

        [ObservableProperty]
        private string timerDisplay = "00:00";

        [ObservableProperty]
        private bool isSessionActive = false;

        [ObservableProperty]
        private bool isShiftPressed = false;

        [ObservableProperty]
        private bool isCapsLockActive = false;

        [ObservableProperty]
        private bool isShiftRequired = false;

        [ObservableProperty]
        private bool? isLastKeyCorrect = null;

        private DateTime sessionStartTime;
        private TypingSession currentSession;
        
        public TypingPracticeViewModel()
        {   
            currentSession = new TypingSession();
            formattedTextInlines = new ObservableCollection<Inline>();
            UpdateDisplayText();
            UpdateShiftRequirement();
        }

        public void OnKeyPressed(char keyChar)
        {

            CurrentChar = keyChar;
            KeyPressTrigger = 1 - KeyPressTrigger;

            if (!IsSessionActive)
            {
                StartSession();
            }

            if (CursorPosition < TargetText.Length)
            {
                char expectedChar = TargetText[CursorPosition];
                TargetChar = expectedChar;

                IsShiftRequired = char.IsUpper(expectedChar) || IsSymbolRequiringShift(expectedChar);

                if (keyChar == expectedChar)
                {
                    IsLastKeyCorrect = true;
                    UserInput += keyChar;
                    CursorPosition++;
                    currentSession.CorrectCharacters++;
                    UpdateDisplayText(true);

                    UpdateShiftRequirement();
                } 
                else
                {   
                    IsLastKeyCorrect = false;
                    ErrorCount++;
                    if (currentSession.MistakeKeys.ContainsKey(expectedChar))
                    {
                        currentSession.MistakeKeys[expectedChar]++;
                    }
                    else
                    {
                        currentSession.MistakeKeys[expectedChar] = 1;
                    }
                    UpdateDisplayText(false);    
                }

                currentSession.TotalCharacters++;
                UpdateStatistics();

                if (CursorPosition > TargetText.Length)
                {
                    EndSession();
                }
            }
        }

        public void OnShiftPressed()
        {
            IsShiftPressed = true;
        }

        public void OnShiftReleased()
        {
            IsShiftPressed = false;
        }

        public void OnCapsLockToggled()
        {
            IsCapsLockActive = !IsCapsLockActive;
        }
        private bool IsSymbolRequiringShift(char c)
        {
            return "!@#$%^&*()_+{}|:\"<>?~".Contains(c);
        }

        private void UpdateShiftRequirement()
        {
            if (CursorPosition < TargetText.Length)
            {
                char nextChar = TargetText[CursorPosition];
                IsShiftRequired = char.IsUpper(nextChar) || IsSymbolRequiringShift(nextChar);
            }
            else
            {
                IsShiftRequired = false;
            }
        }

        private void StartSession()
        {
            IsSessionActive = true;
            sessionStartTime = DateTime.Now;
            currentSession.StartTime = sessionStartTime;
            CursorPosition = 0;
            UserInput = "";
            ErrorCount = 0;
            UpdateDisplayText();
        }

        private void EndSession()
        {
            IsSessionActive = false;
            currentSession.EndTime = DateTime.Now;
            currentSession.TextDisplayed = TargetText;
            currentSession.UserInput = UserInput;

            currentSession.WordCount = TargetText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            currentSession.CalculateWPM();
            currentSession.CalculateAccuracy();

            // TODO: Show results popup
        }

        private void UpdateStatistics()
        {
            if (currentSession.TotalCharacters > 0)
            {
                Accuracy = (double)currentSession.CorrectCharacters / currentSession.TotalCharacters * 100;
            }

            var elapsed = (DateTime.Now - sessionStartTime).TotalMinutes;
            if (elapsed > 0)
            {
                int wordsTyped = UserInput.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                Wpm = (int)(wordsTyped / elapsed);
            }
        }

        private void UpdateDisplayText(bool isCorrect=true)
        {
            FormattedTextInlines.Clear();

            if (CursorPosition < TargetText.Length)
            {
                TargetChar = TargetText[CursorPosition];
            }

            for (int i = 0; i < TargetText.Length; i++)
            {
                var run = new Run(TargetText[i].ToString());
                if (i < CursorPosition)
                {
                    run.Foreground = Brushes.Green;
                    run.FontWeight = FontWeights.Bold;
                }
                else if (i == CursorPosition)
                {
                    if (!isCorrect)
                    {
                        run.Background = Brushes.Red;
                    }
                    else
                    {
                        run.Background = Brushes.Yellow;
                    }
                    run.Foreground = Brushes.Black;
                    run.FontWeight = FontWeights.Bold;
                }
                else
                {
                    run.Foreground = Brushes.Gray;
                }

                FormattedTextInlines.Add(run);
            }
        }
    }
}
