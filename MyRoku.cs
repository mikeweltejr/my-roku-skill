using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MyRokuSkill
{
    public class MyRoku
    {
        public async Task<SkillResponse> Handle(SkillRequest request)
        {
            Console.WriteLine("WORKED");
            // TODO - create factory for different request types
            // TODO - create methods for each type to handle the types
            // TODO - have intents for different rokus in the house
            // TODO - connect to roku API
            // TODO - add different commands that can be done through the skill
            Console.WriteLine(request.GetRequestType());
            if (request.GetRequestType() == typeof(LaunchRequest)) return HandleLaunchRequest(request);
            if (request.GetRequestType() == typeof(IntentRequest)) return await HandleRemoteIntent(request);
            if (request.GetRequestType() == typeof(SessionEndedRequest))
            {
                return new SkillResponse {
                    Response= new ResponseBody
                    {
                        OutputSpeech= new PlainTextOutputSpeech("All done!")
                    },
                    Version = "0.1"
                };
            }

            return new SkillResponse();
        }

        public SkillResponse HandleLaunchRequest(SkillRequest request)
        {
            var responseBody = new ResponseBody();
            responseBody.ShouldEndSession = false;
            responseBody.OutputSpeech = new PlainTextOutputSpeech("Thanks for launching my roku skill!");
            return new SkillResponse { Response=responseBody, Version="0.1" };
        }

        public async Task<SkillResponse> HandleRemoteIntent(SkillRequest request)
        {
            var req = request.Request as IntentRequest;

            if (req.Intent.Name == "RemoteIntent")
            {
                Console.WriteLine("REMOTE INTENT BABY");
                var keyVal = req.Intent.Slots["Key"];
                Console.WriteLine(keyVal.Value);

                var client = new RestClient("http://192.168.86.192:8060");
                var restRequest = new RestRequest($"keypress/{keyVal.Value}");
                var response = await client.ExecutePostTaskAsync(restRequest);

                Console.WriteLine(response.ErrorMessage);
                Console.WriteLine(response.StatusCode);

                return new SkillResponse {
                    Response= new ResponseBody
                    {
                        OutputSpeech= new PlainTextOutputSpeech("Mike you boss, it is done.")
                    },
                    Version = "0.1"
                };
            }

            return new SkillResponse {
                Response= new ResponseBody
                {
                    OutputSpeech= new PlainTextOutputSpeech("Sorry no intent found")
                },
                Version = "0.1"
            };
        }
    }
}
