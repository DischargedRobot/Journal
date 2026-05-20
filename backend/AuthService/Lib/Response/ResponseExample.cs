namespace AuthService.ResponseExample;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ResponseExampleAttribute : Attribute
{
    public int HttpStatus { get; }

    public Type ExampleType { get; }

    public ResponseExampleAttribute(int httpStatus, Type exampleType)
    {
        HttpStatus = httpStatus;
        ExampleType = exampleType;
    }
}