using System;
using smnetjs;

namespace ScrapySharp.JavaScript
{
    [SMEmbedded(AccessibleName = "GlobalObject", Name = "global")]
    public static class GlobalObject
    {

        [SMMethod(Name = "print")]
        public static void Print(string text)
        {
            Console.WriteLine(text);
        }

        [SMMethod(Name = "alert")]
        public static void Alert(string text)
        {
            Console.WriteLine(text);
        }
    }
}