//Note that this program was originally made in java and was converted to c# later
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

using System.Collections.Generic;
//arraylist for c#, currently used for gathering links/images

public class MyProgram {
    public static async Task Main(string[] args) {
        Console.WriteLine("Welcome to CMD browser!");
        Console.WriteLine("Type help for a list of commands");
        Console.WriteLine("Type exit to quit");
        Console.WriteLine();
        string storeInput = ("");
        //create var to store the input for later
        while (true) {
            Console.Write("Enter URL: ");
            string input = Console.ReadLine();
            
            //yes the commands are only 1 line and yes I'm still using {} as it looks pretty
            //also i dont think it needs to be if/else because no two commands should be the same

            if (input.Equals("previous")) {
                input = storeInput;
            }
            //allows you to type previous and bring up previous input
            
            if (input == null || input.Equals("exit", StringComparison.OrdinalIgnoreCase)) {
                break;
            }

            if (input.Equals("common")) {
                //prints some common webpages
                Console.WriteLine("Common Webpages:");

                Console.WriteLine("Wikipedia: https://en.wikipedia.org/wiki/(page)");
                Console.WriteLine("Simple wikipeida: https://simple.wikipedia.org/wiki/(page)");
                Console.WriteLine("Project Gutenberg (Ebooks): https://www.gutenberg.org/cache/epub/(bookcode)/pg(bookcode)-images.html");
                //add more later, especially when links are available.
            }

            if (input.Substring(0,8).equals("linkscan")) {
                Console.WriteLine("Sorry, not implemented yet");
                //essentially, will scan links of the webpage put in after linkscan
                //just create more modified classes of getData, like how I am making one for image urls, also links i guess
            }

            if (input.equals("idk")) {
                Console.WriteLine("Sorry, not implemented yet");
            }

             if (input.Equals("help")) {
                Console.WriteLine("Exit - Closes the program");
                Console.WriteLine("Previous - Runs the previous command/url");
                Console.WriteLine("Common - Shows a list of common sites");
                Console.WriteLine("Linkscan + (url) - Shows available links on the webpage");
                //remember to update this section with more commands as they are added
            }
            

            //after here add more commands, or if anything needs to be done before try

            storeInput = (input);
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
            catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
            }

            Console.WriteLine("\nLine Break\n");
        }

        Console.WriteLine("Closed");
    }
}

// Port of the Java getData class to C#.
public class GetData {
    private readonly string chooseURL;
    private readonly string recievePart;

    public GetData(string pickURL, string parameter) {
        chooseURL = pickURL;
        recievePart = parameter;
    }

    public async Task<string> GetDataFromSiteAsync() {
        string recievedData = string.Empty;

        try {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Browser_Name_CMDLineBrowser");
            client.DefaultRequestHeaders.Add("Accept", "text/html");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");

            var response = await client.GetAsync(chooseURL);
            // read as string even if not successful so we can inspect
            recievedData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                if ((int)response.StatusCode == 403) {
                    return "Http 403, request rejected (Error Detected as bot/blocked traffic";
				}

                if (string.IsNullOrEmpty(recievedData)) {
                    return "Error loading website (Error No Content Recieved From Site)";
				}

                return "An error has occured, check the URL and try again (Error Site not found";
            }
        }
        catch (Exception e) {
            Console.WriteLine("Looks bad: " + e);
            var eString = e.ToString();
            if (eString.IndexOf("403", StringComparison.OrdinalIgnoreCase) != -1) {
                return "Http 403, request rejected (Error Code: yg7f63g)";
            }
            else if (string.IsNullOrEmpty(recievedData)) {
                return "Error loading website (Error Code: d3y7g3)";
            }
            else {
                return "An error has occured, check the URL and try again (Error Code: v7d332)";
            }
        }

        // now filter the html into something more readable: extract <p> contents
        var recievedNew = new StringBuilder();
        string work = recievedData;

        /*Removed images design comment, as it as moved to the GetLinks, for some reason idk why

        oh wait, because for now, links and images are just different kinds of links so they would look similar

        Also just a note to self: i did check the original java code, and yes it did only grab anything in <p> tags. the 
        improvement was that it could grab p tags from anywhere in the html not just the main areas

        */
        while (true) {
            int start = work.IndexOf("<p", StringComparison.OrdinalIgnoreCase);
            if (start == -1) break;

            work = work.Substring(start);
            int gt = work.IndexOf('>');
            if (gt == -1) break; // malformed

            work = work.Substring(gt + 1);
            int endP = work.IndexOf("</p", StringComparison.OrdinalIgnoreCase);
            if (endP == -1) {
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
        for (int i = 0; i < cleaned.Length; i++) {
            char c = cleaned[i];
            if (!inTag) {
                if (c == '<') { inTag = true; if (sb.Length > 0 && !char.IsWhiteSpace(sb[sb.Length - 1])) sb.Append(' '); }
                else sb.Append(c);
            }
            else {
                if (c == '>') inTag = false;
            }
        }

        string decoded = WebUtility.HtmlDecode(sb.ToString());
        // collapse whitespace and trim
        decoded = Regex.Replace(decoded, "\\s+", " ").Trim();

        return decoded;
    }
}

public class GetLinks {
    private readonly string chooseURL;
    private readonly string recievePart;
    //somewhere in here, we need an array to store the links
    List<String> linkArray = new List<String>();
    //linkArray.Add("(url)");
    //should be uppercase

    

