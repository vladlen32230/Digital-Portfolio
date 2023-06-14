using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Text;

namespace DigitalPortfolioProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connection = "Data Source=SQL6030.site4now.net;Initial Catalog=db_a9955d_database;User Id=db_a9955d_database_admin;Password=trX9g}QPd8@wu.!";
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            var app = builder.Build();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.MapGet("/", async context =>
            {
                context.Response.ContentType = "text/html";
                if (context.User.Identity?.IsAuthenticated ?? false)
                    await context.Response.SendFileAsync("wwwroot/mainpageauthorized.html");
                else
                    await context.Response.SendFileAsync("wwwroot/mainpageunauthorized.html");
            });

            app.MapGet("/search", async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync("wwwroot/search.html");
            });

            app.MapGet("/enter", async context =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? false)
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync("wwwroot/enter.html");
                }
            });

            app.MapGet("/register", async context =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? false)
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.SendFileAsync("wwwroot/register.html");
                }
            });

            app.MapGet("/u/{userid}", async (string userid, HttpContext context) =>
            {
                context.Response.ContentType = "text/html";
                if (context.User.Identity?.IsAuthenticated ?? false)
                {
                    var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (id==userid)
                        await context.Response.SendFileAsync("wwwroot/profileowner.html");
                    else
                        await context.Response.SendFileAsync("wwwroot/profilenotowner.html");
                }

                else
                    await context.Response.SendFileAsync("wwwroot/profilenotowner.html");
            });

            app.MapGet("u/{userid}/changeinfo", async (string userid, HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false)
                {
                    var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (id==userid)
                    {
                        context.Response.ContentType = "text/html";
                        await context.Response.SendFileAsync("wwwroot/changeinfo.html");
                    }

                    else
                        context.Response.StatusCode = 403;
                }

                else
                    context.Response.StatusCode = 403;
            });

            app.MapGet("u/{userid}/addportfolio", async (string userid, HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false)
                {
                    var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (id==userid)
                    {
                        context.Response.ContentType = "text/html";
                        await context.Response.SendFileAsync("wwwroot/addportfolio.html");
                    }

                    else
                        context.Response.StatusCode = 403;
                }

                else
                    context.Response.StatusCode = 403;
            });

            app.MapGet("u/{userid}/p/{portfolioid}", async (string userid, string portfolioid, HttpContext context) =>
            {
                context.Response.ContentType = "text/html";
                if (context.User.Identity?.IsAuthenticated ?? false)
                {
                    var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userid == id)
                        await context.Response.SendFileAsync("wwwroot/portfolioowner.html");
                    else
                        await context.Response.SendFileAsync("wwwroot/portfolioauthorized.html");
                }

                else
                    await context.Response.SendFileAsync("wwwroot/portfoliounauthorized.html");
            });

            app.MapPost("/api/registeruser", async context =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? false)
                {
                    var data = context.Request.Form;
                    using var con = new SqlConnection(connection);
                    await con.OpenAsync();
                    var command = new SqlCommand();
                    command.Connection = con;
                    command.Parameters.AddWithValue("email", data["Email"].ToString());
                    command.CommandText = "SELECT count(*) " +
                                          "FROM users " +
                                          "WHERE email=@email";
                    var response = await command.ExecuteScalarAsync();
                    if ((int)response! == 1)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Пользователь с такой почтой уже существует");
                    }

                    else
                    {
                        command.CommandText = "INSERT INTO users(password, name, email) " +
                                              "VALUES(@password, @name, @email)";
                        command.Parameters.AddWithValue("password", data["Password"].ToString());
                        command.Parameters.AddWithValue("name", data["Name"].ToString());
                        await command.ExecuteNonQueryAsync();
                        command.CommandText = "SELECT id " +
                                              "FROM users " +
                                              "WHERE email=@email";
                        var reader = await command.ExecuteReaderAsync();
                        await reader.ReadAsync();
                        var id = reader.GetGuid(0);
                        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                                                       new Claim(ClaimTypes.Name, data["Name"].ToString()) };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    }

                    await con.CloseAsync();
                }
            });

            app.MapPost("/api/enteracc", async context =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? false)
                {
                    var data = context.Request.Form;
                    using var con = new SqlConnection(connection);
                    await con.OpenAsync();
                    var command = new SqlCommand();
                    command.Connection = con;
                    command.CommandText = "SELECT id, name " +
                                          "FROM users " +
                                          "WHERE email=@email AND password=@password";
                    command.Parameters.AddWithValue("email", data["Email"].ToString());
                    command.Parameters.AddWithValue("password", data["Password"].ToString());
                    var reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, reader.GetGuid(0).ToString()),
                                                       new Claim(ClaimTypes.Name, reader.GetString(1))};
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    }

                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Почта или пароль не верны");
                    }
                    await con.CloseAsync();
                }
            });

            app.MapPost("/api/search", async context =>
            {
                var data = context.Request.Form;
                var type = data["Type"].ToString();
                var name = data["Name"].ToString();
                var words = data["Words"].ToString().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                using var con = new SqlConnection(connection);
                await con.OpenAsync();
                var command = new SqlCommand();
                command.Connection = con;
                var sb = new StringBuilder();
                if (type == "project" || type == "user")
                {
                    command.Parameters.AddWithValue("name", name);
                    if (type == "project")
                    {
                        sb.Append("SELECT id, name, description, photo, author_id " +
                                              "FROM portfolios " +
                                              "WHERE name LIKE N'%' + @name + '%'");
                    }

                    else if (type == "user")
                    {
                        sb.Append("SELECT id, name, description, photo " +
                                              "FROM users " +
                                              "WHERE name LIKE N'%' + @name + '%'");
                    }

                    for (int i = 0; i < words.Length; i++)
                    {
                        if (i == 0)
                            sb.Append(" AND(");
                        if (i == words.Length - 1)
                            sb.Append($"description LIKE N'%{words[i]}%')");
                        else
                            sb.Append($"description LIKE N'%{words[i]}%' OR ");
                    }

                    command.CommandText = sb.ToString();
                    var reader = await command.ExecuteReaderAsync();
                    var results = new List<SearchInfo>();
                    while (await reader.ReadAsync())
                    {
                        var result = new SearchInfo
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Description = await reader.IsDBNullAsync(2) ? "" : reader.GetString(2),
                            Photo = await reader.IsDBNullAsync(3) ? "" : reader.GetString(3)
                        };

                        if (type == "project")
                            result.Author_id = reader.GetGuid(4);
                        results.Add(result);
                    }

                    await context.Response.WriteAsJsonAsync(results);
                }

                else
                    context.Response.StatusCode = 400;
                await con.CloseAsync();
            });

            app.MapGet("/api/getuserinfo", async context =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false)
                {
                    var userInfo = new UserInfo()
                    {
                        Id = context.User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                        Name = context.User.FindFirstValue(ClaimTypes.Name)!
                    };

                    await context.Response.WriteAsJsonAsync(userInfo);
                }
            });

            app.MapGet("/api/{userid}/getprofileinfo", async (string userid, HttpContext context) =>
            {
                using var con = new SqlConnection(connection);
                await con.OpenAsync();
                var command = new SqlCommand();
                command.Connection = con;
                command.CommandText = "SELECT name, description, first_skill, second_skill, third_skill, telegram, vk, photo, id " +
                                      "FROM users " +
                                      "WHERE id=@id";
                command.Parameters.AddWithValue("id", userid);
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var profileInfo = new ProfileInfo()
                    {
                        Name = reader.GetString(0),
                        Description = await reader.IsDBNullAsync(1) ? "" : reader.GetString(1),
                        First_skill = await reader.IsDBNullAsync(2) ? "" : reader.GetString(2),
                        Second_skill = await reader.IsDBNullAsync(3) ? "" : reader.GetString(3),
                        Third_skill = await reader.IsDBNullAsync(4) ? "" : reader.GetString(4),
                        Telegram = await reader.IsDBNullAsync(5) ? "" : reader.GetString(5),
                        Vk = await reader.IsDBNullAsync(6) ? "" : reader.GetString(6),
                        Photo = await reader.IsDBNullAsync(7) ? "" : reader.GetString(7),
                        Id = reader.GetGuid(8)
                    };

                    await reader.CloseAsync();
                    profileInfo.Portfolio_infos = new List<PortfolioInfo>();
                    command.CommandText = "SELECT id, name, photo " +
                                          "FROM portfolios " +
                                          "WHERE author_id=@id";
                    reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                        profileInfo.Portfolio_infos.Add(new PortfolioInfo()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Photo = await reader.IsDBNullAsync(2) ? "" : reader.GetString(2)
                        });

                    await context.Response.WriteAsJsonAsync(profileInfo);
                }

                else
                    context.Response.StatusCode = 404;
                await con.CloseAsync();
            });

            app.MapGet("api/u/{userid}/getdescription", async (string userid, HttpContext context) =>
            {
                using var con = new SqlConnection(connection);
                await con.OpenAsync();
                var command = new SqlCommand();
                command.Connection = con;
                command.CommandText = "SELECT description, first_skill, second_skill, third_skill, vk, telegram " +
                                      "FROM users " +
                                      "WHERE id=@id";
                command.Parameters.AddWithValue("id", userid);
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    await context.Response.WriteAsJsonAsync(new ProfileInfo()
                    {
                        Description = await reader.IsDBNullAsync(0) ? "" : reader.GetString(0),
                        First_skill = await reader.IsDBNullAsync(1) ? "" : reader.GetString(1),
                        Second_skill = await reader.IsDBNullAsync(2) ? "" : reader.GetString(2),
                        Third_skill = await reader.IsDBNullAsync(3) ? "" : reader.GetString(3),
                        Vk = await reader.IsDBNullAsync(4) ? "" : reader.GetString(4),
                        Telegram = await reader.IsDBNullAsync(5) ? "" : reader.GetString(5)
                    });
                else
                    context.Response.StatusCode = 404;
                await con.CloseAsync();
            });

            app.MapPut("api/u/{userid}/saveinfo", async (string userid, HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false
                    && context.User.FindFirstValue(ClaimTypes.NameIdentifier) == userid)
                {
                    var data = context.Request.Form;
                    using var con = new SqlConnection(connection);
                    await con.OpenAsync();
                    var command = new SqlCommand();
                    command.Connection = con;
                    command.CommandText = !data.ContainsKey("photo") ? "UPDATE users " +
                                          "SET description=@description, first_skill=@first_skill, second_skill=@second_skill, " +
                                          "third_skill=@third_skill, vk=@vk, telegram=@telegram " +
                                          "WHERE id=@id" :
                                          "UPDATE users " +
                                          "SET description=@description, first_skill=@first_skill, second_skill=@second_skill, " +
                                          "third_skill=@third_skill, vk=@vk, telegram=@telegram, photo=@photo " +
                                          "WHERE id=@id";
                    command.Parameters.AddWithValue("id", userid.ToString());
                    command.Parameters.AddWithValue("description", data["description"].ToString());
                    command.Parameters.AddWithValue("first_skill", data["first_skill"].ToString());
                    command.Parameters.AddWithValue("second_skill", data["second_skill"].ToString());
                    command.Parameters.AddWithValue("third_skill", data["third_skill"].ToString());
                    command.Parameters.AddWithValue("vk", data["vk"].ToString());
                    command.Parameters.AddWithValue("telegram", data["telegram"].ToString());
                    if (data.ContainsKey("photo"))
                    {
                        var photo = data["photo"].ToString();
                        if (photo[11] == 'p' && photo[13] == 'g')
                            photo = string.Concat("data:image/jpeg;base64,", photo.AsSpan(22));
                        command.Parameters.AddWithValue("photo", photo);
                    }

                    await command.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }

                else
                    context.Response.StatusCode = 403;
            });

            app.MapGet("api/unsign", async (context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false)
                    await context.SignOutAsync();
                else
                    context.Response.StatusCode = 403;
            });

            app.MapPost("api/u/{userid}/addportfolio", async (string userid, HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false
                    && context.User.FindFirstValue(ClaimTypes.NameIdentifier) == userid)
                {
                    var data = context.Request.Form;
                    using var con = new SqlConnection(connection);
                    await con.OpenAsync();
                    var command = new SqlCommand();
                    command.Connection = con;
                    command.CommandText = "INSERT INTO portfolios(author_id, name, description, reference, project, photo) " +
                                          "VALUES (@author_id, @name, @description, @reference, @project, @photo)";
                    command.Parameters.AddWithValue("author_id", userid);
                    command.Parameters.AddWithValue("name", data["name"].ToString());
                    command.Parameters.AddWithValue("description", data["description"].ToString());
                    command.Parameters.AddWithValue("reference", data["reference"].ToString());
                    var project = data.ContainsKey("file") ? data["file"].ToString() : "";
                    command.Parameters.AddWithValue("project", project);
                    if (data.ContainsKey("photo"))
                    {
                        var photo = data["photo"].ToString();
                        if (photo[11] == 'p' && photo[13] == 'g')
                            photo = string.Concat("data:image/jpeg;base64,", photo.AsSpan(22));
                        command.Parameters.AddWithValue("photo", photo);
                    }

                    else
                        command.Parameters.AddWithValue("photo", "");

                    await command.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }

                else
                    context.Response.StatusCode = 403;
            });

            app.MapGet("api/u/{userid}/p/{portfolioid}/getinfo", async (string userid, string portfolioid, HttpContext context) =>
            {
                using var con = new SqlConnection(connection);
                await con.OpenAsync();
                var command = new SqlCommand();
                command.Connection = con;
                var result = new PortfolioInfo();
                command.CommandText = "SELECT portfolios.name, portfolios.description, reference, project, portfolios.photo, users.name " +
                                      "FROM portfolios JOIN users ON " +
                                      "(users.id=portfolios.author_id) " +
                                      "WHERE portfolios.id=@id";
                command.Parameters.AddWithValue("id", portfolioid);
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    result.Name = reader.GetString(0);
                    result.Description = reader.GetString(1);
                    result.Reference = reader.GetString(2);
                    result.Project = reader.GetString(3);
                    result.Photo = reader.GetString(4);
                    result.Author_name = reader.GetString(5);
                    await reader.CloseAsync();
                    command.CommandText = "SELECT sum(rate) " +
                                          "FROM rating " +
                                          "WHERE portfolio_id=@portfolioid";
                    command.Parameters.AddWithValue("portfolioid", portfolioid);
                    var rate = await command.ExecuteScalarAsync();
                    if (rate is not DBNull)
                        result.Rating = (int)rate!;
                    if (context.User.Identity?.IsAuthenticated ?? false)
                    {
                        command.CommandText = "SELECT rate " +
                                              "FROM rating " +
                                              "WHERE author_id=@author_id";
                        command.Parameters.AddWithValue("author_id", context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                        var authorRate = await command.ExecuteScalarAsync();
                        if (authorRate is null)
                            result.Author_rate = "";
                        else if ((Int16)authorRate! == 1)
                            result.Author_rate = "Лайк поставлен";
                        else if ((Int16)authorRate! == -1)
                            result.Author_rate = "Дизлайк поставлен";
                    }

                    else
                        result.Author_rate = "";

                    command.CommandText = "SELECT author_id, commentaries.description, name, photo, commentaries.id " +
                                          "FROM commentaries JOIN users ON " +
                                          "(commentaries.author_id=users.id) " +
                                          "WHERE commentaries.portfolio_id=@portfolioid";
                    reader = await command.ExecuteReaderAsync();
                    result.Commentaries = new List<CommentaryInfo>();
                    while (await reader.ReadAsync())
                    {
                        var user_id = reader.GetGuid(0);
                        result.Commentaries.Add(new CommentaryInfo()
                        {
                            User_id = user_id,
                            Description = reader.GetString(1),
                            Name = reader.GetString(2),
                            Photo = await reader.IsDBNullAsync(3) ? "" : reader.GetString(3),
                            Is_owner = context.User.Identity?.IsAuthenticated ?? false &&
                                context.User.FindFirstValue(ClaimTypes.NameIdentifier) == user_id.ToString(),
                            Id = reader.GetGuid(4)
                        });
                    }

                    await context.Response.WriteAsJsonAsync(result);
                }

                else
                    context.Response.StatusCode = 404;
                await con.CloseAsync();
            });

            app.MapPost("api/u/{userid}/p/{portfolioid}/addcomment", async (string portfolioid, HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false)
                {
                    var data = context.Request.Form;
                    using var con = new SqlConnection(connection);
                    await con.OpenAsync();
                    var command = new SqlCommand();
                    command.Connection = con;
                    command.CommandText = "INSERT INTO commentaries(author_id, portfolio_id, description) " +
                                          "VALUES (@author_id, @portfolio_id, @description)";
                    command.Parameters.AddWithValue("author_id", context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    command.Parameters.AddWithValue("portfolio_id", portfolioid);
                    command.Parameters.AddWithValue("description", data["description"].ToString());
                    await command.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }

                else
                    context.Response.StatusCode = 403;
            });

            app.MapGet("api/deletecomment/{authorid}/{commentid}", async (string authorid, string commentid, HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false &&
                    context.User.FindFirstValue(ClaimTypes.NameIdentifier) == authorid)
                {
                    using var con = new SqlConnection(connection);
                    await con.OpenAsync();
                    var command = new SqlCommand();
                    command.Connection = con;
                    command.CommandText = "DELETE FROM commentaries " +
                                          "WHERE id=@id";
                    command.Parameters.AddWithValue("id", commentid);
                    await command.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }

                else
                    context.Response.StatusCode = 403;
            });

            app.MapPost("api/u/{userid}/p/{portfolioid}/like", async (string userid, string portfolioid, HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false &&
                    context.User.FindFirstValue(ClaimTypes.NameIdentifier) != userid)
                {
                    using var con = new SqlConnection(connection);
                    await con.OpenAsync();
                    var command = new SqlCommand();
                    command.Connection = con;
                    command.CommandText = "SELECT sum(rate) " +
                                          "FROM rating " +
                                          "WHERE author_id=@author_id AND portfolio_id=@portfolio_id";
                    command.Parameters.AddWithValue("author_id", context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    command.Parameters.AddWithValue("portfolio_id", portfolioid);
                    var result = await command.ExecuteScalarAsync();
                    if (result is not DBNull)
                    {
                        if ((int)result! == 1)
                        {
                            command.CommandText = "DELETE FROM rating " +
                                                  "WHERE author_id=@author_id AND portfolio_id=@portfolio_id";
                            await command.ExecuteNonQueryAsync();
                            await context.Response.WriteAsync("Лайк убран");
                        }

                        else if ((int)result == -1)
                        {
                            command.CommandText = "UPDATE rating " +
                                                  "SET rate=@rate " +
                                                  "WHERE author_id=@author_id AND portfolio_id=@portfolio_id";
                            command.Parameters.AddWithValue("rate", 1);
                            await command.ExecuteNonQueryAsync();
                            await context.Response.WriteAsync("Лайк поставлен");
                        }
                    }

                    else
                    {
                        command.CommandText = "INSERT INTO rating (author_id, portfolio_id, rate) " +
                                              "VALUES (@author_id, @portfolio_id, @rate)";
                        command.Parameters.AddWithValue("rate", 1);
                        await command.ExecuteNonQueryAsync();
                        await context.Response.WriteAsync("Лайк поставлен");
                    }

                    await con.CloseAsync();
                }

                else
                    context.Response.StatusCode = 403;
            });

            app.MapPost("api/u/{userid}/p/{portfolioid}/dislike", async (string userid, string portfolioid, HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false &&
                    context.User.FindFirstValue(ClaimTypes.NameIdentifier) != userid)
                {
                    using var con = new SqlConnection(connection);
                    await con.OpenAsync();
                    var command = new SqlCommand();
                    command.Connection = con;
                    command.CommandText = "SELECT sum(rate) " +
                                          "FROM rating " +
                                          "WHERE author_id=@author_id AND portfolio_id=@portfolio_id";
                    command.Parameters.AddWithValue("author_id", context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    command.Parameters.AddWithValue("portfolio_id", portfolioid);
                    var result = await command.ExecuteScalarAsync();
                    if (result is not DBNull)
                    {
                        if ((int)result! == -1)
                        {
                            command.CommandText = "DELETE FROM rating " +
                                                  "WHERE author_id=@author_id AND portfolio_id=@portfolio_id";
                            await command.ExecuteNonQueryAsync();
                            await context.Response.WriteAsync("Дизлайк убран");
                        }

                        else if ((int)result == 1)
                        {
                            command.CommandText = "UPDATE rating " +
                                                  "SET rate=@rate " +
                                                  "WHERE author_id=@author_id AND portfolio_id=@portfolio_id";
                            command.Parameters.AddWithValue("rate", -1);
                            await command.ExecuteNonQueryAsync();
                            await context.Response.WriteAsync("Дизлайк поставлен");
                        }
                    }

                    else
                    {
                        command.CommandText = "INSERT INTO rating (author_id, portfolio_id, rate) " +
                                              "VALUES (@author_id, @portfolio_id, @rate)";
                        command.Parameters.AddWithValue("rate", -1);
                        await command.ExecuteNonQueryAsync();
                        await context.Response.WriteAsync("Дизлайк поставлен");
                    }

                    await con.CloseAsync();
                }

                else
                    context.Response.StatusCode = 403;
            });

            app.MapDelete("api/u/{userid}/p/{portfolioid}/delete", async (string userid, string portfolioid, HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated ?? false &&
                    context.User.FindFirstValue(ClaimTypes.NameIdentifier) == userid)
                {
                    using var con = new SqlConnection(connection);
                    await con.OpenAsync();
                    var command = new SqlCommand();
                    command.Connection = con;
                    command.Parameters.AddWithValue("portfolio_id", portfolioid);
                    command.CommandText = "DELETE FROM rating " +
                                          "WHERE portfolio_id=@portfolio_id";
                    await command.ExecuteNonQueryAsync();
                    command.CommandText = "DELETE FROM commentaries " +
                                          "WHERE portfolio_id=@portfolio_id";
                    await command.ExecuteNonQueryAsync();
                    command.CommandText = "DELETE FROM portfolios " +
                                          "WHERE id=@portfolio_id";
                    await command.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }

                else
                    context.Response.StatusCode = 403;
            });

            app.Run();
        }
    }
}