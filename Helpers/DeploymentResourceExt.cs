using OctoClient.Models;
using Octopus.Client.Model;

namespace OctoClient.Helpers
{
    public static class DeploymentResourceExt
    {
        public static OctoDeployment ToOctoDeployment(this DeploymentResource resource)
        {
            var deploy = new OctoDeployment();
            deploy.ID = resource.Id;

            return deploy;
        }
    }
}