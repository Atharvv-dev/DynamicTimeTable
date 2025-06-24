using System.ComponentModel.DataAnnotations;

namespace DynamicTimeTable.Models
{
    public class TimetableInputModel
    {

        [Required]
        [Range(1, 7, ErrorMessage = "Working days must be between 1 and 7")]
        public int WorkingDays { get; set; }

        [Required]
        [Range(1, 8, ErrorMessage = "Subjects per day must be between 1 and 8")]
        public int SubjectsPerDay { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Total subjects must be a positive number")]
        public int TotalSubjects { get; set; }

        // Automatically calculated property
        public int TotalHours => WorkingDays * SubjectsPerDay;
    }

    public class SubjectHourModel
    {
        public string SubjectName { get; set; }
        public int Hours { get; set; }
    }



    public class SubjectHoursViewModel
    {
        public int TotalSubjects { get; set; }
        public int TotalHours { get; set; }

        public List<SubjectHourModel> SubjectHours { get; set; }

        public SubjectHoursViewModel()
        {
            SubjectHours = new List<SubjectHourModel>();
        }
    }

    public class FinalTimetableViewModel
    {
        public string[,] TimetableGrid { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
    }
}
