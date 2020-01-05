using System;
using System.Linq;
using OctoClient.Models;

namespace OctoClient.Helpers
{
    public static class OctopusErrorHandler
    {
        public static void ParseException(this OctoClientErrors errors, Exception ex)
        {
            var exMsg = ex.Message;
            var lines = exMsg.Split(Environment.NewLine).ToList();
            var output = lines.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            errors.ErrorMessages.AddRange(output);

            return;
        }
    }
}