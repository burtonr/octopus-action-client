using System;
using OctoClient.Helpers;
using OctoClient.Models;
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

        public ActionResult<ReleaseResource> BuildNewRelease()
        {
            CreateClient();
            var releaseResult = new ActionResult<ReleaseResource>();
            var projectResult = GetProjectResult(_inputs.ProjectName);

            releaseResult.Error = projectResult.Error;

            if (projectResult.Result == null)
            {
                return releaseResult;
            }

            releaseResult.Result = new ReleaseResource
            {
                ProjectId = projectResult.Result.Id,
                Version = _inputs.ReleaseVersion
            };

            var templateResult = GetTemplateResult(projectResult.Result);

            if (templateResult.HasErrors())
            {
                releaseResult.Error.ErrorCount += templateResult.Error.ErrorCount;
                releaseResult.Error.ErrorMessages.AddRange(templateResult.Error.ErrorMessages);
            }

            foreach (var package in templateResult.Result.Packages)
            {
                releaseResult.Result.SelectedPackages.Add(
                    new SelectedPackage
                    {
                        ActionName = package.ActionName,
                        PackageReferenceName = package.PackageReferenceName,
                        Version = _inputs.ReleaseVersion
                    }
                );
            }

            return releaseResult;
        }

        public ActionResult<DeploymentResource> BuildNewDeployment()
        {
            CreateClient();
            var deploymentResult = new ActionResult<DeploymentResource>();
            var projectResult = GetProjectResult(_inputs.ProjectName);
            var envResult = GetEnvironmentResult(_inputs.EnvironmentName);

            deploymentResult.Error.ErrorCount += projectResult.Error.ErrorCount;
            deploymentResult.Error.ErrorMessages.AddRange(projectResult.Error.ErrorMessages);
            deploymentResult.Error.ErrorCount += envResult.Error.ErrorCount;
            deploymentResult.Error.ErrorMessages.AddRange(envResult.Error.ErrorMessages);

            if (projectResult.HasErrors() || envResult.HasErrors())
            {
                return deploymentResult;
            }

            deploymentResult.Result = new DeploymentResource
            {
                ReleaseId = _inputs.ReleaseId,
                ProjectId = projectResult.Result.Id,
                EnvironmentId = envResult.Result.Id
            };

            return deploymentResult;
        }

        public ActionResult<OctoRelease> CreateReleaseResult()
        {
            CreateClient();
            var relResult = new ActionResult<OctoRelease>();

            var space = _client.ForSystem().Spaces.FindByName(_inputs.SpaceName);
            var newRelease = BuildNewRelease();
            relResult.Error = newRelease.Error;

            if (newRelease.Result == null)
            {
                return relResult;
            }

            var repo = new OctopusRepository(_client, RepositoryScope.ForSpace(space));

            try
            {
                var relResource = repo.Releases.Create(newRelease.Result);
                relResult.Result = relResource.ToOctoRelease();
            }
            catch (Exception ex)
            {
                relResult.Error.ErrorCount++;
                relResult.Error.ParseException(ex);
            }

            return relResult;
        }

        public ActionResult<OctoDeployment> CreateDeploymentResult()
        {
            CreateClient();
            var depResult = new ActionResult<OctoDeployment>();

            var space = _client.ForSystem().Spaces.FindByName(_inputs.SpaceName);
            var newDeploy = BuildNewDeployment();
            depResult.Error = newDeploy.Error;

            if (depResult.HasErrors())
            {
                return depResult;
            }

            var repo = new OctopusRepository(_client, RepositoryScope.ForSpace(space));
            
            try
            {
                var depResource = repo.Deployments.Create(newDeploy.Result);
                depResult.Result = depResource.ToOctoDeployment();
            }
            catch (Exception ex)
            {
                depResult.Error.ParseException(ex);
            }

            return depResult;
        }

        private ActionResult<ProjectResource> GetProjectResult(string projectName)
        {
            CreateClient();
            var result = new ActionResult<ProjectResource>();

            var repo = new OctopusRepository(_client);
            var project = repo.Projects.FindByName(_inputs.ProjectName);

            if (project == null)
            {
                result.Error.ErrorCount++;
                result.Error.ErrorMessages.Add($"Project {_inputs.ProjectName} was not found");
            }

            result.Result = project;
            return result;
        }

        private ActionResult<ReleaseTemplateResource> GetTemplateResult(Octopus.Client.Model.ProjectResource projectResource)
        {
            CreateClient();
            var result = new ActionResult<ReleaseTemplateResource>();

            var repo = new OctopusRepository(_client);
            var process = repo.DeploymentProcesses.Get(projectResource.DeploymentProcessId);

            if (process == null)
            {
                result.Error.ErrorCount++;
                result.Error.ErrorMessages.Add($"No Deployment Process was not found");
            }

            var channel = repo.Channels.FindByName(projectResource, _inputs.ChannelName);
            var template = repo.DeploymentProcesses.GetTemplate(process, channel);

            if (template == null)
            {
                result.Error.ErrorCount++;
                result.Error.ErrorMessages.Add($"Release Template for {_inputs.ChannelName} channel was not found");
            }

            result.Result = template;
            return result;
        }

        private ActionResult<EnvironmentResource> GetEnvironmentResult(string envName)
        {
            CreateClient();
            var result = new ActionResult<EnvironmentResource>();
            var repo = new OctopusRepository(_client);
            var environment = repo.Environments.FindByName(_inputs.EnvironmentName);

            if (environment == null)
            {
                result.Error.ErrorCount++;
                result.Error.ErrorMessages.Add($"Environment {_inputs.EnvironmentName} was not found");
            }

            result.Result = environment;
            return result;
        }
    }
}