    public GetLinks(string pickURL, string parameter) {
        chooseURL = pickURL;
        recievePart = parameter;
    }

    public async Task<string> GetLinksFromSiteAsync() {
        string recievedData = string.Empty;

        try {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Browser_Name_CMDLineBrowser");
            client.DefaultRequestHeaders.Add("Accept", "text/html");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");

            var response = await client.GetAsync(chooseURL);
            // read as string even if not successful so we can inspect
            recievedData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                if ((int)response.StatusCode == 403) {
                    return "Http 403, request rejected (Error Detected as bot/blocked traffic";
				}

                if (string.IsNullOrEmpty(recievedData)) {
                    return "Error loading website (Error No Content Recieved From Site)";
				}

                return "An error has occured, check the URL and try again (Error Site not found";
            }
        }
        catch (Exception e) {
            Console.WriteLine("Looks bad: " + e);
            var eString = e.ToString();
            if (eString.IndexOf("403", StringComparison.OrdinalIgnoreCase) != -1) {
                return "Http 403, request rejected (Error Code: yg7f63g)";
            }
            else if (string.IsNullOrEmpty(recievedData)) {
                return "Error loading website (Error Code: d3y7g3)";
            }
            else {
                return "An error has occured, check the URL and try again (Error Code: v7d332)";
            }
        }

        // now filter the html into something more readable: extract <p> contents
        var recievedNew = new StringBuilder();
        string work = recievedData;

        /*easiest way of recieving images will likely add in here plan is to (without changing work variable)
        go in and get all of the image src (to account for multiple tags, maybe just also check for http or something)
        basically then save all those urls for later in an array or something

        then as an initial experiment, just build the png/jpeg (easiest/common) to ascii converter (likely as another method
        so not in the GetData or main) then just print them all at the end at first.

        After which, I think easiest would be to somehow make a marker or index system of where the images are and then
        add a simple check while it is getting rid of all the "junk" html to periodically (like after a set of <p> tags
        or something) to print the image

        Also just a note to self: i did check the original java code, and yes it did only grab anything in <p> tags. the 
        improvement was that it could grab p tags from anywhere in the html not just the main areas

        */

        //THIS is what needs to be rewritten, both for the getlinks as well as images, in order to extract links.
        //also jsut unrelated note about images, some can be like /images/image.png, so if no full url, just append url to
        //the front of it and it hopefully works. Will need a fallback/error handling for this as it is probably risky
        while (true) {

            //its going to need to scan for href or something
            //also further note, oftentimes the image is src="url"

            //put like, href here
            int start = work.IndexOf("<p", StringComparison.OrdinalIgnoreCase);
            if (start == -1) break;

            work = work.Substring(start);
            int gt = work.IndexOf('>');
            //this would instead be like, the second " because it would say like href="(url)"
            if (gt == -1) break; // malformed

            work = work.Substring(gt + 1);
            //in this stage, would have the first/whatever number url, plus everything after it


            //now down here, just instead of adding that <p> </p> to the output and removing it, just add the link to the
            //string array and remove the link + href from the sccanning. repeat until all href links are indexed.
            int endP = work.IndexOf("</p", StringComparison.OrdinalIgnoreCase);
            if (endP == -1) {
                // take remainder
                recievedNew.Append(work);
                break;
            }

            recievedNew.Append(work.Substring(0, endP));

            int endPtag = work.IndexOf('>', endP);
            if (endPtag == -1) break;

            work = work.Substring(endPtag + 1);

        }

        //now down here we should have all of the links indexed, and we shouldn't also need to clean them
        //thus just throw away anything leftover and just return a string of all the links put together, plus
        //something like the \n to mean new line, so it prints out all the links in a nice list

        string cleaned = recievedNew.ToString();

        // remove any remaining tags

        //i think this is further cleaning, but idk yet what to do with this for images/links
        var sb = new StringBuilder();
        bool inTag = false;
        for (int i = 0; i < cleaned.Length; i++) {
            char c = cleaned[i];
            if (!inTag) {
                if (c == '<') { inTag = true; if (sb.Length > 0 && !char.IsWhiteSpace(sb[sb.Length - 1])) sb.Append(' '); }
                else sb.Append(c);
            }
            else {
                if (c == '>') inTag = false;
            }
        }

        string decoded = WebUtility.HtmlDecode(sb.ToString());
        // collapse whitespace and trim
        decoded = Regex.Replace(decoded, "\\s+", " ").Trim();

        return decoded;
    }
}
