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
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {

            // Simulate a deserialized JSON API response with a delimited list as a field value
            var DeserializedApiResponse = new Response();
            DeserializedApiResponse.task = "Install Widgets";
            DeserializedApiResponse.quantity = 8;
            DeserializedApiResponse.description = "Installation of 8 different styles of widgets";
            DeserializedApiResponse.lineItems =
                           "1: UNECESSARY DATA: 1111\nDATE:11/11/11\n" +
                           "2: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Alpha\nTEMPLATE: Model-1234\nID: 1\n" +
                           "3: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Beta\nTEMPLATE: Model-4321\nID: 2\n" +
                           "4: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Gamma\nTEMPLATE: Model-4444\nID: 3\n" +
                           "5: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Delta\nTEMPLATE: Model-1234\nID: 4\n" +
                           "6: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Epsilon\nTEMPLATE: Model-3333\nID: 5\n" +
                           "7: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Zeta\nTEMPLATE: Model-2222\nID: 5\n" +
                           "8: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Eta\nTEMPLATE: Model-1221\nID: 6\n" +
                           "8: HEADER FOR CHUNK:\n" +
                           "LAYOUT: Theta\nTEMPLATE: Model-1001\nID: 7";

            // Declare what our chunk header will be and standard delimiter will be
            var headerName = "HEADER FOR CHUNK:";
            var delimiter = "\n";

            // Split lineItems on the line break(/n) to create a list containing a string representing a key value pair
            // Also get the count of rawLineItemList for use later
            var rawLineItemList = DeserializedApiResponse.lineItems.Split(delimiter).ToList();
            var rawCount = rawLineItemList.Count;

            // Pull the first lineitem of the response into a string so that we can find tokens per lineitem and create 
            // delimiters for what will be the beginning and end of the first real line of useful data
            var firstLineStartDelimiter = $"{headerName}{delimiter}";
            var firstLineEndDelimiter = $"{delimiter}{headerName}{delimiter}";
            var firstLine = DeserializedApiResponse.lineItems.Split(
                new string[] { firstLineStartDelimiter, firstLineEndDelimiter }, StringSplitOptions.None)[1];

            // Calculate tokens per lineitem to be used as a loop top end 
            // The -1 is because this pulls 1 more than the tokens per lineitem due to line numbers skewing the results
            var tokensPerLineItem = firstLine.Split(delimiter).Count() - 1;

            // Create a list to contain only the data we want to work with - strip out all unecessary lines
            var strippedList = new List<string>();

            // Iterate over rawLineItemList
            // Make a new list containing anything contained between "HEADER FOR CHUNK" and "HEADER FOR CHUNK":
            for (var h = 0; h < rawCount; h++)
            {
                // If we encounter HEADER FOR CHUNK
                if (!rawLineItemList[h].Contains(headerName))
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
                        strippedList.Add(rawLineItemList[h]);

                        var nextIteration = h + 1;

                        // If the next loop iteration would not be larger than the bounds of the list we are 
                        //   looping for
                        if (nextIteration < rawCount)
                        {
                            // Set header to the value of the next loop iteration (to detect if it contains 
                            //   HEADER FOR CHUNK)
                            header = rawLineItemList[nextIteration];
                        }
                        else // If the loop iteration would be out of bounds
                        {
                            // Set header so we exit the loop cleanly
                            header = headerName;
                        }
                    }
                    while (!header.Contains(headerName));
                }
            }

            // Break the stripped list into chunks
            List<List<string>> lineItemChunks = strippedList.SplitList(tokensPerLineItem);

            // Loop over all chunks
            for (var i = 0; i < lineItemChunks.Count; i++)
            {
                //Loop through each individual chunk
                for (var j = 0; j < tokensPerLineItem; j++)
                {


                    // Business logic goes here


                    // Here is an example of how to access each key/value pair for each loop iteration 
                    var kvp = lineItemChunks[i][j].Split(": ", 2);
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


            
            return Ok("hi");
        }
    }
}
