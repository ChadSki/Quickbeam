using System.Collections.Generic;
using System.Collections.Specialized;

namespace Quickbeam.Low
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
