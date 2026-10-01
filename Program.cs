// Create the application builder and load configuration and services.
var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers and Razor views.
builder.Services.AddControllersWithViews();

// Register IMemoryCache for caching the doctor list.
builder.Services.AddMemoryCache();

// Register response caching services.
builder.Services.AddResponseCaching();

// Register session services.
builder.Services.AddSession(options =>
{
    // Set the session to expire after 30 minutes of inactivity.
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Prevent JavaScript from accessing the session cookie.
    options.Cookie.HttpOnly = true;

    // Mark the session cookie as essential.
    options.Cookie.IsEssential = true;
});

// Register IHttpClientFactory for asynchronous API requests.
builder.Services.AddHttpClient();

// Build the application using all configured services.
var app = builder.Build();

// Check whether the application is not running in development mode.
if (!app.Environment.IsDevelopment())
{
    // Redirect unhandled exceptions to the error page.
    app.UseExceptionHandler("/Home/Error");

    // Enable HTTP Strict Transport Security.
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Enable CSS, JavaScript and other static files.
app.UseStaticFiles();

// Enable ASP.NET Core routing.
app.UseRouting();

// Enable response caching middleware.
app.UseResponseCaching();

// Enable session state management.
app.UseSession();

// Enable authorization middleware.
app.UseAuthorization();

// Configure the default MVC route.
// The application starts with DoctorController's Index action.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Doctor}/{action=Index}/{id?}");

// Start running the web application.
app.Run();
