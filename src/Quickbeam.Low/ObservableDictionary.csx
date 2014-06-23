// Copyright (c) 2013, Chad Zawistowski
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

/// <summary>
/// See http://stackoverflow.com/a/14410196/1628916
/// </summary>
public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public ObservableDictionary() : base() { }
    public ObservableDictionary(int capacity) : base(capacity) { }
    public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
    public ObservableDictionary(int capacity,
                                IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary,
                                IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

    public event NotifyCollectionChangedEventHandler CollectionChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    public new TValue this[TKey key]
    {
        get
        {
            return base[key];
        }
        set
        {
            TValue oldValue;
            var exists = TryGetValue(key, out oldValue);
            var oldItem = new KeyValuePair<TKey, TValue>(key, oldValue);
            var newItem = new KeyValuePair<TKey, TValue>(key, value);
            
            base[key] = value;

            if (exists)
            {
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        newItem,
                        oldItem,
                        Keys.ToList().IndexOf(key)));
            }
            else
            {
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        newItem,
                        Keys.ToList().IndexOf(key)));

                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            }
        }
    }

    public new void Add(TKey key, TValue value)
    {
        if (ContainsKey(key)) return;
        var item = new KeyValuePair<TKey, TValue>(key, value);
        base.Add(key, value);

        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                item,
                Keys.ToList().IndexOf(key)));

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
    }

    public new bool Remove(TKey key)
    {
        TValue value;
        if (!TryGetValue(key, out value)) return false;

        var item = new KeyValuePair<TKey, TValue>(key, base[key]);
        var result = base.Remove(key);

        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                item,
                Keys.ToList().IndexOf(key)));

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        return result;
    }

    public new void Clear()
    {
        base.Clear();
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (CollectionChanged != null) { CollectionChanged(this, e); }
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (PropertyChanged != null) { PropertyChanged(this, e); }
    }
}

var x = new ObservableDictionary<string, string>();
x["hey"] = "yo";
Console.WriteLine(x["hey"]);
