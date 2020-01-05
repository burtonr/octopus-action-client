using System;
using OctoClient.Models;

namespace OctoClient
{
    public static class ActionsHelp
    {
        public static ActionResult<Inputs> ReadInputValues()
        {
            var inputResult = new ActionResult<Inputs>();
            inputResult.Result = new Inputs
            {
                OctopusURL = Environment.GetEnvironmentVariable("INPUT_OCTOPUS_URL"),
                OctopusApiKey = Environment.GetEnvironmentVariable("INPUT_API_KEY"),
                ProjectName = Environment.GetEnvironmentVariable("INPUT_PROJECT_NAME"),
                ReleaseVersion = Environment.GetEnvironmentVariable("INPUT_RELEASE_VERSION"),
                ReleaseId = Environment.GetEnvironmentVariable("INPUT_RELEASE_ID"),
                EnvironmentName = Environment.GetEnvironmentVariable("INPUT_ENVIRONMENT_NAME")
            };
            
            var space = Environment.GetEnvironmentVariable("INPUT_SPACE_NAME");
            if (!string.IsNullOrWhiteSpace(space))
            {
                inputResult.Result.SpaceName = space;
            }

            inputResult.Error = inputResult.Result.ValidateCommonInputs();

            return inputResult;
        }

        public static void WriteOutput(OctoRelease release)
        {
            Console.WriteLine($"::set-output name=release_id::{release.ID}");
        }

        public static void WriteOutput(OctoDeployment deployment)
        {
            Console.WriteLine($"::set-output name=deployment_id::{deployment.ID}");
        }

        public static void WriteOutput(OctoClientErrors errors)
        {
            foreach (var err in errors.ErrorMessages)
            {
                Console.WriteLine($"::error:: {err}");
            }
        }
    }
}