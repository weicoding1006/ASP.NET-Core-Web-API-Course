

using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using ClassLibrary1;
using ClassLibrary1.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using WebApplication1.Configurations;
using WebApplication1.Contracts;
using WebApplication1.Middleware;
using WebApplication1.Repository;


namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );

            builder.Services.AddIdentityCore<ApiUser>()
                .AddRoles<IdentityRole>()
                .AddTokenProvider<DataProtectorTokenProvider<ApiUser>>("CourseApi")
                .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // Add services to the container.

  
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "課程API", Version = "v1" });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "@請輸入'Bearer'[space]然後加入你的token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = "Oauth2",
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
            });

            //builder.Services.AddApiVersioning(options =>
            //{
            //    options.AssumeDefaultVersionWhenUnspecified = true; // 當未指定版本時使用預設版本
            //    options.DefaultApiVersion = new ApiVersion(1, 0);   // 預設版本 1.0
            //    options.ReportApiVersions = true;                  // 在回應 Header 中報告支援的版本
            //    options.ApiVersionReader = ApiVersionReader.Combine(
            //        new QueryStringApiVersionReader("api-version"), // 查詢參數，例如 ?api-version=1.0
            //        new HeaderApiVersionReader("X-Version"),       // 自定義 Header，例如 X-Version: 1.0
            //        new MediaTypeApiVersionReader("ver")           // 媒體類型版本控制，例如 application/json;ver=1.0
            //    );
            //})
            //.AddApiExplorer(options =>
            //{
            //    options.GroupNameFormat = "'v'VVV";                 // 版本分組格式，例如 v1, v2
            //    options.SubstituteApiVersionInUrl = true;          // 替換 URL 中的版本
            //});


            builder.Host.UseSerilog((ctx,lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

            builder.Services.AddAutoMapper(typeof(MapperConfig));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICountriesRepository,CountriesRepository>();
            builder.Services.AddScoped<IHotelsRepository,HotelsRepository>();
            builder.Services.AddScoped<IAuthManager,AuthManager>();

            // 設定 JWT 驗證
            builder.Services.AddAuthentication(options =>
            {
                // 設定預設的驗證方案為 JwtBearer
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // 設定 JWT 的驗證參數
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // 驗證簽發密鑰是否有效，確保 JWT 來自可信來源
                    ValidateIssuerSigningKey = true,

                    // 驗證 JWT 的簽發者，確保 Token 的簽發者是允許的
                    ValidateIssuer = true,

                    // 驗證 JWT 的受眾，確保 Token 的受眾是本應用程式
                    ValidateAudience = true,

                    // 驗證 JWT 的有效期，確保 Token 在有效期內使用
                    ValidateLifetime = true,

                    // 設定允許的時鐘偏移量為 0，避免因時間差異導致驗證失敗
                    ClockSkew = TimeSpan.Zero,

                    // 從設定中讀取有效的簽發者
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

                    // 從設定中讀取有效的受眾
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],

                    // 從設定中讀取加密密鑰，並使用對稱加密
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                };
            });

            builder.Services.AddControllers().AddOData(options =>
            {
                options.Select().Filter().OrderBy();
            });

            //builder.Services.AddResponseCaching(options =>
            //{
            //    options.MaximumBodySize = 1024;
            //    options.UseCaseSensitivePaths = true;
            //});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            //app.UseResponseCaching();
            //app.Use(async (context, next) =>
            //{
            //    // 設置 Cache-Control 頭
            //    context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            //    {
            //        Public = true,  // 是否允許代理伺服器緩存
            //        MaxAge = TimeSpan.FromSeconds(1)  // 緩存的有效時間
            //    };
            //    // 設置其他相關緩存頭（如果需要）
            //    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };
            //    // 呼叫下一個中介軟體
            //    await next();
            //});

            //使用身分驗證
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
