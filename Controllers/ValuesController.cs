using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ParseCSVFromJson.Data.Models;

namespace ParseCSVFromJson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private bool first = true; // A flag to indicate that this is the first loop iteration when parsing out the first line
        private int bottomIndex = 0; // Indicates the starting loop iteration when parsing out the first line
        private int topIndex = 0; // Indicates the ending loop iteration when parsing out the first line
        private string firstLine = new string(""); // String to hold the first line with

        private const string HEADER_NAME = "HEADER FOR CHUNK:"; //Header to split on
        private const string DELIMITER = "\n"; //Delimiter to split on

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var lineItems = ParseResponse(); // 1) Parse the response into an uppercase list, filtering out all garbage

            var tokensPerChunk = ParseFirstLine(lineItems); // 2) Set the amount of tokens that exist between headers

            var chunks = BuildChunks(lineItems, tokensPerChunk); // 3) Build the chunks
                                          
            /* Now, we can work with the chunks */

            for (var i = 0; i < chunks.Count; i++) // Loop over all chunks
            {
                for (var j = 0; j < tokensPerChunk; j++) // Loop through each individual chunk

                {
                    // Business logic goes here
                    // Here is an example of how to access each key/value pair for each loop iteration 
                    var kvp = chunks[i][j].Split(": ", 2);
                    var key = kvp[0];
                    var value = kvp[1];

                    // This inner loop is looping over each key value pair item in each chunk

                    // For example, the first chunk would iterated over like so:

                    //      Iteration #1:    Key: LAYOUT         Value: Alpha
                    //      Iteration #2:    Key: TEMPLATE       Value: Model-1234
                    //      Iteration #3:    Key: ID             Value: 1

                    // When the outer loop iterates again, the second chunk gets iterated over:

                    //      Iteration #1:    Key: LAYOUT         Value: Beta
                    //      Iteration #2:    Key: TEMPLATE       Value: Model-4321
                    //      Iteration #3:    Key: ID             Value: 2

                }
            }
            return Ok(chunks);
        }

        /* Function that accepts a list and a number of tokens per chunk and breaks the list
        into chunks of the size indicated by tokensPerChunk */
        private List<List<string>> BuildChunks(List<string> lineItems, int tokensPerChunk)
        {
            // Create a list to contain only the data we want to work with - strip out all unecessary lines
            var strippedList = new List<string>();

            var rawCount = lineItems.Count();
            // Iterate over lineItems
            // Make a new list containing anything contained between "HEADER FOR CHUNK" and "HEADER FOR CHUNK":
            for (var h = 0; h < rawCount; h++)
            {
                // If we encounter HEADER FOR CHUNK
                if (!lineItems[h].Contains(HEADER_NAME))
                {
                    // Continue to the next iteration
                    continue;
                }
                else // If we do not encounter HEADER FOR CHUNK
                {
                    // Initialize while loop variable
                    var header = "";

                    //While the header variable does not contain 'HEADER FOR CHUNK'
                    do
                    {
                        // Increment h. This helps us return to the next chunk in the list when we are done
                        h++;

                        // Add the token to strippedList
                        strippedList.Add(lineItems[h]);

                        var nextIteration = h + 1;

                        // If the next loop iteration would not be larger than the bounds of the list we are 
                        //   looping for
                        if (nextIteration < rawCount)
                        {
                            // Set header to the value of the next loop iteration (to detect if it contains 
                            //   HEADER FOR CHUNK)
                            header = lineItems[nextIteration];
                        }
                        else // If the loop iteration would be out of bounds
                        {
                            // Set header so we exit the loop cleanly
                            header = HEADER_NAME;
                        }
                    }
                    while (!header.Contains(HEADER_NAME));
                }
            }

            // Break the stripped list into chunks
            List<List<string>> lineItemChunks = strippedList.SplitList(tokensPerChunk);
            return lineItemChunks;
        }

        /* Parses a JSON response, filters out "", filters out lines that contain all 
        capitalization variations of the word stuff and returns the response broken out 
        into a list */
        private List<string> ParseResponse()
        {
            Response mockResponse = MockResponse(); //Simulated deserialzied JSON response


            //Create a list that splits on each DELIMITER, converted to upper case that filters out
            //all capitalization variations of the word 'stuff' and occurences of ""
            List<string> lineItems = mockResponse.lineItems
                .Split(DELIMITER)
                .ToList()
                .ConvertAll(d => d.ToUpper())
                .Where(x => !x.Contains("STUFF"))
                .Where(x => x != "\"\"")
                .ToList(); ;
            return lineItems;
        }

        /* Parses the first usable line out to be able to generate tokensPerChunk */
        private int ParseFirstLine(List<string> lineItems)
        {
            for (var z = 0; z < lineItems.Count(); z++)
            {
                //If we iterated over the header 
                if (lineItems[z].Contains(HEADER_NAME))
                {
                    //and if this the first loop iteration
                    if (first)
                    {
                        //set the bottom index of the first line 
                        bottomIndex = z + 1;
                        //indicate that this is no longer the first loop iteration
                        first = false;
                    }
                    else
                    {   //If this is the second time we have encountered the header, set top index and stop the loop from any further iteration 
                        // as we have our top and bottom index of the first line now.
                        topIndex = z;
                        break;
                    }
                }
            }

            //Using the indexes built in the previous step, find the first real line we want to look at in the lineitems
            for (var d = bottomIndex; d < topIndex; d++)
            {
                //Build the first line to be able to tokenize to get a count of max tokens per chunk
                firstLine += $"{lineItems[d]}\n";
            }

            //Get a count of tokens per chunk
            return firstLine.Split(DELIMITER).Count() - 1;
        }

        /* Builds a mock response object to use */
        private Response MockResponse()
        {
            var MockResponse = new Response();
            MockResponse.task = "Install Widgets";
            MockResponse.quantity = 8;
            MockResponse.description = "Installation of 8 different styles of widgets";
            MockResponse.lineItems =
                           "1: UNECESSARY DATA: 1111\nDATE:11/11/11\n" +
                           "2: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Alpha\nTEMPLATE: Model-1234\nID: 1\n" +
                           "3: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Beta\nTEMPLATE: Model-4321\nID: 2\n" +
                           "4: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Gamma\nTEMPLATE: Model-4444\nID: 3\n\"\"\nstuff we dont care about\n\"\"\n" +
                           "5: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Delta\nTEMPLATE: Model-1234\nID: 4\n" +
                           "6: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Epsilon\nstuff we dont care about\nTEMPLATE: Model-3333\nID: 5\n\"\"\n" +
                           "7: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Zeta\nTEMPLATE: Model-2222\n\"\"\nID: 7\n" +
                           "101: HEADER FOR CHUNK:\n" +
                           "Stuff we dont care about\nLAYOUT: Eta\n\"\"\nTEMPLATE: Model-1221\nID: 101\n" +
                           "10: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Theta\nTEMPLATE: Model-1001\nID: 10\nStuff we dont care about";
            return MockResponse;
        }
    }
}
