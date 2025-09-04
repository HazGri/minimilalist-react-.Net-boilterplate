using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);


// Configuration des services (équivalent à tes imports et middleware Express)
builder.Services.AddControllers(); // Équivalent à app.use(express.json())

// Configuration de la base de données Oracle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injection de dépendances (équivalent à tes modules Node.js)
builder.Services.AddScoped<IUserService, UserService>();

// Swagger pour la documentation API (comme Postman mais auto-généré)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS pour mon front
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configuration du pipeline HTTP (équivalent aux middleware Express)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Interface graphique pour tester l'API
}

app.UseHttpsRedirection();
app.UseCors("ReactApp"); // Active CORS pour React

app.UseAuthorization();

app.MapControllers(); // Équivalent à app.use('/api', routes)

// Création automatique de la base de données si elle n'existe pas
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated(); // Comme mongoose.connect() avec auto-creation
}

app.Run(); // Équivalent à app.listen(port)