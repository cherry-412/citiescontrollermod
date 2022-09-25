using System.Reflection;

namespace CitiesControllerMod.Helpers
{
    public static class ReflectionExtensions
    {
        public static T GetFieldValue<T>(this object obj, string name)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }

        public static void InvokeMethod(this object obj, string name)
        {
            MethodInfo mi = obj.GetType().GetMethod(name);
            mi.Invoke(obj, null);
        }

        public static void InvokePrivateMethod(this object obj, string name)
        {
            MethodInfo mi = obj.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(obj, null);
        }
    }
}
