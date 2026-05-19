using System.Diagnostics;

namespace AuthService.Lib
{
    public static class Tracing
    {
        public static ActivitySource ActivitySource(string serviceName) => new(serviceName);
    }
}
