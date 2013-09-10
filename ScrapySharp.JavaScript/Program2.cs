using System;
using System.IO;
using ScrapySharp.JavaScript.Dom;
using smnetjs;

namespace ScrapySharp.JavaScript
{
    class Program2
    {
        public static SMRuntime Runtime = new SMRuntime();

        public static void Main()
        {
            Runtime.Embed(typeof(Script));
            Runtime.Embed(typeof(Document));
            Runtime.Embed(typeof(Window));
            
            var smScript = Runtime.InitScript("eval", typeof(Program2));
            
            smScript.SetOperationTimeout(30000U);
            Runtime.OnScriptError += (script, report) =>
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{3} line {0}: {1}\n {2}", report.LineNumber, report.LineSource, report.Message, report.Filename);
                Console.ResetColor();
            };

            var document = new Document();
            var window = new Window(document);
            
            var initMocksSource = File.ReadAllText("EmbeddedScripts/InitMockings.js");
            smScript.Eval(initMocksSource);
            
            smScript.CallFunction("__InitWindow", window);


            document.LoadHtml(File.ReadAllText("EmbeddedScripts/Html1.html"));

            //smScript.CallFunction("__LoadHtml", File.ReadAllText("EmbeddedScripts/Html1.html"));

            smScript.Eval(File.ReadAllText("EmbeddedScripts/jquery-1.9.1.js"));
            //smScript.Eval(File.ReadAllText("EmbeddedScripts/jquery-1.9.1.min.js"));
            

            //var source = File.ReadAllText("EmbeddedScripts/JavaScript1.js");
            //smScript.Eval(source);

            document.ExecuteScripts(smScript);

            smScript.Eval(File.ReadAllText("EmbeddedScripts/JavaScript2.js"));

            File.WriteAllText("out.html", document.GetOuterHtml());


            smScript.GarbageCollect();
            //smScript.Dispose();
            
            Console.WriteLine("Press any key ...");
            Console.ReadKey(true);
        }

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