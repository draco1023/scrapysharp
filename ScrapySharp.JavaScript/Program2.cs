using System;
using System.IO;
using ScrapySharp.JavaScript.Dom;
using ScrapySharp.Network;
using smnetjs;
using System.Linq;

namespace ScrapySharp.JavaScript
{
    class Program2
    {
        public static SMRuntime Runtime = new SMRuntime();

        public static void Main()
        {

            //var url = new Uri("https://www.google.fr/?q=sa#q=sa");
            var url = new Uri("http://fr.wikipedia.org/wiki/Soci%C3%A9t%C3%A9_anonyme");

            var document = Navigate(url);

            File.WriteAllText("out3.html", document.GetOuterHtml());


            Console.WriteLine("Press any key ...");
            Console.ReadKey(true);
        }

        private static Document Navigate(Uri url)
        {
            var browser = new ScrapingBrowser {AutoDownloadPagesResources = true};

            var homePage = browser.NavigateToPage(url, HttpVerb.Get, string.Empty);
            var scriptResources = homePage.Resources.Where(r => r.IsScript).ToArray();

            Runtime.Embed(typeof (Script));
            Runtime.Embed(typeof (Document));
            Runtime.Embed(typeof (Window));

            var smScript = Runtime.InitScript("eval", typeof (GlobalObject));

            smScript.SetOperationTimeout(30000U);
            Runtime.OnScriptError += (script, report) =>
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("{3} line {0}: {1}\n {2}", report.LineNumber, report.LineSource, report.Message,
                                      report.Filename);
                    Console.ResetColor();
                };

            var initMocksSource = File.ReadAllText("EmbeddedScripts/InitMockings.js");
            smScript.Eval(initMocksSource);
            var document = new Document();
            var window = new Window(document, smScript);

            document.Location = url.ToString();
            smScript.SetGlobalProperty("location", document.Location);

            smScript.CallFunction("__InitWindow", window);

            document.LoadHtml(homePage.Content);

            foreach (var scriptResource in scriptResources)
            {
                document.ExecuteScript(smScript, scriptResource.Content);
            }

            document.ExecuteScripts(smScript);
            smScript.GarbageCollect();
            return document;
        }

        public static void Main1()
        {
            Runtime.Embed(typeof(Script));
            Runtime.Embed(typeof(Document));
            Runtime.Embed(typeof(Window));
            Runtime.Embed(typeof(DynamicJsObject));

            var smScript = Runtime.InitScript("eval", typeof(GlobalObject));
            
            smScript.SetOperationTimeout(30000U);
            Runtime.OnScriptError += (script, report) =>
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{3} line {0}: {1}\n {2}", report.LineNumber, report.LineSource, report.Message, report.Filename);
                Console.ResetColor();
            };

            
            var initMocksSource = File.ReadAllText("EmbeddedScripts/InitMockings.js");
            smScript.Eval(initMocksSource);


            var document = new Document();
            var window = new Window(document, smScript);

            smScript.CallFunction("__InitWindow", window);

            //var jsObject = new DynamicJsObject();
            //smScript.CallFunction("__InitBag", jsObject);


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

    }
}