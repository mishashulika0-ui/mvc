using Microsoft.AspNetCore.Builder;
using p48_lessons_asp.data.dto;
using p48_lessons_asp.Services.Operation;
using p48_lessons_asp.Services; // Додано для LoggerService та ShoppingCartService

// Файл старту, тут запускається сервер

// як запустити ASP проєкт
// # запуск проєкту 
// dotnet run 
namespace p48_lessons_asp
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Включаємо сервіс, який дозволяє класам(контролерам у які будуть приходити запити) повертати html-верстку
            builder.Services.AddControllersWithViews();

            #region Services Register
            // builder.Services.AddTransient<TransientOperation>();
            // builder.Services.AddScoped<ScopedOperation>();
            // builder.Services.AddSingleton<SingletonOperation>();
            
            /*
            Може виникнути ситуація що нам знадобляться декілька реалізацій сервісу, щоб обрати конкретну ми додамо ключ, за яки будемо діставати реалізацію під інтерфейс
            */
            builder.Services.AddKeyedTransient<IOperation, TransientOperation>("transient");
            builder.Services.AddKeyedScoped<IOperation, ScopedOperation>("scoped");
            builder.Services.AddKeyedSingleton<IOperation, SingletonOperation>("singleton");

            builder.Services.AddSingleton<LoggerService>();
            builder.Services.AddScoped<ShoppingCartService>();
            #endregion

            var app = builder.Build();

            // Якщо проєкт знаходиться не у режимі розробки то наступні інструкції виконаються
            if (!app.Environment.IsDevelopment())
            {   

                // мідлвеєр який перехоплює будь-яку помилку, і у бразуер віддає html-сторінку з описом помилку
                app.UseExceptionHandler("/Home/Error");
                // мідлвеєр який забороняє заходити на цей ресурс без https
                app.UseHsts();
            }
            
            // Для роботи картинок, стилів, скриптів, необхідно підключити цей мідлвеєр
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<LoggerService>();
                logger.Add($"{context.Request.Method} {context.Request.Path}");
                await next();
            });

            app.MapGet("/api/logs", (LoggerService logger) => logger.Logs);

            app.MapControllerRoute(
                name:"default",
                pattern:"{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
            
        }
        public static void MainOld(string[] args)
        {
            // Main запускає сервер(бекенд)
            /* Для створення додатку, необхідно створити білдер
            Білдер займається налаштуванням додатку:
            - конфігурація
            - логування
            - контейнери DI(Dependency Injection)
            - підключення бд
            - створення власних серівсів

            на основі білдера створюється додаток
            */

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            #region Lesson1
            /*
            У додатку налаштовується
            - рутінг
            - мідлвеєри
            - обробка запитів
            */


            // MapGet - (route)ендпоінт який отримує GET запити
            // отримує два параметри: перший- назва еднопінту
            // другий - лямбда-хендлер

            /*
            На кожен HTTP Method є свій мап
            MapGet - GET
            MapPost - POST
            MapPut - PUT
            MapPatch - PATCH
            */
            // Мідлвеєр який дозволяє шукати файли у проєкті
            app.UseDefaultFiles();

            // Мідлвеєр який включає роздачу статичних файлів сервером.
            app.UseStaticFiles();

            /* Коли ми підключаємо роздачу статичних файлів
            і в нас є індекс.html необхідність в ендопінті / відпадає,
            замість нашої логіки почне роздаватися html
            */
            // app.MapGet("/", () => "Hello World!");

            app.MapGet("/greet/{name}", (string name) =>
            {
                return $"Hello, Dear {name}";
            });

            app.MapGet("/health", () =>
            {
                return "Server is running";
            });

            app.MapGet("/object", () =>
            {
                return new
                {
                    status = 200,
                    message = "Ok",
                    data = new
                    {
                        username = "Ivan",
                        accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
                    }
                };
            });
            // спрацьовує екранування тому HTML нормально не відображається у браузері
            // Щоб спрацювало необхідно передати у заголовках Content-Type
            app.MapGet("/index", () =>
            {

                return "<h1>This Header1</h1>";
            });
            /*
            1-й Спосіб вказати тип контенту: Використовуємо клас-хелпер, у якому можна вказати тип контенту.
            */
            app.MapGet("/htmlText", () =>
            {
                var result = "<h1>This Header1</h1>";

                return Results.Content(result, contentType: "text/html", System.Text.Encoding.UTF8);
            });

            /*
            2-й спосіб використовуємо HttpContext
            */

            app.MapGet("/beatyHtml", (HttpContext context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                return "<h1 style=\"color: darkgreen; font-family: sans-serif;\">This Header1</h1>";
            });
            #endregion


            // HttpContext
            app.MapGet("/httpContext",(HttpContext httpContext) =>
            {
                var clientIp = httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
                System.Console.WriteLine($"Client IP: {clientIp}");
                System.Console.WriteLine($"User-Agent: {httpContext.Request.Headers["User-Agent"].ToString()}");
                System.Console.WriteLine($"Http method: {httpContext.Request.Method}");

                foreach(var item in httpContext.Request.Headers)
                {
                    System.Console.WriteLine($"Key:[{item.Key}]=>Value:[{item.Value}]");
                }

                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.Headers["X-MyHeader"] = "ASP.NET Core 10";
                return "Ok";
            });

            /* node js express
            app.map('/login', (req:Request, res: Response)=>{
                // щоб отримати детальну інформацію про запит
                // необхідно вручну діставати всю інформацію
            })
            
            */
            /*
            ASP може самостійно перетворити body у об'єкт. 
            Якщо об'єкт не Nullable він вважається обов'язковим, тобто якщо його не передати ASP автоматично відправить респонс зі статусом 
            400-BadRequest (Проблема клієнта, означає що запит сформовано не правильно, наприклад неправильний body, чи queryParams)
            */
            app.MapPost("/api/users/login", (LoginDto? loginData) =>
            {
                if (loginData is null)
                {
                    return Results.BadRequest(new
                    {
                        Status = 400,
                        Message = "BadRequest. Username and password required"
                    });
                }

                Console.WriteLine($"[POST] /api/users/login Data: username=>{loginData.Username} password=>{loginData.Password}");
                return Results.Ok(new
                {
                    Status = 200,
                    Message = $"{loginData.Username} successfuly logined",
                });

            });

            // Як опрацювати queryParams
            app.MapGet("/api/search", (string query, int page = 1) =>
            {
                System.Console.WriteLine($"Query: {query}");
                // 200 - OK 
                return Results.Ok(new
                {
                    Status = 200,
                    Message = "Ok",
                    Data = new
                    {
                        Query = query,
                        PageNumber = page,
                    }
                });
            });




            // Запуск додатку: запуск вбудованого веб-сервера Kestrel,
            // який слухає усі вхідні запити
            // для сервера створюється порт на якому він слухає
            app.Run();

            /*Завдання:
            Взяти реакт проєкт, збілдити його, і роздати бандли за допмогою ASP
            */
        }
    }
}

// Ctrl+K, Ctrl+F - formatrting code