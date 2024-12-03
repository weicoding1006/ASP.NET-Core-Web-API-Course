

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
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "�ҵ{API", Version = "v1" });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "@�п�J'Bearer'[space]�M��[�J�A��token",
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
            //    options.AssumeDefaultVersionWhenUnspecified = true; // �����w�����ɨϥιw�]����
            //    options.DefaultApiVersion = new ApiVersion(1, 0);   // �w�]���� 1.0
            //    options.ReportApiVersions = true;                  // �b�^�� Header �����i�䴩������
            //    options.ApiVersionReader = ApiVersionReader.Combine(
            //        new QueryStringApiVersionReader("api-version"), // �d�߰ѼơA�Ҧp ?api-version=1.0
            //        new HeaderApiVersionReader("X-Version"),       // �۩w�q Header�A�Ҧp X-Version: 1.0
            //        new MediaTypeApiVersionReader("ver")           // �C��������������A�Ҧp application/json;ver=1.0
            //    );
            //})
            //.AddApiExplorer(options =>
            //{
            //    options.GroupNameFormat = "'v'VVV";                 // �������ծ榡�A�Ҧp v1, v2
            //    options.SubstituteApiVersionInUrl = true;          // ���� URL ��������
            //});


            builder.Host.UseSerilog((ctx,lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

            builder.Services.AddAutoMapper(typeof(MapperConfig));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICountriesRepository,CountriesRepository>();
            builder.Services.AddScoped<IHotelsRepository,HotelsRepository>();
            builder.Services.AddScoped<IAuthManager,AuthManager>();

            // �]�w JWT ����
            builder.Services.AddAuthentication(options =>
            {
                // �]�w�w�]�����Ҥ�׬� JwtBearer
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // �]�w JWT �����ҰѼ�
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ����ñ�o�K�_�O�_���ġA�T�O JWT �Ӧۥi�H�ӷ�
                    ValidateIssuerSigningKey = true,

                    // ���� JWT ��ñ�o�̡A�T�O Token ��ñ�o�̬O���\��
                    ValidateIssuer = true,

                    // ���� JWT �������A�T�O Token �������O�����ε{��
                    ValidateAudience = true,

                    // ���� JWT �����Ĵ��A�T�O Token �b���Ĵ����ϥ�
                    ValidateLifetime = true,

                    // �]�w���\�����������q�� 0�A�קK�]�ɶ��t���ɭP���ҥ���
                    ClockSkew = TimeSpan.Zero,

                    // �q�]�w��Ū�����Ī�ñ�o��
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

                    // �q�]�w��Ū�����Ī�����
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],

                    // �q�]�w��Ū���[�K�K�_�A�èϥι�٥[�K
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
            //    // �]�m Cache-Control �Y
            //    context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
            //    {
            //        Public = true,  // �O�_���\�N�z���A���w�s
            //        MaxAge = TimeSpan.FromSeconds(1)  // �w�s�����Įɶ�
            //    };
            //    // �]�m��L�����w�s�Y�]�p�G�ݭn�^
            //    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };
            //    // �I�s�U�@�Ӥ����n��
            //    await next();
            //});

            //�ϥΨ�������
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
