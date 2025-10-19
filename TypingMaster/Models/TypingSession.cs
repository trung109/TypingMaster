using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingMaster.Models
{
    public class TypingSession
    {
        public string SessionId { get; set; }
        public int LessonId { get; set; }
        public string Mode {  get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TextDisplayed { get; set; }
        public string UserInput { get; set; }
        public int TotalCharacters { get; set; }
        public int CorrectCharacters { get; set; }

        public int WordCount { get; set; }
        public double WPM { get; set; }
        public double Accuracy { get; set; }
        public Dictionary<char, int> MistakeKeys { get; set; }

        public TypingSession ()
        {
            SessionId = Guid.NewGuid ().ToString ();
            MistakeKeys = new Dictionary<char, int> ();
            StartTime = DateTime.Now;
        }

        public void CalculateWPM()
        {
            var duration = (EndTime - StartTime).TotalMinutes;
            if (duration > 0)
            {
                WPM = WordCount / duration;
            }
        }

        public void CalculateAccuracy()
        {
            if (TotalCharacters > 0)
            {
                Accuracy = (double) CorrectCharacters / TotalCharacters * 100;
                //Accuracy = (double) CorrectCharacters * 100;
            }
        }
    }

}
