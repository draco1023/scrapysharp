using smnetjs;

namespace ScrapySharp.JavaScript.Dom
{
    public static class Script
    {
        [SMMethod(Name = "eval")]
        public static string Eval(string name, string code)
        {
            var script = Program2.Runtime.FindScript(name);
            if (script == null)
                return string.Empty;
            return script.Eval<string>(code);
        }

    }
}