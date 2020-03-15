using System.Reflection;

namespace MotoHealth.Infrastructure
{
    public static class InfrastructureApplicationPartMarker
    {
        public static Assembly Assembly => typeof(InfrastructureApplicationPartMarker).Assembly;
    }
}