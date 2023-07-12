using DemoFintechApp.Data.Context;
using DemoFintechApp.Data.Implementations;
using DemoFintechApp.Data.Interfaces;
using DemoFintechApp.Logging;
using DemoFintechApp.Models.Identity;
using DemoFintechApp.Services.Implementations;
using DemoFintechApp.Services.Interfaces;
using DemoFintechApp.Services.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DemoFintechApp.API.Extensions
{
	public static class ServiceExtensions
	{
		public static void RegisterServices(this IServiceCollection services)
		{
			services.AddTransient<IUnitOfWork, UnitOfWork<FintechDbContext>>();
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<ITransactionService, TransactionService>();

		}

		public static void ConfigureSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(s =>
			{
				s.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "DemoFintechApp API",
					Version = "v1",
					Description = " DemoFintechApp API by NimiCoder",
					TermsOfService = new Uri("https://example.com/terms"),
					Contact = new OpenApiContact
					{
						Name = "David Ukpoju",
						Email = "ukpojuojdave12@gmail.com",
						Url = new Uri("https://twitter.com/johndoe"),
					},
					License = new OpenApiLicense
					{
						Name = "DemoFintechApp API LICX",
						Url = new Uri("https://example.com/license"),
					}
				});
				s.SwaggerDoc("v2", new OpenApiInfo
				{
					Title = "DemoFintechApp API",
					Version = "v2"
				});

				s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Place to add JWT with Bearer",
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});

				s.AddSecurityRequirement(new OpenApiSecurityRequirement()
				 {
					 {
						 new OpenApiSecurityScheme
						 {
							 Reference = new OpenApiReference
							 {
								 Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Name = "Bearer",
						 },
						 new List<string>()
					 }
				 });




			});
		}
		public static void ConfigureLoggerService(this IServiceCollection services) =>
		 services.AddSingleton<ILoggerService, LoggerService>();


		//Allows all requests from all origins to be sent to our API
		public static void ConfigureCors(this IServiceCollection services) =>
			 services.AddCors(options =>
			 {
				 options.AddPolicy("CorsPolicy", builder =>
				 builder.AllowAnyOrigin()
				 .AllowAnyMethod()
				 .AllowAnyHeader());
			 });

		public static void ConfigureAuthentication(this IServiceCollection services, JwtConfig jwtConfig)
		{
			var builder = services.AddIdentity<ApplicationUser, IdentityRole>(o =>
			{
				o.Password.RequireDigit = true;
				o.Password.RequireLowercase = false;
				o.Password.RequireUppercase = false;
				o.Password.RequireNonAlphanumeric = false;
				o.Password.RequiredLength = 10;
				o.User.RequireUniqueEmail = true;
			})
			.AddEntityFrameworkStores<FintechDbContext>()
			.AddDefaultTokenProviders();

			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.JwtKey));
				options.TokenValidationParameters = new TokenValidationParameters
				{
					IssuerSigningKey = serverSecret,
					ValidIssuer = jwtConfig.JwtIssuer,
					ValidAudience = jwtConfig.JwtAudience,
					ClockSkew = TimeSpan.Zero,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
				};
			});

			services.AddAuthorization();
		}
		public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
		   services.AddDbContext<FintechDbContext>(opts =>
		   opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

	}

	
}
