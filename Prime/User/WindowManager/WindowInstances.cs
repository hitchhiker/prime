using System;
using System.Collections.Generic;
using System.Linq;

namespace Prime.Core
{
    public class WindowInstances
    {
        private WindowInstances() {}

        public static WindowInstances I => Lazy.Value;
        private static readonly Lazy<WindowInstances> Lazy = new Lazy<WindowInstances>(()=>new WindowInstances());

        public void Save<T>(UserContext context, IEnumerable<T> instances) where T : WindowInstanceBase
        {
            var existing = Get<T>(context);

            foreach (var i in existing)
                i.Delete(context);

            foreach (var i in instances)
            {
                i.UserId = context.Id;
                i.Save(context);
            }
        }

        public List<T> Get<T>(UserContext context) where T : WindowInstanceBase
        {
            return context.As<T>().Where(x => x.UserId == context.Id).ToList();
        }
    }
}