using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Servicios;
using WebAPIAutores.Filtros;
using WebAPIAutores.Middlewares;

namespace WebApiAutores;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // para q las claims salgan con el nombre q les paso ;P
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
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
            opciones => opciones.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(Configuration["llavejwt"])),
                ClockSkew = TimeSpan.Zero
            }); //    * p' ocupar [Authorize] en controladores

        //services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            //c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIAutores", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

        });

        services.AddAutoMapper(typeof(Startup));

        services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        // autorizacion basada en claims p' poner los admin en el token
        // el usuario va a tener que tener el claim "esAdmin"
        services.AddAuthorization(opciones =>
        {//                   nombre de la policy                      la key q va en la claim
            opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
        });


        services.AddCors(opciones =>
        {
            opciones.AddDefaultPolicy(builder =>
            {//                          url permitida
                builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader();
            });
        });

        services.AddDataProtection(); // p' lo de encriptacion

        services.AddTransient<HashService>();
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


        app.UseCors();


        app.UseAuthorization(); //       *

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

}
