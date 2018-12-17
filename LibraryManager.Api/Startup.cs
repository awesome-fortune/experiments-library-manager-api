using LibraryManager.Api.Entities;
using LibraryManager.Api.Helpers;
using LibraryManager.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace LibraryManager.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LibraryContext>(options => 
            options.UseSqlServer(
                Configuration["Data:ConnectionStrings:LibraryManagerDbConnection"]
            ));

            services.AddScoped<ILibraryRepository, LibraryRepository>();

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            services.AddTransient<ITypeHelperService, TypeHelperService>();

            // Need the following 2 services for pagination ninjarism
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory => 
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddMvc(setupAction => 
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()); // supporting application/xml content-type
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
            })
            .AddJsonOptions(options => 
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, LibraryContext libraryContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder => 
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected issue has occured. Try again later.");
                    });
                });
                app.UseHsts();
            }

            AutoMapper.Mapper.Initialize(config => {
                config.CreateMap<Entities.Author, Models.AuthorDto>()
                    .ForMember(
                        dest => dest.Name,
                        opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")
                    )
                    .ForMember(
                        dest => dest.Age,
                        opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge())
                    );

                config.CreateMap<Entities.Book, Models.BookDto>();

                config.CreateMap<Models.AuthorCreateDto, Entities.Author>();

                config.CreateMap<Models.BookCreateDto, Entities.Book>();

                config.CreateMap<Models.BookUpdateDto, Entities.Book>();

                config.CreateMap<Entities.Book, Models.BookUpdateDto>();
            });

            libraryContext.EnsureSeedDataForContext();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
