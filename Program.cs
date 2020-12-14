using System;

namespace OctoClient
{
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

            var inputs = ActionsHelp.ReadInputValues();

            if (inputs.HasErrors())
            {
                ActionsHelp.WriteOutput(inputs.Error);
                return 1;
            }

            var octo = new OctoClient(inputs.Result);

            switch (args[0].ToLower())
            {
                // TODO: Consider "pack" functionality. This comes from the Octopus CLI (dotnet tool)
                // Possibly add the OctoCLI to the image and just call it directly from the calling action?
                // Further reading: https://octopus.com/docs/packaging-applications/create-packages/octopus-cli
                // For dotnet Core, they suggest using `dotnet pack` which tells me that "pack"-ing is an external process
                // that should NOT be included in this tool
                case "push": // TODO: Implement https://octopus.com/docs/octopus-rest-api/examples/feeds/push-package-to-builtin-feed
                    Console.WriteLine("Not currently supported");
                    return 1;
                case "release":
                    inputs.Error = inputs.Result.ValidateForRelease();
                    if (inputs.HasErrors())
                    {
                        ActionsHelp.WriteOutput(inputs.Error);
                        return 1;
                    }
                    else
                    {
                        var release = octo.CreateReleaseResult();
                        if (release.HasErrors())
                        {
                            ActionsHelp.WriteOutput(release.Error);
                            return 1;
                        }

                        ActionsHelp.WriteOutput(release.Result);
                    }
                    break;
                case "deploy":
                    inputs.Error = inputs.Result.ValidateForDeploy();
                    if (inputs.HasErrors())
                    {
                        ActionsHelp.WriteOutput(inputs.Error);
                        return 1;
                    }
                    else
                    {
                        var deployment = octo.CreateDeploymentResult();
                        if (deployment.HasErrors())
                        {
                            ActionsHelp.WriteOutput(deployment.Error);
                            return 1;
                        }

                        ActionsHelp.WriteOutput(deployment.Result);
                    }
                    break;
                default:
                    inputs.Error.ErrorMessages.Add($"Action {args[0].ToLower()} not yet supported");
                    ActionsHelp.WriteOutput(inputs.Error);
                    return 1;
            }

            return 0;
        }
    }
}
