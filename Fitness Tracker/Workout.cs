namespace Fitness_Tracker;
using System.ComponentModel.DataAnnotations;
public class Workout
{
    [Key]
    public int Id { get; set; }
    
    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; }

    public double Weight { get; set; }
    public int Reps { get; set; }
    public DateTime Date { get; set; }
    
}