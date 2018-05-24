using System;
using System.Reflection;

namespace CommitTemplateInferrer.Classes
{
    public class Wrapper<T> : IDisposable
    {
        public readonly T Object;

        public Wrapper(T @object)
        {
            Object = @object;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TF GetField<TF>(string name)
        {
            return (TF)typeof(T).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Object);
        }

        public void InvokeEvent(string name, params object[] parameters)
        {
            typeof(T).GetEvent(name, BindingFlags.Instance | BindingFlags.NonPublic)?.RaiseMethod
                .Invoke(Object, parameters);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
            {
                typeof(T).GetMethod("Dispose", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.Invoke(Object, new object[] { disposing });
            }
        }
    }
}