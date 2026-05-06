using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using StudyReminder.Models;
using StudyReminder.Models.Repositories;
using StudyReminder.Services;
using StudyReminder.Settings;

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ??builder.Configuration.GetConnectionString("SpacedRepAppConnectionString");
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SpacedRepDbContext>(options =>
options.UseNpgsql(connectionString)
);
builder.Services.AddHttpClient();


builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedAccount = true;

    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<SpacedRepDbContext>();
builder.Services.AddScoped<IStudyTopicRepository, StudyTopicRepository>();
builder.Services.AddScoped<IStudyFileRepository, StudyFileRepository>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();
builder.Services.AddScoped<EmailTemplate>();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 35MB
});
builder.Services.AddHangfire(config=>config.UsePostgreSqlStorage(c=>c.UseNpgsqlConnection(connectionString)));
builder.Services.AddHangfireServer();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<RevisionReminderService>(
    "send-revision-reminders",
    service => service.SendRevisionReminderAsync(),
   "* 16 * * *"
    );

app.Run();
