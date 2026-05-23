using System.Diagnostics;

namespace MainService.Lib
{
    public static class Tracing
    {
        public static ActivitySource ActivitySource(string serviceName) => new(serviceName);
    }
}
