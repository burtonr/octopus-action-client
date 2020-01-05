using System.Collections.Generic;

namespace OctoClient.Models
{
    public class OctoClientErrors
    {
        public int ErrorCount { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}