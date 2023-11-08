using Microsoft.AspNetCore.Mvc;
using SerilogTimings.Extensions;
using Serilog;

namespace Serilog_Setup.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly Serilog.ILogger _loggerTimer;
    private readonly ILogger<WeatherForecastController> _logger;

    private readonly EventId _event = new (5001, nameof(WeatherForecastController));
    
    public WeatherForecastController(Serilog.ILogger loggerTimer, ILogger<WeatherForecastController> logger)
    {
        _loggerTimer = loggerTimer.ForContext<WeatherForecastController>();
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var op = _loggerTimer.BeginOperation("Start generating weather forecast");
        
        _logger.LogInformation(_event,"Initiate weather forecast");
        
        _logger.LogWarning("Production warning example");
        try
        {
            //throw new Exception("Force an abandon!");
            await Task.Delay(2000);
        
            var returnValue =  Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
            
            _logger.LogInformation(_event,"Finalised weather forecast generation");
            
            op.Complete();
            
            return returnValue;
        
        }
        catch (Exception e)
        {
            op.Abandon();
            throw;
        }
    }
}