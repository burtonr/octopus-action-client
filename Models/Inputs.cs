namespace OctoClient.Models
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

        public OctoClientErrors ValidateCommonInputs()
        {
            var errors = new OctoClientErrors();

            if (string.IsNullOrWhiteSpace(OctopusURL))
            {
                errors.ErrorCount++;
                errors.ErrorMessages.Add("Octopus URL is missing and required");
            }

            if (string.IsNullOrWhiteSpace(OctopusApiKey))
            {
                errors.ErrorCount++;
                errors.ErrorMessages.Add("API Key is missing and required");
            }

            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                errors.ErrorCount++;
                errors.ErrorMessages.Add("ProjectName is missing and required");
            }

            return errors;
        }

        public OctoClientErrors ValidateForRelease()
        {
            var errors = ValidateCommonInputs();

            if (string.IsNullOrWhiteSpace(ReleaseVersion))
            {
                errors.ErrorCount++;
                errors.ErrorMessages.Add("ReleaseVersion is missing and required");
            }

            return errors;
        }

        public OctoClientErrors ValidateForDeploy()
        {
            var errors = ValidateCommonInputs();

            if (string.IsNullOrWhiteSpace(ReleaseId))
            {
                errors.ErrorCount++;
                errors.ErrorMessages.Add("ReleaseId is missing and required for deployment");
            }

            if (string.IsNullOrWhiteSpace(EnvironmentName))
            {
                errors.ErrorCount++;
                errors.ErrorMessages.Add("EnvironmentName is missing and required for deployment");
            }

            return errors;
        }
    }
}