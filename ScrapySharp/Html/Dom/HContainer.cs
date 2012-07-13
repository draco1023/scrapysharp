using System.Collections.Generic;

namespace ScrapySharp.Html.Dom
{
    public abstract class HContainer
    {
        protected HContainer()
        {
            Children = new List<HElement>();
        }

        public List<HElement> Children { get; set; }

        
    }
}