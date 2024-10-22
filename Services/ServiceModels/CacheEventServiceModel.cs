using Data.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public class CacheEvent<T>
    {
        public string Key { get; set; }
        public IMemoryCache Cache { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public Func<IServiceScope, Task<T>> FetchUpdatedData { get; set; }
    }

    public class DataCreatedEvent<T> : CacheEvent<T> { }
    public class DataUpdatedEvent<T> : CacheEvent<T> { }
    public class DataDeletedEvent : CacheEvent<object> { }
    public class CacheListEvent<T> : CacheEvent<List<T>> { }
    public class DataListCreatedEvent<T> : CacheListEvent<T> { }
    public class DataListUpdatedEvent<T> : CacheListEvent<T> { }

}
