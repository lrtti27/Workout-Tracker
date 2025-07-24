
namespace Fitness_Tracker;
using Microsoft.EntityFrameworkCore;
public class FitnessContext : DbContext
{
    public FitnessContext(DbContextOptions<FitnessContext> options) : base(options) {}

    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
}