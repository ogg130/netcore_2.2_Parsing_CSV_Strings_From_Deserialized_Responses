using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                           "LAYOUT: Alpha\nTEMPLATE: Model-1234\nID: 1\n" +
                           "LAYOUT: Beta\nTEMPLATE: Model-4321\nID: 2\n" +
                           "LAYOUT: Gamma\nTEMPLATE: Model-4444\nID: 3\n" +
                           "LAYOUT: Delta\nTEMPLATE: Model-1234\nID: 4\n" +
                           "LAYOUT: Epsilon\nTEMPLATE: Model-3333\nID: 5\n" +
                           "LAYOUT: Zeta\nTEMPLATE: Model-2222\nID: 5\n" +
                           "LAYOUT: Eta\nTEMPLATE: Model-1221\nID: 6\n" +
                           "LAYOUT: Theta\nTEMPLATE: Model-1001\nID: 7";

            // Split lineItems on the line break(/n) to create a list containing a string representing a key value pair
            var rawLineItemList = DeserializedApiResponse.lineItems.Split("\n").ToList();

            // Pull the first lineitem of the response into a string so that we can find tokens per lineitem
            var firstLine = DeserializedApiResponse.lineItems.Split(new string[] { "\nLAYOUT: " }, StringSplitOptions.None)[0];

            // Calculate tokens per lineitem to be used as a loop top end 
            var tokensPerLineItem = firstLine.Split("\n").Count();

            // Break the raw list into chunks
            List<List<string>> lineItemChunks = rawLineItemList.SplitList();
            
            // Loop over all chunks
            for (var i = 0; i < lineItemChunks.Count; i++)
            {
                //Loop through each individual chunk
                for (var j = 0; j < tokensPerLineItem; j++)
                {
                    // This inner loop is looping over each key value pair item in each chunk

                    // For example, the first chunk would iterated over like such:
                    // Iteration #1:    Key: LAYOUT         Value: Alpha
                    // Iteration #2:    Key: TEMPLATE       Value: Model-1234
                    // Iteration #3:    Key: ID             Value: 1

                    // Here is how you access each key/value pair for each loop iteration 
                    var key = lineItemChunks[i][j].Split(": ", 2)[0];
                    var value = lineItemChunks[i][j].Split(": ", 2)[1];

                    // Business logic goes here
                }
            }
            return Ok("hi");
        }
    }
}
