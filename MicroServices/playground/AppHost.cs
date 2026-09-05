var builder = DistributedApplication.CreateBuilder(args);

// `aspire run --project MicroServices/playground -- --exercise ex001`
var id = builder.Configuration["exercise"];

if (string.IsNullOrWhiteSpace(id))
{
    Console.Error.WriteLine(
        "Pass an exercise, e.g.: aspire run --project MicroServices/playground -- --exercise ex001");
    Console.Error.WriteLine("Known: " + string.Join(", ", ExerciseRegistry.Known));
    return;
}

var configure = ExerciseRegistry.Lookup(id)
    ?? throw new InvalidOperationException(
        $"Unknown exercise '{id}'. Known: {string.Join(", ", ExerciseRegistry.Known)}");

configure(builder);

builder.Build().Run();
