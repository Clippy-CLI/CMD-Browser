using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

public class MyProgram {
    public static async Task Main(string[] args) {
        Console.WriteLine("Welcome to (Name of your browser) browser!");
        Console.WriteLine("Type exit to quit");
        Console.WriteLine();

        while (true) {
            Console.Write("Enter URL: ");
            string input = Console.ReadLine();
            if (input == null || input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            try {
                var getter = new GetData(input, "");
                string result = await getter.GetDataFromSiteAsync();

                if (string.IsNullOrWhiteSpace(result)) {
                    Console.WriteLine("(no readable <p> content found)");
                }
                else {
                    var lines = Regex.Split(result, "\r?\n");
                    foreach (var line in lines) {
                        var t = line.Trim();
                        if (!string.IsNullOrEmpty(t)) Console.WriteLine(t);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            Console.WriteLine("\nLine Break\n");
        }

        Console.WriteLine("Closed");
    }
}

// Port of the Java getData class to C#.
public class GetData
{
    private readonly string chooseURL;
    private readonly string recievePart;

    public GetData(string pickURL, string parameter)
    {
        chooseURL = pickURL;
        recievePart = parameter;
    }

    public async Task<string> GetDataFromSiteAsync()
    {
        string recievedData = string.Empty;

        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Browser_Name_CMDLineBrowser");
            client.DefaultRequestHeaders.Add("Accept", "text/html");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");

            var response = await client.GetAsync(chooseURL);
            // read as string even if not successful so we can inspect
            recievedData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode == 403)
                    return "Http 403, request rejected (Error Code: yg7f63g)";

                if (string.IsNullOrEmpty(recievedData))
                    return "Error loading website (Error Code: d3y7g3)";

                return "An error has occured, check the URL and try again (Error Code: v7d332)";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Looks bad: " + e);
            var eString = e.ToString();
            if (eString.IndexOf("403", StringComparison.OrdinalIgnoreCase) != -1)
            {
                return "Http 403, request rejected (Error Code: yg7f63g)";
            }
            else if (string.IsNullOrEmpty(recievedData))
            {
                return "Error loading website (Error Code: d3y7g3)";
            }
            else
            {
                return "An error has occured, check the URL and try again (Error Code: v7d332)";
            }
        }

        // now filter the html into something more readable: extract <p> contents
        var recievedNew = new StringBuilder();
        string work = recievedData;

        while (true)
        {
            int start = work.IndexOf("<p", StringComparison.OrdinalIgnoreCase);
            if (start == -1) break;

            work = work.Substring(start);
            int gt = work.IndexOf('>');
            if (gt == -1) break; // malformed

            work = work.Substring(gt + 1);
            int endP = work.IndexOf("</p", StringComparison.OrdinalIgnoreCase);
            if (endP == -1)
            {
                // take remainder
                recievedNew.Append(work);
                break;
            }

            recievedNew.Append(work.Substring(0, endP));

            int endPtag = work.IndexOf('>', endP);
            if (endPtag == -1) break;

            work = work.Substring(endPtag + 1);
        }

        string cleaned = recievedNew.ToString();

        // remove any remaining tags
        var sb = new StringBuilder();
        bool inTag = false;
        for (int i = 0; i < cleaned.Length; i++)
        {
            char c = cleaned[i];
            if (!inTag)
            {
                if (c == '<') { inTag = true; if (sb.Length > 0 && !char.IsWhiteSpace(sb[sb.Length - 1])) sb.Append(' '); }
                else sb.Append(c);
            }
            else
            {
                if (c == '>') inTag = false;
            }
        }

        string decoded = WebUtility.HtmlDecode(sb.ToString());
        // collapse whitespace and trim
        decoded = Regex.Replace(decoded, "\\s+", " ").Trim();

        return decoded;
    }
}
