using System.Text.Json;
using RestSharp;
using Discord;

class Program
{
   

    
    
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        HashSet<int> seenPostIds = new HashSet<int>();
        string baseUrl = "https://e621.net";
        //ENTER YOUR USERNAME FROM E621.NET HERE
        string username = "KRAKENcracker";
        //ENTER YOUR API KEY FROM E621.NET HERE
        string apiKey = "apikeyhere";
        //ENTER THE MESSAGES YOU WANT THE BOT TO RANDOMLY SEND WITH A IMAGE, LEAVE IT EMPTY IF YOU DONT WANT ANY
        string[] messagew = new string[] { "daddy", "OwO", "uWu", "UwU", "Ahhhhhhh", "Ouch", "Agggggh", "this message was randomly generated", "hi", "¯_(ツ)_/¯", "V.V", "gay ass furry", "i have a raging porn addiction", "you have a raging porn addiction", "go do something better then this", "[insert inspirational quote here]" };

        //ENTER YOUR DISCORD WEBHOOK HERE
        var client = new RestClient("webhookhere");
        //ENTER WHAT GENDER YOU WANT
        string gender = "female";

        string[] tags = new string[] { "hi_res", "anthro", "mammal", "loona_(helluva_boss)", "dragon", "nipples", "absurd_res", "fur", "canine", "tail", "butt", "ass" };
        //EVERYTHING BELOW HERE YOU DONT HAVE TO WORRY ABOUT JUST CLICK THE LITTLE START BUTTON IN YOUR IDE
        Random random = new();
        int counter = 1;
        
        while (true)
        {
            counter++;
            Thread.Sleep(5000);
            string tag = tags[random.Next(tags.Length)];
            // Set up URL and parameters
            string url = "https://e621.net/posts.json";
            var request = new RestRequest(url, Method.GET);
            request.AddParameter("tags", gender);
            request.AddParameter("tags", "nsfw");
            request.AddParameter("tags", tag);
            request.AddParameter("limit", 1);
            if (seenPostIds.Count > 0)
            {
                string excludedIds = string.Join(",", seenPostIds);
                request.AddParameter("exclude_posts", excludedIds);
            }

            // Set up headers
            request.AddHeader("User-Agent", "MyProject/1.0 (by " + username + " on e621)");
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + apiKey)));

            // Make API request
            var restClient = new RestClient();
            var response = restClient.Execute(request);
            string urlField = string.Empty;
            // Check for errors
            if ((int)response.StatusCode != 200)
            {
                Console.WriteLine("Error: " + response.Content);
            }
            else
            {
                
                // Parse the JSON response
                using (JsonDocument document = JsonDocument.Parse(response.Content))
                {
                    // Get the URL field for each post
                    JsonElement root = document.RootElement;
                    JsonElement posts = root.GetProperty("posts");
                    foreach (JsonElement post in posts.EnumerateArray())
                    {
                        int postId = post.GetProperty("id").GetInt32();
                        if (!seenPostIds.Contains(postId))
                        {
                            seenPostIds.Add(postId);
                            JsonElement file = post.GetProperty("file");
                            urlField = file.GetProperty("url").GetString();
                            Console.WriteLine(urlField);
                        }
                    }
                }
            }
            if( urlField.Contains("webm"))
            {
                continue;
            }
            
            var request1 = new RestRequest(Method.POST);

            request1.AddHeader("Content-Type", "application/json");
            string newmessage = messagew[random.Next(messagew.Length)];
            request1.AddJsonBody(new
            {
                username = "YiffBot alpha",
                avatar_url = "https://cdn.discordapp.com/attachments/937342289087451231/1085716191840047184/Screenshot_2023-03-15_200824.jpg",
                content = newmessage,
                embeds = new[]
                {
        new
        {
            image = new
            {
                url = urlField
            }
        }
    }
            });

            IRestResponse response1 = client.Execute(request1);
        }
    }
    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}
