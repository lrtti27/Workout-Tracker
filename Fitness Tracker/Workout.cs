namespace Fitness_Tracker;
using System.ComponentModel.DataAnnotations;
public class Workout
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Exercise { get; set; }
    
    public double Weight { get; set; }
    public int Reps { get; set; }
    public DateTime Date { get; set; }

    public Workout()
    {
    }
}