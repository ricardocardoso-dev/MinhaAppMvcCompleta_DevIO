using System.Collections.Generic;
using System.Globalization;
using DevIO.App.Data;
using DevIO.Business.Interfaces;
using DevIO.Data.Context;
using DevIO.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static DevIO.App.Extensions.MoedaAttributeAdapter;


namespace DevIO.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<MeuDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));


            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            services.AddAutoMapper(typeof(Startup));
            services.AddMvc(o =>
            {
                string invalidValueMsg = "O valor preenchido é inválido para este campo.";
                string beNumericMsg = "O campo deve ser numérico.";
                string requiredValueMsg = "Este campo precisa ser preenchido.";
                string bodyRequiredMsg = "É necessário que o body na requisição não esteja vazio.";


                o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => invalidValueMsg);
                o.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => requiredValueMsg);
                o.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => requiredValueMsg);
                o.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => bodyRequiredMsg);
                o.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(x => invalidValueMsg);
                o.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => invalidValueMsg);
                o.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => beNumericMsg);
                o.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor((x) => invalidValueMsg);
                o.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => invalidValueMsg);
                o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => beNumericMsg);
                o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => requiredValueMsg);


            })
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);



            //Injeções de dependência
            services.AddScoped<MeuDbContext>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddSingleton<IValidationAttributeAdapterProvider, MoedaValidationAttributeAdapterProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            var defaultCulture = new CultureInfo("pt-BR");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };
            app.UseRequestLocalization(localizationOptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
