using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ScrapySharp.Html.Dom
{
    public class HAttibutesCollection : IEnumerable<HAttribute>
    {
        private readonly List<HAttribute> attributes;

        public HAttibutesCollection()
        {
            attributes = new List<HAttribute>();
        }

        public void Add(string name, string value)
        {
            attributes.Add(new HAttribute(name, value));
        }

        public string this[string name]
        {
            get { return attributes.Where(a => a.Name == name).Select(a => a.Value).FirstOrDefault(); }
            set { Add(name, value); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<HAttribute> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        public int Count
        {
            get { return attributes.Count; }
        }

        public string[] AllKeys
        {
            get { return attributes.Select(a => a.Name).ToArray(); }
        }
    }
}