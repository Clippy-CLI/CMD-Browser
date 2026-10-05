//Note that this program was originally made in java and was converted to c# later

//To do: fix bugs clean code

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
            bool handled = false;

            //yes the commands are only 1 line and yes I'm still using {} as it looks pretty
            //also i dont think it needs to be if/else because no two commands should be the same
            if (input == null || input.Equals("exit", StringComparison.OrdinalIgnoreCase)) {
                break;
            }

            if (input.Equals("previous")) {
                input = storeInput;
            }
            //allows you to type previous and bring up previous input

            if (input.Equals("common")) {
                handled = true;
                //prints some common webpages
                Console.WriteLine("Common Webpages:");

                Console.WriteLine("Wikipedia: https://en.wikipedia.org/wiki/(page)");
                Console.WriteLine("Simple wikipeida: https://simple.wikipedia.org/wiki/(page)");
                Console.WriteLine("Project Gutenberg (Ebooks): https://www.gutenberg.org/cache/epub/(bookcode)/pg(bookcode)-images.html");
                //add more later, especially when links are available.
            }

            if (input.StartsWith("linkscan", StringComparison.OrdinalIgnoreCase)) {
                handled = true;
                //essentially, will scan links of the webpage put in after linkscan
                try {
                    //substring is 9 as command is linkscan (space) url
                    string urlPart = input.Length > 9 ? input.Substring(9).Trim() : string.Empty;
                    var getter = new GetLinks(urlPart, "");
                    string result = await getter.GetLinksFromSiteAsync();

                    if (string.IsNullOrWhiteSpace(result)) {
                        Console.WriteLine("no links found");
                    }
                    else {
                        /*var lines = Regex.Split(result, "\r?\n");
                        foreach (var line in lines) {
                            var t = line.Trim();
                            if (!string.IsNullOrEmpty(t)) Console.WriteLine(t);
                        }
                        do nothing, we dont need to format the links more they are formatted in getLinks
                        */
                        // print the raw links output
                        Console.WriteLine(result);
                    }
                } catch (Exception e) {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            if (input.Equals("idk", StringComparison.OrdinalIgnoreCase)) {
                handled = true;
                Console.WriteLine("Sorry, not implemented yet");
            }

            if (input.Equals("help")) {
                handled = true;
                Console.WriteLine("exit - Closes the program");
                Console.WriteLine("previous - Runs the previous command/url");
                Console.WriteLine("common - Shows a list of common sites");
                Console.WriteLine("linkscan + (url) - Shows available links on the webpage");
                //remember to update this section with more commands as they are added
            }
            

            //after here add more commands, or if anything needs to be done before try

            storeInput = (input);
            if (!handled) {
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
                } catch (Exception e) {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
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
        } catch (Exception e) {
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
        } catch (Exception e) {
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

        while (true) {

            //its going to need to scan for href or something
            //also further note, oftentimes the image is src=\"url\"

            //put like, href here
            int start = work.IndexOf("href", StringComparison.OrdinalIgnoreCase);
            if (start == -1) break;

            work = work.Substring(start);
            int gt = work.IndexOf('"');
            //first \" in the link, so it does get removed

            if (gt == -1) break; // malformed

            work = work.Substring(gt + 1);
            //in this stage, would have the first/whatever number url, plus everything after it

            //now down here, just instead of adding that <p> </p> to the output and removing it, just add the link to the
            //string array and remove the link + href from the sccanning. repeat until all href links are indexed.

            //now down here there is only one '"' which is the end of the link
            int endP = work.IndexOf('"');
            if (endP == -1) {
                // take remainder
                break;
            }

            //replace this with add the link to the array
            //currently 0 (start of URL) to endP
            linkArray.Add(work.Substring(0, endP));
            //nice link is added to array for later
            //advance past the end of this link so scanning continues
            work = work.Substring(endP + 1);
        }

        var decodedSb = new StringBuilder();
        for (int indexCount = 0; indexCount < linkArray.Count; indexCount++) {
            //for each string in linkArray do something
            decodedSb.Append(linkArray[indexCount]);
            //add link to thing
            decodedSb.Append("\n");
            //add a line break after every link
        }

        return decodedSb.ToString();
    }
}


//keeping this separate for now
//for now, might just make this into a command
public class GetImageLinks {
    private readonly string chooseURL;
    private readonly string recievePart;
    //somewhere in here, we need an array to store the links
    //for images though, due to variable scope we could just keep linkArray
    List<String> linkArray = new List<String>();
    //linkArray.Add("(url)");
    //should be uppercase

    public GetImageLinks(string pickURL, string parameter) {
        chooseURL = pickURL;
        recievePart = parameter;
    }

    public async Task<string> GetImageLinksFromSiteAsync() {
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
        } catch (Exception e) {
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
        while (true) {

            //its going to need to scan for href or something
            //also further note, oftentimes the image is src=\"url\"

            //THIS IS WHERE WE GET IMAGE LINKS
            //THIS IS THE ONLY PART LEFT TO BASCIALLY CHANGE THE INDEXED LINKS FROM HREF TO SRC ONES
            //put like, src here
            int start = work.IndexOf("src", StringComparison.OrdinalIgnoreCase);
            if (start == -1) break;

            work = work.Substring(start);
            int gt = work.IndexOf("\"");
            //first \" in the src, so it does get removed

            if (gt == -1) break; // malformed

            work = work.Substring(gt + 1);
            //in this stage, would have the first/whatever number url, plus everything after it

            //now down here, just instead of adding that <p> </p> to the output and removing it, just add the link to the
            //string array and remove the link + href from the sccanning. repeat until all href links are indexed.

            //now down here there is only one \" which is the end of the link
            int endP = work.IndexOf("\"", StringComparison.OrdinalIgnoreCase);
            if (endP == -1) {
                // take remainder

                break;
            }


            //uses same thing as href and just grabs a raw link here (\"image.png\", \"script.js\", etc)
            //so actually, we can reuse it but just save the source urls only if it ends in like .jpg .png .jpeg .webp .svg, etc
            //this allows us to bascially completely re-use the href, and just filter stuff out at a later level



            //now to determine if it is an image using the above mentioned file formats
            string linkToAdd = work.Substring(0, endP);
            //just declaring it as a simpler variable to reuse
            string linkToAddExtension = linkToAdd.Length >= 4 ? linkToAdd.Substring(linkToAdd.Length - 4) : linkToAdd;
            //no idea if that math is even correct to get that extension or not, 4 is lazy way to do .png or jpeg, etc
            if (linkToAddExtension.Equals(".png")) {
                linkArray.Add(linkToAdd);
            //why use || when you can use else if?
            } else if (linkToAddExtension.Equals("jpeg")) {
                linkArray.Add(linkToAdd);
            } else if (linkToAddExtension.Equals(".jpg")) {
                linkArray.Add(linkToAdd);
            } else if (linkToAddExtension.Equals("webp")) {
                linkArray.Add(linkToAdd);
            }
            string decoded = ("");
            return decoded;
        }
        // ensure method always returns a string (keep returning empty string for now)
        return string.Empty;
    }
}
