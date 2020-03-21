using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models
{
    public class Cacheable<T>
    {
        public Cacheable(Func<T> initalizer)
        {
            this.initializer = initializer;
        }

        private Func<T> initializer;
        private T? cached;

        public T GetOrCreate()
        {
            if (cached.HasValue)
                return cached;
            cached = initializer();
            return GetOrCreate();
        }
    }
}
