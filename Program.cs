using System;

namespace OctoClient
{
    public class Inputs
    {
        public string OctopusURL { get; set; }
        public string OctopusApiKey { get; set; }
        public string SpaceName { get; set; } = "Default";
        public string ProjectName { get; set; }
        public string ReleaseVersion { get; set; }
        public string ReleaseId { get; set; }
        public string EnvironmentName { get; set; }

        private bool ValidCommonInputs()
        {
            if (string.IsNullOrWhiteSpace(OctopusURL))
            {
                Console.WriteLine("Octopus URL is missing and required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(OctopusApiKey))
            {
                Console.WriteLine("API Key is missing and required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                Console.WriteLine("ProjectName is missing and required");
                return false;
            }

            return true;
        }

        public bool IsValidForRelease()
        {
            if (!ValidCommonInputs())
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ReleaseVersion))
            {
                Console.WriteLine("ReleaseVersion is missing and required");
                return false;
            }

            return true;
        }

        public bool IsValidForDeploy()
        {
            // TODO: Implement
            if (!ValidCommonInputs())
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ReleaseId))
            {
                Console.WriteLine("ReleaseId is missing and required for deployment");
                return false;
            }

            if (string.IsNullOrWhiteSpace(EnvironmentName))
            {
                Console.WriteLine("EnvironmentName is missing and required for deployment");
                return false;
            }

            return true;
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine(@"You must supply the action to take as a single argument. Available actions: 
- release
- deploy
                ");
                return 1;
            }

            var inputs = readInputs();

            if (inputs == null)
            {
                return 1;
            }

            var octo = new OctoClient(inputs);

            switch (args[0].ToLower())
            {
                case "release":
                    Console.WriteLine("Creating release");
                    if (inputs.IsValidForRelease())
                    {
                        CreateRelease(octo);
                    }
                    break;
                case "deploy":
                    Console.WriteLine("Creating deployment");
                    if (inputs.IsValidForDeploy())
                    {
                        CreateDeployment(octo);
                    }
                    break;
                default:
                    Console.WriteLine("Action not yet supported");
                    break;
            }

            return 0;
        }

        static void CreateRelease(OctoClient client)
        {
            try
            {
                client.CreateRelease();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create Release Failed: {ex.Message}");
            }
        }

        static void CreateDeployment(OctoClient client)
        {
            try
            {
                client.CreateDeployment();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create Deployment Failed: {ex.Message}");
            }
        }

        static Inputs readInputs()
        {
            var result = new Inputs();

            result.OctopusURL = Environment.GetEnvironmentVariable("INPUT_OCTOPUS_URL");
            result.OctopusApiKey = Environment.GetEnvironmentVariable("INPUT_API_KEY");
            result.ProjectName = Environment.GetEnvironmentVariable("INPUT_PROJECT_NAME");
            result.ReleaseVersion = Environment.GetEnvironmentVariable("INPUT_RELEASE_VERSION");
            result.ReleaseId = Environment.GetEnvironmentVariable("INPUT_RELEASE_ID");
            result.EnvironmentName = Environment.GetEnvironmentVariable("INPUT_ENVIRONMENT_NAME");

            var space = Environment.GetEnvironmentVariable("INPUT_SPACE_NAME");
            if (!string.IsNullOrWhiteSpace(space))
            {
                result.SpaceName = space;
            }

            return result;
        }
    }
}
