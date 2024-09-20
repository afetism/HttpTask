using System.Net;
using System.Text.Json;
using System.Text;
using System;

namespace ServerSide;

public class WebHost
{
    public List<User> Users { get; set; }
    private int _port;
    private HttpListener _listener { get; set; }
    public WebHost(int port)
    {
        _port = port;
        Users = new List<User>();
        Users.AddRange(new List<User>() { new User() {Id=1, Name="Afet",Surname="Ismayilova",Age=20},
                                          new User() {Id=2, Name = "Hakuna", Surname = "Matata", Age = 40 }
            });
       

    }

    public void Run()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{_port}/");
        _listener.Start();
        Console.WriteLine($"Http sever start on {_port}");
        while ( true )
        {
            var context =_listener.GetContext();
            Task.Run(() => { HandleRequest(context); });
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        if (request.HttpMethod == "GET")
        {
            try
            {

                var json = JsonSerializer.Serialize(Users);
                var buffer = Encoding.UTF8.GetBytes(json);


                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;


                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }
        else if (request.HttpMethod == "POST")
        {
            try
            {
                using var reader = new StreamReader(request.InputStream);
                var body=reader.ReadToEnd();
                var newUser =JsonSerializer.Deserialize<User>(body);
                Users.Add(newUser);
            }
            catch (Exception)
            {

                throw;
            }
        }
        else if (request.HttpMethod == "DELETE")
        {
          
            var rawUrl = request.RawUrl;
            Console.WriteLine(rawUrl);
            var segments = rawUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length > 0)
            {
                string lastSegment = segments[^1];
                Console.WriteLine($"{lastSegment}");
                if (int.TryParse(lastSegment, out int userId))
                {
                    Users.Remove(Users.FirstOrDefault(U=>U.Id==userId));

                }
            }
           

            response.OutputStream.Close();
        }

    }
}
