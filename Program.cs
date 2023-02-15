using System.Text;
using PhotinoNET;
using PhotinoNET.Server;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Photino.HelloPhotino.React;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        PhotinoServer
            .CreateStaticFileServer(args, out string baseUrl)
            .RunAsync();

        // Window title declared here for visibility
        string windowTitle = "Photino.React Demo App";

        // Creating a new PhotinoWindow instance with the fluent API
        var window = new PhotinoWindow()
            .SetTitle(windowTitle)
            // Resize to a percentage of the main monitor work area
            //.Resize(50, 50, "%")
            // Center window in the middle of the screen
            .Center()
            // Users can resize windows by default.
            // Let's make this one fixed instead.
            .SetResizable(true)
            .RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
            {
                contentType = "text/javascript";
                return new MemoryStream(Encoding.UTF8.GetBytes(@"
                        (() =>{
                            window.setTimeout(() => {
                                alert(`🎉 Dynamically inserted JavaScript.`);
                            }, 1000);
                        })();
                    "));
            })
            // Most event handlers can be registered after the
            // PhotinoWindow was instantiated by calling a registration 
            // method like the following RegisterWebMessageReceivedHandler.
            // This could be added in the PhotinoWindowOptions if preferred.
            .RegisterWebMessageReceivedHandler((object sender, string message) =>
            {

                WindowMessage wm = JsonSerializer.Deserialize<WindowMessage>(message);

                if (wm == null){
                    Console.WriteLine("Couldn't build WindowMessage!");
                    return;
                }
                var window = (PhotinoWindow)sender;
                Console.WriteLine(Environment.CurrentDirectory);
            
                String response = String.Empty;

                switch(wm.Command){
                    case  "getUserProfile" :{
                        Console.WriteLine("getUserProfile...");     
                        wm.Parameters = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        window.SendWebMessage(JsonSerializer.Serialize(wm));                   
                        break;
                    }
                    case "message":{
                        response = $"Received message: \"{wm.Parameters}\"";

                        break;
                    }
                    default: {
                        window.SendWebMessage(JsonSerializer.Serialize(wm));
                        break;
                    }
                }
                // The message argument is coming in from sendMessage.
                // "window.external.sendMessage(message: string)"
                
                // Send a message back the to JavaScript event handler.
                // "window.external.receiveMessage(callback: Function)"
                window.SendWebMessage(response);
            })
            .Load($"{baseUrl}/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.
            window.IconFile = "wwwroot\\favicon.ico";

        window.WaitForClose(); // Starts the application event loop
    }
}
