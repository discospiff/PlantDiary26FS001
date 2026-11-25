var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<PlantDiary26FS001.Services.PlantFeedOptions>()
    .Bind(builder.Configuration.GetSection("PlantFeeds"))
    .Validate(options => options.WateringPlantsUrl is not null && options.SpecimensUrl is not null,
        "Both plant feed URLs are required.")
    .ValidateOnStart();
builder.Services.AddOptions<PlantDiary26FS001.Services.WeatherFeedOptions>()
    .Bind(builder.Configuration.GetSection("WeatherFeed"))
    .Validate(options => options.ForecastUrl is not null,
        "The weather forecast URL is required.")
    .ValidateOnStart();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<PlantDiary26FS001.Services.IPlantFeedService, PlantDiary26FS001.Services.PlantFeedService>();
builder.Services.AddHttpClient<PlantDiary26FS001.Services.IWeatherService, PlantDiary26FS001.Services.WeatherService>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
