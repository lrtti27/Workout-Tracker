
using System.ComponentModel.DataAnnotations;
namespace Fitness_Tracker;

public class Exercise
{
    [Key]
    public int Id { get; set; }

    [Required] 
    public string Name { get; set; }
}