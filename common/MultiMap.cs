// MultiMap.cs

using System;

//---------------------------------------------------------------------------//

namespace System.Collections.Generic
{
    public sealed class MultiMap<Key, Value>
    {
        private Dictionary<Key, List<Value>> d;            

        public MultiMap()
        {
            d = new Dictionary<Key, List<Value>>();
        }

        public List<Value> this[Key k]
        {
            get {  return d.ContainsKey(k) ? d[k] : null; }
        }

        public void Add(Key k, Value v)
        {
            if ( d.ContainsKey(k) )
            {
                d[k].Add(v);
            }
            else
            {
                var l = new List<Value>();
                l.Add(v);
                d.Add(k, l);
            }
        }

        public bool Remove(Key k)
        {
            if ( d.ContainsKey(k) )
            {
                return d.Remove(k);
            }
            else
            {
                return false;
            }
        }

        public bool Remove(Key k, Value v, bool rm_all = false)
        {
            if ( ! d.ContainsKey(k) )
            {
                return false;
            }
            else
            {
                var l = d[k];

                if ( ! rm_all )
                {
                    return l.Contains(v) ? l.Remove(v) : false;
                }
                else
                {
                    bool ret = false;
                    while ( l.Contains(v) )
                    {
                        if ( l.Remove(v) )
                        {
                            ret = true;
                        }
                    }
                    return ret;
                }
            }
        }

        public bool ContainsKey(Key k)
        {
            return d.ContainsKey(k);
        }

        public bool Contains(Key k, Value v)
        {
            var l = this[k];

            if ( null == l )
            {
                return false;
            }
            else
            {
                return l.Contains(v);
            }
        }

        public IEnumerator<KeyValuePair<Key, List<Value>>> GetEnumerator()
        {
            foreach (KeyValuePair<Key, List<Value>> pair in d)
            {
                yield return pair;
            }
        }

        public Dictionary<Key, List<Value>>.KeyCollection Keys
        {
            get { return d.Keys; }
        }

        public Dictionary<Key, List<Value>>.ValueCollection Values
        {
            get { return d.Values; }
        }

        public void Clear()
        {
            d.Clear();
        }

        public int Count
        {
            get { return d.Count; }
        }
    }
}

//---------------------------------------------------------------------------//

// MultiMap.cs