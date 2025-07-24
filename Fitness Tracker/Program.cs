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

app.MapGet("/workouts", async (FitnessContext db) => await db.Workouts.ToListAsync());
app.MapPost("/workouts", async (WorkoutDto workoutDto , FitnessContext db) =>
{
    var workout = new Workout
    {
        Exercise = workoutDto.Exercise,
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
    //Get weights from the week before this one
    var lastWeek = DateTime.Now.AddDays(-14);
    var thisWeek = DateTime.Now.AddDays(-7);

    var lastWeekRecent = await db.Workouts
        .Where(w => w.Exercise == exercise && w.Date >= lastWeek && w.Date < thisWeek)
        .OrderBy(w => w.Date)
        .ToListAsync();
    
    var thisWeekRecent = await db.Workouts
        .Where(w => w.Exercise == exercise && w.Date >= thisWeek)
        .OrderBy(w => w.Date)
        .ToListAsync();

    double lastWeekAvg = lastWeekRecent.Any() ? lastWeekRecent.Average(w => w.Weight) : 0.0;
    double thisWeekAvg = thisWeekRecent.Any() ? thisWeekRecent.Average(w => w.Weight) : 0.0;
    
    return Results.Json(new { lastWeekAvg, thisWeekAvg });

});

app.Run();