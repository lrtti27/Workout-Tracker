using Fitness_Tracker;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FitnessContext>(options => options.UseSqlite("Data Source=fitness.db"));
var app = builder.Build();
app.UseStaticFiles();



app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.MapGet("/workouts", async (FitnessContext db) =>
{
    var workouts = await db.Workouts
        .Include(w => w.Exercise) // 👈 This is the missing piece
        .OrderByDescending(w => w.Date)
        .ToListAsync();

    return Results.Ok(workouts);
});
app.MapGet("/exercises", async (FitnessContext db) => await db.Exercises.ToListAsync());
app.MapPost("/workouts", async (WorkoutDto workoutDto, FitnessContext db) =>
{
    if (string.IsNullOrWhiteSpace(workoutDto.Exercise))
        return Results.BadRequest("Exercise name is required.");

    // Using EF.Functions.Like for case-insensitive match
    var exercise = await db.Exercises
        .FirstOrDefaultAsync(e => EF.Functions.Like(e.Name, workoutDto.Exercise));

    if (exercise == null)
    {
        exercise = new Exercise { Name = workoutDto.Exercise };
        db.Exercises.Add(exercise);
        await db.SaveChangesAsync();
    }

    var workout = new Workout
    {
        ExerciseId = exercise.Id,
        Weight = workoutDto.Weight,
        Reps = workoutDto.Reps,
        Date = DateTime.Now,
    };

    db.Workouts.Add(workout);
    await db.SaveChangesAsync();

    return Results.Created($"/workouts/{workout.Id}", workout);
});

app.MapDelete("/workouts/{id}", async (int id, FitnessContext db) =>
{
    var workout = await db.Workouts.FindAsync(id);
    if (workout is null) return Results.NotFound();

    db.Workouts.Remove(workout);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/insights/{exercise}", async (string exercise, FitnessContext db) =>
{
    
    //Get the exercise id based on the name of the exercise
    var exerciseEntity = await db.Exercises.FirstOrDefaultAsync(x => x.Name == exercise);
    if (exerciseEntity == null)
    {
        return Results.BadRequest("Could not find exercise with that name");
    }
    
    //Now, we can safely use the exercise ID corresponding to the name
    int exerciseId = exerciseEntity.Id;
    
    //Get weights from the week before this one
    var lastWeek = DateTime.Now.AddDays(-14);
    var thisWeek = DateTime.Now.AddDays(-7);

    var lastWeekRecent = await db.Workouts
        .Where(w => w.ExerciseId == exerciseId && w.Date >= lastWeek && w.Date < thisWeek)
        .OrderBy(w => w.Date)
        .ToListAsync();
    
    var thisWeekRecent = await db.Workouts
        .Where(w => w.ExerciseId == exerciseId && w.Date >= thisWeek)
        .OrderBy(w => w.Date)
        .ToListAsync();

    double lastWeekAvg = lastWeekRecent.Any() ? lastWeekRecent.Average(w => w.Weight) : 0.0;
    double thisWeekAvg = thisWeekRecent.Any() ? thisWeekRecent.Average(w => w.Weight) : 0.0;
    
    return Results.Json(new { lastWeekAvg, thisWeekAvg });

});




app.Run();