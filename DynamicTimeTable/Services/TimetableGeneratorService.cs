using DynamicTimeTable.Models;

namespace DynamicTimeTable.Services
{
    public class TimetableGeneratorService
    {
        /// <summary>
        /// Generates a subject timetable grid based on subject hours, rows (slots per day), and columns (working days).
        /// </summary>
        public string[,] GenerateGrid(List<SubjectHourModel> subjectHours, int rows, int cols)
        {
            int totalSlots = rows * cols;

            // Prepare a flat list of subject names repeated by their hours
            var flatList = new List<string>();
            foreach (var subject in subjectHours)
            {
                for (int i = 0; i < subject.Hours; i++)
                {
                    flatList.Add(subject.SubjectName);
                }
            }

            // Safety check: ensure total subject hours = total slots
            if (flatList.Count != totalSlots)
                throw new InvalidOperationException("Subject hours do not match total timetable slots.");

            // Shuffle subject list randomly (Fisher-Yates shuffle can also be used for better randomness)
            var random = new Random();
            flatList = flatList.OrderBy(_ => random.Next()).ToList();

            // Initialize grid
            var grid = new string[rows, cols];
            int index = 0;

            // Fill timetable grid row-wise
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    grid[row, col] = flatList[index++];
                }
            }

            return grid;
        }
    }
}
