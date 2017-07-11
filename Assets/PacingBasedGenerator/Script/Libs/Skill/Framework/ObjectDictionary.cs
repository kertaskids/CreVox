//  Copyright (c) 2012 Calvin Rien
//        http://the.darktable.com
// 
// This software is provided 'as-is', without any express or implied warranty. In
// no event will the authors be held liable for any damages arising from the use
// of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it freely,
// subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim
// that you wrote the original software. If you use this software in a product,
// an acknowledgment in the product documentation would be appreciated but is not
// required.
// 
// 2. Altered source versions must be plainly marked as such, and must not be
// misrepresented as being the original software.
// 
// 3. This notice may not be removed or altered from any source distribution.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Framework
{
    public class UnityNameValuePair<V> : UnityKeyValuePair<string, V>
    {
        public string name = null;

        override public string Key
        {
            get { return name; }
            set { name = value; }
        }

        public UnityNameValuePair() : base()
        {
        }

        public UnityNameValuePair(string key, V value) : base(key, value)
        {
        }
    }

    public class UnityKeyValuePair<K, V>
    {
        virtual public K Key
        {
            get;
            set;
        }

        virtual public V Value
        {
            get;
            set;
        }

        public UnityKeyValuePair()
        {
            Key = default(K);
            Value = default(V);
        }

        public UnityKeyValuePair(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }

    [System.Serializable]
    public sealed class ObjectKvp : UnityNameValuePair<string>
    {
        public string value = null;

        override public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public ObjectKvp(string key, string value) : base(key, value)
        {
        }
    }

    [System.Serializable]
    public class ObjectDictionary : UnityDictionary<string>
    {
        public List<ObjectKvp> values;

        override protected List<UnityKeyValuePair<string, string>> KeyValuePairs
        {
            get
            {
                if (values == null)
                {
                    values = new List<ObjectKvp>();
                }
                return values.ConvertAll<UnityKeyValuePair<string, string>>(new System.Converter<ObjectKvp, UnityKeyValuePair<string, string>>(
                    x =>
                    {
                        return x as UnityKeyValuePair<string, string>;
                    }));
            }
            set
            {
                if (value == null)
                {
                    values = new List<ObjectKvp>();
                    return;
                }

                values = value.ConvertAll<ObjectKvp>(new System.Converter<UnityKeyValuePair<string, string>, ObjectKvp>(
                    x =>
                    {
                        return new ObjectKvp(x.Key, x.Value as string);
                    }));
            }
        }

        override protected void SetKeyValuePair(string k, string v)
        {
            var index = values.FindIndex(x =>
            {
                return x.Key == k;
            });

            if (index != -1)
            {
                if (v == null)
                {
                    values.RemoveAt(index);
                    return;
                }

                values[index] = new ObjectKvp(k, v);
                return;
            }

            values.Add(new ObjectKvp(k, v));
        }
    }

    public abstract class UnityDictionary<V> : UnityDictionary<string, V>
    {

    }

    public abstract class UnityDictionary<K, V> : IDictionary<K, V>
    {
        abstract protected List<UnityKeyValuePair<K, V>> KeyValuePairs
        {
            get;
            set;
        }

        protected abstract void SetKeyValuePair(K k, V v); 

        virtual public V this[K key]
        {
            get
            {
                var result = KeyValuePairs.Find(x =>
                {
                    return x.Key.Equals(key);
                });

                if (result == null)
                {
                    return default(V);
                }

                return result.Value;
            }
            set
            {
                if (key == null)
                {
                    return;
                }

                SetKeyValuePair(key, value);
            }
        }

        #region IDictionary interface

        public void Add(K key, V value)
        {
            this[key] = value;
        }

        public void Add(KeyValuePair<K, V> kvp)
        {
            this[kvp.Key] = kvp.Value;
        }

        public bool TryGetValue(K key, out V value)
        {
            if (!this.ContainsKey(key))
            {
                value = default(V);
                return false;
            }

            value = this[key];
            return true;
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(K key)
        {
            var list = KeyValuePairs;

            var index = list.FindIndex(x =>
            {
                return x.Key.Equals(key);
            });

            if (index == -1)
            {
                return false;
            }

            list.RemoveAt(index);

            KeyValuePairs = list;

            return true;
        }

        public void Clear()
        {
            var list = KeyValuePairs;

            list.Clear();

            KeyValuePairs = list;
        }

        public bool ContainsKey(K key)
        {
            return KeyValuePairs.FindIndex(x =>
            {
                return x.Key.Equals(key);
            }) != -1;
        }

        public bool Contains(KeyValuePair<K, V> kvp)
        {
            return this[kvp.Key].Equals(kvp.Value);
        }

        public int Count
        {
            get
            {
                return KeyValuePairs.Count;
            }
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int index)
        {
            var copy = KeyValuePairs.ConvertAll<KeyValuePair<K, V>>(
                new System.Converter<UnityKeyValuePair<K, V>, KeyValuePair<K, V>>(
                x =>
                {
                    return new KeyValuePair<K, V>(x.Key, (V)x.Value);
                }));

            copy.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator() as IEnumerator;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return new UnityDictionaryEnumerator(this);
        }

        public ICollection<K> Keys
        {
            get
            {
                return KeyValuePairs.ConvertAll(new System.Converter<UnityKeyValuePair<K, V>, K>(x =>
                {
                    return x.Key;
                }));
            }
        }

        public ICollection<V> Values
        {
            get
            {
                return KeyValuePairs.ConvertAll(new System.Converter<UnityKeyValuePair<K, V>, V>(x =>
                {
                    return x.Value;
                }));
            }
        }

        public ICollection<KeyValuePair<K, V>> Items
        {
            get
            {
                return KeyValuePairs.ConvertAll<KeyValuePair<K, V>>(new System.Converter<UnityKeyValuePair<K, V>, KeyValuePair<K, V>>(x =>
                {
                    return new KeyValuePair<K, V>(x.Key, x.Value);
                }));
            }
        }

        public V SyncRoot
        {
            get { return default(V); }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        internal sealed class UnityDictionaryEnumerator : IEnumerator<KeyValuePair<K, V>>
        {
            // A copy of the SimpleDictionary T's key/value pairs.
            KeyValuePair<K, V>[] items;
            int index = -1;

            internal UnityDictionaryEnumerator()
            {
            }

            internal UnityDictionaryEnumerator(UnityDictionary<K, V> ud)
            {
                // Make a copy of the dictionary entries currently in the SimpleDictionary T.
                items = new KeyValuePair<K, V>[ud.Count];

                ud.CopyTo(items, 0);
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public KeyValuePair<K, V> Current
            {
                get
                {
                    ValidateIndex();
                    return items[index];
                }
            }

            // Return the current dictionary entry.
            public KeyValuePair<K, V> Entry
            {
                get { return (KeyValuePair<K, V>)Current; }
            }

            public void Dispose()
            {
                index = -1;
                items = null;
            }

            // Return the key of the current item.
            public K Key
            {
                get
                {
                    ValidateIndex();
                    return items[index].Key;
                }
            }

            // Return the value of the current item.
            public V Value
            {
                get
                {
                    ValidateIndex();
                    return items[index].Value;
                }
            }

            // Advance to the next item.
            public bool MoveNext()
            {
                if (index < items.Length - 1)
                {
                    index++;
                    return true;
                }
                return false;
            }

            // Validate the enumeration index and throw an exception if the index is out of range.
            private void ValidateIndex()
            {
                if (index < 0 || index >= items.Length)
                {
                    throw new System.InvalidOperationException("Enumerator is before or after the collection.");
                }
            }

            // Reset the index to restart the enumeration.
            public void Reset()
            {
                index = -1;
            }
            #endregion
        }
    }
    
}