namespace StudentSIS;

using StudentSIS.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        var dataService = new DataService();
        
        
        var apiThread = new Thread(() => StartAPI(dataService))
        {
            IsBackground = true
        };
        apiThread.Start();
        
        using var loginForm = new LoginForm(dataService);

        if (loginForm.ShowDialog() == DialogResult.OK)
        {
            Application.Run(new MainForm(dataService, loginForm.IsAdmin, loginForm.LoggedStudent));
        }
    }

    private static void StartAPI(DataService dataService)
    {
        try
        {
            var builder = WebApplication.CreateBuilder();

            
            builder.Services.AddSingleton(dataService);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policyBuilder =>
                {
                    policyBuilder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { message = "An unexpected server error occurred." });
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AllowAll");
            app.MapControllers();

            
            app.Run("http://localhost:5000");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"API Error: {ex.Message}", "API Startup Error");
        }
    }
}
