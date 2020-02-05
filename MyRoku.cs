using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.SQS;
using MyRokuSkill.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using static MyRokuSkill.Models.Roku;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MyRokuSkill
{
    public class MyRoku
    {
        public async Task<SkillResponse> Handle(SkillRequest request)
        {
            // TODO - create factory for different request types
            // TODO - create methods for each type to handle the types
            // TODO - have intents for different rokus in the house
            // TODO - connect to roku API
            // TODO - add different commands that can be done through the skill
            if (request.GetRequestType() == typeof(LaunchRequest)) return HandleLaunchRequest(request);
            if (request.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = request.Request as IntentRequest;
                if (intentRequest.Intent.Name == "RemoteIntent") return await HandleRemoteIntent(request);
                if (intentRequest.Intent.Name == "LauncherIntent") return await HandleLauncherIntent(request);

                return new SkillResponse {
                    Response= new ResponseBody
                    {
                        OutputSpeech= new PlainTextOutputSpeech("Sorry no intent found")
                    },
                    Version = "0.1"
                };
            }
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

        public async Task<SkillResponse> HandleLauncherIntent(SkillRequest request)
        {
            var req = request.Request as IntentRequest;
            var appToLaunch = req.Intent.Slots["RokuApp"] != null ? req.Intent.Slots["RokuApp"].Value : "Netflix";
            var room = req.Intent.Slots["Room"] != null ? req.Intent.Slots["Room"].Value : "Living Room";

            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.RegionEndpoint = RegionEndpoint.USEast1;
            var sqsClient = new AmazonSQSClient(sqsConfig);

            var queue = await sqsClient.GetQueueUrlAsync("my-roku-skill-queue", default(CancellationToken));

            var roomType = RoomType.LivingRoom;

            switch (room.ToLowerInvariant())
            {
                case "living room":
                    roomType = RoomType.LivingRoom;
                    break;
                case "bedroom":
                    roomType = RoomType.Bedroom;
                    break;
            }

            var rokuCommand = new Roku(appToLaunch, roomType);
            var rokuCommandJson = JsonConvert.SerializeObject(rokuCommand);

            await sqsClient.SendMessageAsync(queue.QueueUrl, rokuCommandJson, default(CancellationToken));

            return new SkillResponse {
                Response= new ResponseBody
                {
                    OutputSpeech= new PlainTextOutputSpeech("Your Roku has obliged your request")
                },
                Version = "0.1"
            };
        }

        public async Task<SkillResponse> HandleRemoteIntent(SkillRequest request)
        {
            var req = request.Request as IntentRequest;

            var keyVal = req.Intent.Slots["Key"];
            var room = req.Intent.Slots["Room"] != null ? req.Intent.Slots["Room"].Value : "Living Room";
            var button = ButtonType.Home;

            var roomType = RoomType.LivingRoom;

            switch (room.ToLowerInvariant())
            {
                case "living room":
                    roomType = RoomType.LivingRoom;
                    break;
                case "bedroom":
                    roomType = RoomType.Bedroom;
                    break;
            }

            Console.WriteLine(keyVal.Value.ToLowerInvariant());

            switch (keyVal.Value.ToLowerInvariant())
            {
                case "up":
                    button = ButtonType.Up;
                    break;
                case "down":
                    button = ButtonType.Down;
                    break;
                case "left":
                    button = ButtonType.Left;
                    break;
                case "right":
                    button = ButtonType.Right;
                    break;
                case "back":
                    button = ButtonType.Back;
                    break;
                case "play":
                    button = ButtonType.Play;
                    break;
                case "rewind":
                    button = ButtonType.Rewind;
                    break;
                case "forward":
                    button = ButtonType.FastForward;
                    break;
                case "ok":
                    button = ButtonType.Select;
                    break;
                case "enter":
                    button = ButtonType.Select;
                    break;
                case "select":
                    button = ButtonType.Select;
                    break;
                case "mute":
                    button = ButtonType.Mute;
                    break;
                default:
                    button = ButtonType.Home;
                    break;
            }

            // Send to SQS Queue
            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.RegionEndpoint = RegionEndpoint.USEast1;
            var sqsClient = new AmazonSQSClient(sqsConfig);

            var queue = await sqsClient.GetQueueUrlAsync("my-roku-skill-queue", default(CancellationToken));

            var rokuCommand = new Roku(button, roomType);
            var rokuCommandJson = JsonConvert.SerializeObject(rokuCommand);

            await sqsClient.SendMessageAsync(queue.QueueUrl, rokuCommandJson, default(CancellationToken));

            return new SkillResponse {
                Response= new ResponseBody
                {
                    OutputSpeech= new PlainTextOutputSpeech("Your Roku has obliged your request")
                },
                Version = "0.1"
            };
        }
    }
}
