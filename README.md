Welcome
ScrapySharp has a Web Client able to simulate a real Web browser (handle referrer, cookies …)
Html parsing has to be as natural as possible. So I like to use CSS Selectors and Linq.
This framework wraps HtmlAgilityPack.
Basic examples of CssSelect usages
```csharp
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

class Example
{
	public void Main()
	{
		var divs = html.CssSelect(“div”);  //all div elements
		var nodes = html.CssSelect(“div.content”); //all div elements with css class ‘content’
		var nodes = html.CssSelect(“div.widget.monthlist”); //all div elements with the both css class
		var nodes = html.CssSelect(“#postPaging”); //all HTML elements with the id postPaging
		var nodes = html.CssSelect(“div#postPaging.testClass”); // all HTML elements with the id postPaging and css class testClass

		var nodes = html.CssSelect(“div.content > p.para”); //p elements who are direct children of div elements with css class ‘content’
 
		var nodes = html.CssSelect(“input[type=text].login”); // textbox with css class login
	}
}
```

Scrapysharp can also simulate a web browser

```csharp
ScrapingBrowser browser = new ScrapingBrowser();

//set UseDefaultCookiesParser as false if a website returns invalid cookies format
//browser.UseDefaultCookiesParser = false;

WebPage homePage = browser.NavigateToPage(new Uri("http://www.bing.com/"));

PageWebForm form = homePage.FindFormById("sb_form");
form["q"] = "scrapysharp";
form.Method = HttpVerb.Get;
WebPage resultsPage = form.Submit();

HtmlNode[] resultsLinks = resultsPage.Html.CssSelect("div.sb_tlst h3 a").ToArray();

WebPage blogPage = resultsPage.FindLinks(By.Text("romcyber blog | Just another WordPress site")).Single().Click();
```

Install Scrapysharp in your project
It's easy to use Scrapysharp in your project.
A Nuget package exists (https://www.nuget.org/packages/ScrapySharp)
To install ScrapySharp, run the following command in the Package Manager Console
PM> Install-Package ScrapySharp
Have fun!
