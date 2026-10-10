//Note that this program was originally made in java and was converted to c# later

//To do: fix bugs clean code
//improve images maybe
//other ideas to add: more commands, implementations of normal browser features, idk what else


//notes from test run 10/9/2026:
//for sacefile, say whaat the file is saved as, not just "saved file"
//but savepage and readfile actually work!
//change the enter url thing to soemthing else because now there are commands
//image command still says no images found
//replit does work very well for creating a shareable link for people to see! I am very happy about this
//dotnet run --project browser/browser.csproj note that instead of just dotnet run, it also does say the warning as well
//as the license issue. Hopefully not too big of a problem but look into that

using SixLabors.ImageSharp;
using System.IO;
using SixLabors.ImageSharp.Processing;
using System;
using System.Net.Http;
using System.Text;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;

using System.Collections.Generic;
//arraylist for c#, currently used for gathering links/images

public class MyProgram {
    public static async Task Main(string[] args) {
        Console.WriteLine("+-----+");
        Console.WriteLine("|--H--|");
        Console.WriteLine("+-----+");
        //neat image for now
        Console.WriteLine("Welcome to Hydrogen browser!");
        Console.WriteLine("Type help for a list of commands");
        Console.WriteLine("Type exit to quit");
        Console.WriteLine("Made by Clippy-CLI");
        Console.WriteLine();
        string storeInput = ("");
        List<String> tabArray = new List<String>();
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
                Console.WriteLine("Legible news: https://legiblenews.com/");
                Console.WriteLine("");
                //add more later, especially when links are available.
                //ideas: google code styleguides, stock tickers or something, whatever else i can maybe find
               
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
            //for now, Im just running images as a command
            if (input.StartsWith("images", StringComparison.OrdinalIgnoreCase)) {
                handled = true;
                //uhh idk i copied this from link command
                try {
                    //images is 7 as command is linkscan (space) url
                    string urlPart = input.Length > 7 ? input.Substring(7).Trim() : string.Empty;
                    var getter = new GetImageLinks(urlPart, "");
                    string result = await getter.GetImageLinksFromSiteAsync();

                    if (string.IsNullOrWhiteSpace(result)) {
                        Console.WriteLine("no images found");
                    }
                    else {
                        Console.WriteLine(result);
                    }
                } catch (Exception e) {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            if (input.Equals("tab", StringComparison.OrdinalIgnoreCase)) {
                handled = true;
                Console.WriteLine(storeInput + "added as a tab");
                tabArray.Add(storeInput);
                //maybe add a thing so that you name it whatever you want but i'll add that later
                //could also just make it simple one string array all even numbers (0,2,4) are the tab names and
                //odd numbers (1,3,5) are the url
            }

            if (input.Equals("tabs", StringComparison.OrdinalIgnoreCase)) {
                handled = true;
                Console.WriteLine("Open Tabs:");
                for (int i = 0; i < tabArray.Count; i++) {
                    Console.WriteLine(tabArray[i]);
                }
                //simply print an arraylist 
            }

            if (input.Equals("clear", StringComparison.OrdinalIgnoreCase)) {
                handled = true;
                Console.Clear();
                //clears the terminal
            }

            if (input.Equals("game", StringComparison.OrdinalIgnoreCase)) {
                //start a game, all handled right here
                Console.WriteLine("Sorry not implemented yet");
                /*hmm, what would be a fun game, ideas:
                I want it to be unique
                lets just wait on this
                */
            }

            if (input.StartsWith("savepage", StringComparison.OrdinalIgnoreCase)) {
                handled = true;
                //essentially, will scan links of the webpage put in after linkscan
                try {
                    //substring is 9 as command is linkscan (space) url
                    string urlPart = input.Length > 9 ? input.Substring(9).Trim() : string.Empty;
                    var getter = new GetData(urlPart, "");
                    string result = await getter.GetDataFromSiteAsync();

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

                        //string = result
                        //linkname.txt saves all the data that it has I think here

                        //for now I am calling it savedSite.txt as using like http:// has a lot of characters that 
                        //cannot be used in filenames
                        File.WriteAllText("savedSite.txt", result);
                        Console.WriteLine("Page saved as txt file");
                    }
                } catch (Exception e) {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            if (input.StartsWith("readfile", StringComparison.OrdinalIgnoreCase)) {
                handled = true;
                    string filePath = input.Length > 9 ? input.Substring(9).Trim() : string.Empty;
                        try {
                            // Read the entire file into a string
                            string fileText = File.ReadAllText(filePath);
                            Console.WriteLine(fileText);
                            Console.WriteLine("File contents displayed successfully");
                        } catch (IOException e)  {
                            Console.WriteLine($"An error occurred while reading the file: {e.Message}");
                        }
        
            }

            if (input.Equals("help")) {
                handled = true;
                Console.WriteLine("exit - Closes the program");
                Console.WriteLine("previous - Runs the previous command/url");
                Console.WriteLine("common - Shows a list of common sites");
                Console.WriteLine("linkscan + (url) - Shows available links on the webpage");
                Console.WriteLine("tab - opens previous input as a tab");
                Console.WriteLine("tabs - Shows a list of open tabs");
                Console.WriteLine("images + (url) - displays images from site as ascii");
                Console.WriteLine("clear - clears the terminal");
                Console.WriteLine("game - a small, fun, offline game");
                Console.WriteLine("savepage + (url) - saves the page as a text file");
                Console.WriteLine("readfile + (file) - reads a specified txt file");
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
            client.DefaultRequestHeaders.Add("User-Agent", "Hydrogen-Browser");
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
            client.DefaultRequestHeaders.Add("User-Agent", "Hydrogen-Browser");
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
        string recievedData = ("");
        //in other words, this is returning empty somehow

        try {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Hydrogen-Browser");
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
            //first " in the src, so it does get removed

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
                if ((linkToAdd.Length < 5)||!linkToAdd.Substring(0,3).Equals("http")) {
                    linkArray.Add(chooseURL + linkToAdd);
                    //probably should get add url then the actual image if it isnt stored right
                } else {
                    linkArray.Add(linkToAdd);
                }
            //why use || when you can use else if?
            } else if (linkToAddExtension.Equals("jpeg")) {
                if ((linkToAdd.Length < 5)||!linkToAdd.Substring(0,3).Equals("http")) {
                    linkArray.Add(chooseURL + linkToAdd);
                    //probably should get add url then the actual image if it isnt stored right
                } else {
                    linkArray.Add(linkToAdd);
                }
                linkArray.Add(linkToAdd);
            } else if (linkToAddExtension.Equals(".jpg")) {
                if ((linkToAdd.Length < 5)||!linkToAdd.Substring(0,3).Equals("http")) {
                    linkArray.Add(chooseURL + linkToAdd);
                    //probably should get add url then the actual image if it isnt stored right
                } else {
                    linkArray.Add(linkToAdd);
                }
            } else if (linkToAddExtension.Equals("webp")) {
                if ((linkToAdd.Length < 5)||!linkToAdd.Substring(0,3).Equals("http")) {
                    linkArray.Add(chooseURL + linkToAdd);
                    //probably should get add url then the actual image if it isnt stored right
                } else {
                    linkArray.Add(linkToAdd);
                }
            }

            //part we need to add
            //now is gonna return a whole lot of stuff
        }








        //moving issue space to down here as decoded is empty


          //definitely getting to this part as otherwise it wouldnt return as nothing
        //from also what I'm finding, it does return the correct string, that string just has nothing
        //it doesnt need to be a stringbuilder...

        //there are 2 possible sources of error. Either images arent made correctly or it truly isnt finding anything
        //i will run a quick test where I will add like AAA to each image and see if that solves it
        //okay it still says no images found even If i add like ABC to decoded string meaning that decoded isnt even
        //being added to correctly
        //next lets try changing it to a stringbuilder
        //ok so stringbuilder is not compiling
        //moved the return to outside the while loop, so now while loop gets links and everything
        //i did just run a sanity check that the src and link sources are structured correctly (at least for wikipedia)
        //last checked on 10/9/2026 im taking a break








































        string decoded = ("");
            for (int i = 0; i < linkArray.Count; i++) {

                
                //as per my understanding, gets images from URL
                //i think here, we may need to make a new http client as the previous one is only text
                //so i guess we make another http client for images?
                // or is it there a method to jsut, get all images from a site?
                //actually no, lets not do that as the image links can probably be helpful somewhere else in the future

                    using var imageClient = new HttpClient();
                    imageClient.DefaultRequestHeaders.Add("User-Agent", "Hydrogen-Browser");
                    imageClient.DefaultRequestHeaders.Add("Accept", "text/html");
                    imageClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                    imageClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    var response = await imageClient.GetAsync(linkArray[i]);

                    recievedData = await response.Content.ReadAsStringAsync();
                        using (Stream streamImage = await imageClient.GetStreamAsync(linkArray[i])) {
                        using (Image image = await Image.LoadAsync(streamImage)) {
                            

                            //here convert given image to ascii then add to decoded

                                //suppsoed example I found
                                            char[] AsciiRamp = { ' ', '.', ':', '-', '=', '+', '*', '%', '@', '#' };
                                            

                                            using Image<Rgb24> imageToPrint = image.CloneAs<Rgb24>();
                                            int width = imageToPrint.Width;
                                            int height = imageToPrint.Height;
                                            //change dimensions
                                            double aspectRadio = (double)imageToPrint.Height / imageToPrint.Width;
                                            int targetHeight = (int)(80 * aspectRadio * 0.5);

                                            //resize
                                            imageToPrint.Mutate(ctx => ctx.Resize(80, targetHeight));

                                            var asciiBuilder = new StringBuilder();

                                            imageToPrint.ProcessPixelRows(accessor =>{
                                                for (int y = 0; y < accessor.Height; y++) {
                                                    Span<Rgb24> pixelRow = accessor.GetRowSpan(y);

                                                    for (int x = 0; x < pixelRow.Length; x++) {
                                                        Rgb24 pixel = pixelRow[x];

                                                        // Standard luminance formula to convert RGB to Grayscale
                                                        double luminance = (0.2126 * pixel.R) + (0.7152 * pixel.G) + (0.0722 * pixel.B);

                                                        // Map the 0-255 luminance value to our 0-(AsciiRamp.Length - 1) index
                                                        int rampIndex = (int)(luminance / 255.0 * (AsciiRamp.Length - 1));

                                                        asciiBuilder.Append(AsciiRamp[rampIndex]);
                                                    }

                                                    // Move to the next text row
                                                    asciiBuilder.AppendLine();
                                                }
                                            });
                            
                            decoded = (decoded + asciiBuilder);
                            //add the image which should include \n between lines with appendline
                            decoded = (decoded + "\n");
                            decoded = (decoded + "\n");
                            decoded = (decoded + "\n");
                            //big line break between each image
                        }
                }


                
            }
        return ("");
    }
}
