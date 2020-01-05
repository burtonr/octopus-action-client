namespace OctoClient.Models
{
    public class ActionResult<T>
    {
        public OctoClientErrors Error { get; set; } = new OctoClientErrors();
        public T Result { get; set; }

        public bool HasErrors()
        {
            return Error.ErrorCount != 0;
        }
    }
}