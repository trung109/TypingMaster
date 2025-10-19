using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingMaster.Models
{
    public class UserProgress
    {
        public string UserId { get; set; }
        public int TotalSessions { get; set; }
        public TimeSpan TotalTimePracticed { get; set; }
        public double AverageWPM { get; set; }
        public double BestWPM { get; set; }
        public double AverageAccuracy { get; set; }
        public Dictionary<int, LessonProgress> LessonProgressData { get; set; }
        public Dictionary<char, int> WeakKeys { get; set; }
        public DateTime LastPraciceDate { get; set; }
        public List<SessionHistory> RecentSessions { get; set; }

        public UserProgress ()
        {
            UserId = Guid.NewGuid().ToString();
            LessonProgressData = new Dictionary<int, LessonProgress>();
            WeakKeys = new Dictionary<char, int>();
            RecentSessions = new List<SessionHistory> ();
        }

    }
}
public class LessonProgress
{
    public int LessonId { get; set; }
    public int TimeCompleted { get; set; }
    public double BestWPM { get; set; }
    public double BestAccuracy { get; set; }
    public int Stars { get; set; }
    public bool IsUnlocked { get; set; }
}

public class SessionHistory
{
    public DateTime Date { get; set; }
    public double WPM { get; set; }
    public double Accuracy { get; set; }
}
