using OctoClient.Models;
using Octopus.Client.Model;

namespace OctoClient.Helpers
{
    public static class ReleaseResourceExt
    {
        public static OctoRelease ToOctoRelease(this ReleaseResource resource)
        {
            var release = new OctoRelease();
            release.ID = resource.Id;
            return release;
        }
    }
}