using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TypingMaster.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AllowedKeys { get; set; }
        public int Order { get; set; }
        public string Difficulty { get; set; }

        public Dictionary<char, FingerAssignment> KeyFingerMap { get; set; }

        public Lesson() {
            KeyFingerMap = new Dictionary<char, FingerAssignment>();
        }
    }
}

public class FingerAssignment
{
    public string Hand { get; set; }
    public string Finger { get; set; }
    public string Color { get; set; }
}


