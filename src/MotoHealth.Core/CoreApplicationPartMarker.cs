using System.Reflection;

namespace MotoHealth.Core
{
    public class CoreApplicationPartMarker
    {
        public static Assembly Assembly => typeof(CoreApplicationPartMarker).Assembly;
    }
}