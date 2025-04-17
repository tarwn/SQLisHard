using System.Net;
using System.Net.Mail;
using CommandLine;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SQLisHard.Attributes;
using SQLisHard.Configuration;
using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Domain.ExerciseEvaluator;
using SQLisHard.Domain.Exercises;
using SQLisHard.Domain.Exercises.ExerciseStore;
using SQLisHard.Domain.QueryEngine;
using SQLisHard.Domain.QueryEngine.DatabaseExecution;
using SQLisHard.General.ErrorLogging;
using SQLisHard.General.ExperienceLogging.Log;

var builder = WebApplication.CreateBuilder(args);

var coreConnectionString = builder.Configuration.GetConnectionString("Core")!;
var exercisesConnectionString = builder.Configuration.GetConnectionString("Exercises")!;
builder.Services.Configure<EmailErrorSettings>(builder.Configuration.GetSection("EmailErrorSettings"));

builder.Services.AddScoped<IQueryEngine>(s => new QueryEngine(exercisesConnectionString));
// TODO: singleton instead of scoped so we're not adding unnecessary overhead to every request
builder.Services.AddScoped<IExerciseStore>(s =>
{
    var queryEngine = s.GetService<IQueryEngine>()!;
    var env = s.GetService<IWebHostEnvironment>()!;
    var store = new FlatFileExerciseStore(queryEngine);
    foreach (var file in Directory.EnumerateFiles(Path.Combine(env.ContentRootPath, "Exercises")))
    {
        store.Add(File.ReadAllText(file));
    }
    /*
    #if DEBUG
                _fw = new FileSystemWatcher(Server.MapPath("Exercises"),"*.txt");
                _fw.EnableRaisingEvents = true;
                _fw.Changed += ExerciseChanged;
     #endif
     */
    return store;
});
builder.Services.AddScoped<IHistoryStore>(s => new HistoryStore(coreConnectionString));
builder.Services.AddScoped<IUserStore>(s => new UserStore(coreConnectionString));
builder.Services.AddScoped<ISessionStore>(s => new SessionStore(coreConnectionString));
builder.Services.AddScoped<CoreMembership>();
builder.Services.AddScoped<IExerciseResultEvaluator, ExerciseResultEvaluator>();
builder.Services.AddScoped<IExperienceLogProvider, NullLogProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "sih-cookie";
})
    .AddCookie("sih-cookie")
    .AddScheme<AutomatedGuestOptions, AutomatedGuestHandler>("sih-guest", options =>
    {
        options.SignInScheme = "sih-cookie";
    });
builder.Services.AddAuthorization();

// Note: Not crazy about thes services being singlestones below, I grabbed FluentEmail as a quick fix and not sure
//  if it executes the builder once or on demand (SmtpClient is not threadsafe)
builder.Services.AddExceptionHandler<EmailExceptionHandler>();
builder.Services.AddSingleton<IErrorReporter, EmailErrorReporter>();
if (builder.Configuration["Email:Method"] == "File")
{
    builder.Services.AddFluentEmail(builder.Configuration["Email:FromAddress"], builder.Configuration["Email:FromName"])
        .AddSmtpSender(new SmtpClient
        {
            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
            PickupDirectoryLocation = Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["Email:FilePath"]!)
        });
}
else
{
    builder.Services.AddFluentEmail(builder.Configuration["Email:FromAddress"], builder.Configuration["Email:FromName"])
        .AddSmtpSender(new SmtpClient(builder.Configuration["Email:Host"], int.Parse(builder.Configuration["Email:Port"]!))
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(builder.Configuration["Email:Username"], builder.Configuration["Email:Password"])
        });
}

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LogUserInteractionAttribute>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var core = app.Configuration["Migrations:Core:ConnectionString"];
    var exercises = app.Configuration["Migrations:Exercises:ConnectionString"];
    if (core == null || exercises == null)
    {
        throw new Exception("Connection Strings must be set for sql migrations to run during development using admin-level rights");
    }
    LocalDevelopmentTasks.MigrateDatabase(core, Path.Combine(app.Environment.ContentRootPath, "../../../database/coredb/migrations"));
    LocalDevelopmentTasks.MigrateDatabase(exercises, Path.Combine(app.Environment.ContentRootPath, "../../../database/exercisedb/migrations"));
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStatusCodePages();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
