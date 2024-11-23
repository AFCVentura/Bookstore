using Bookstore.Data;
using Bookstore.Services;
using Bookstore.Services.Seeding;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Bookstore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<GenreService>();
            builder.Services.AddScoped<BookService>();
            builder.Services.AddScoped<SeedingService>();
            builder.Services.AddScoped<SaleService>();
            builder.Services.AddScoped<SellerService>();

            builder.Services.AddDbContext<BookstoreContext>(options =>
            {
                // Configura o serviço DbContext que será usado para a interação com o banco de dados.
                // O DbContext é uma classe que representa uma sessão com o banco de dados e permite a 
                // execução de consultas e operações de CRUD.

                options.UseMySql(
                    // Aqui, estamos dizendo ao DbContext para usar o MySQL como o provedor de banco de dados.
                    builder
                        .Configuration
                        // O método `Configuration` acessa as configurações da aplicação.
                        .GetConnectionString("BookstoreContext"),
                    // Recupera a string de conexão com o banco de dados a partir do arquivo de configuração.
                    // Neste caso, a string é identificada pelo nome "BookstoreContext".

                    ServerVersion
                        .AutoDetect(
                            // O método `AutoDetect` automaticamente detecta a versão do servidor MySQL
                            // para garantir que a aplicação se conecte corretamente à versão compatível.
                            builder
                                .Configuration
                                // Novamente, acessa a configuração da aplicação.
                                .GetConnectionString("BookstoreContext")
                        // Obtém novamente a mesma string de conexão, que será usada para detectar a versão do MySQL.
                        )
                );
            });

            var app = builder.Build();

            var ptBR = new CultureInfo("pt-BR");

            var localizationOption = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ptBR),
                SupportedCultures = new List<CultureInfo> { ptBR },
                SupportedUICultures = new List<CultureInfo> { ptBR }
            };

            app.UseRequestLocalization(localizationOption);


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.Services.CreateScope().ServiceProvider.GetRequiredService<SeedingService>().Seed();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Esse é o mapeamento de rotas do projeto, todas as rotas terão o formato:
            // /controller/action e talvez um /id no final.
            // Sendo que o controller padrão é o Home e a action padrão é a Index.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
