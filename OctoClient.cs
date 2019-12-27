using System;
using Octopus.Client;
using Octopus.Client.Model;

namespace OctoClient
{
    public class OctoClient
    {
        private readonly Inputs _inputs;
        private OctopusClient _client;

        public OctoClient(Inputs inputParams)
        {
            _inputs = inputParams;
        }

        public void CreateClient()
        {
            if (_client != null)
            {
                return;
            }

            var endpoint = new OctopusServerEndpoint(_inputs.OctopusURL, _inputs.OctopusApiKey);
            var client = new OctopusClient(endpoint);
            _client = client;
            return;
        }

        public ReleaseResource BuildNewRelease()
        {
            CreateClient();
            var project = GetProjectOrThrow(_inputs.ProjectName);

            var release = new ReleaseResource
            {
                ProjectId = project.Id,
                Version = _inputs.ReleaseVersion
            };

            return release;
        }

        public DeploymentResource BuildNewDeployment()
        {
            CreateClient();            
            var project = GetProjectOrThrow(_inputs.ProjectName);
            var environment = GetEnvironmentOrThrow(_inputs.EnvironmentName);

            var deployment = new DeploymentResource
            {
                ReleaseId = _inputs.ReleaseId,
                ProjectId = project.Id,
                EnvironmentId = environment.Id
            };

            return deployment;
        }

        public void CreateRelease()
        {
            CreateClient();
            var space = _client.ForSystem().Spaces.FindByName(_inputs.SpaceName);

            var newRelease = BuildNewRelease();

            var repo = new OctopusRepository(_client, RepositoryScope.ForSpace(space));
            var release = repo.Releases.Create(newRelease);

            // release.Id

            return;
        }

        public void CreateDeployment()
        {
            CreateClient();
            var space = _client.ForSystem().Spaces.FindByName(_inputs.SpaceName);

            var newDeploy = BuildNewDeployment();

            var repo = new OctopusRepository(_client, RepositoryScope.ForSpace(space));
            var deployment = repo.Deployments.Create(newDeploy);

            // newDeploy.Id

            return;
        }

        private ProjectResource GetProjectOrThrow(string projectName)
        {
            CreateClient();
            var repo = new OctopusRepository(_client);
            var project = repo.Projects.FindByName(_inputs.ProjectName);

            if (project == null)
            {
                throw new Exception($"Project {_inputs.ProjectName} was not found");
            }

            return project;
        }

        private EnvironmentResource GetEnvironmentOrThrow(string envName)
        {
            CreateClient();
            var repo = new OctopusRepository(_client);
            var environment = repo.Environments.FindByName(_inputs.EnvironmentName);

            if (environment == null)
            {
                throw new Exception($"Environment {_inputs.EnvironmentName} was not found");
            }

            return environment;
        }
    }
}