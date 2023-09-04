using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebAPIAutores.Filtros;
using WebAPIAutores.Middlewares;

namespace WebApiAutores;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    //////////////////////////////////////////
    /////////////////////////////////////////////
    public void ConfigureServices(IServiceCollection services)
    {
        // AddJsonOptions p' evitar el error del cyclo al llamar entidades que se hacen referencia
        // ( x las q creo los DTO's )
        services.AddControllers(opciones =>
        {
            opciones.Filters.Add(typeof(FiltroDeExcepcion));
        }).AddJsonOptions(x =>
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
        .AddNewtonsoftJson();

        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

        // instalar Microsoft.AspNetCore.Authentication.JwtBearer
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(); //           * p' ocupar [Authorize] en controladores

        //services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(Startup));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        // p' guardar todas las respuestas http
        // app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
        app.UseLoguearRespuestaHTTP();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization(); //           *

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

}
