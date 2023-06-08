

namespace DLLTest.Repository.Factory
{
    internal class RepositoryFactory
    {
        internal static T Instance<T>() => (T)Activator.CreateInstance(typeof(T), ContextSingleton.Context);
    }
}
