module Browser
    
    open System
    open System.Text
    open System.Net
    open System.Globalization
    open System.Collections.Generic
    open System.IO
    open HtmlAgilityPack

    open Collections

    let toHtmlNode content =
        let document = new HtmlDocument()
        document.LoadHtml(content)
        document.DocumentNode

    let getAttributeValue (name:string) (node:HtmlNode) =
        node.GetAttributeValue(name, "")

    type IWebProxy =
        abstract GetProxy : Uri -> Uri
        abstract IsBypassed : Uri -> bool

    type RequestCacheLevel =
        | BypassCache
        | CacheIfAvailable
        | CacheOnly
        | Default
        | NoCacheNoStore
        | Reload
        | Revalidate

    type HttpVerb = 
        | Get
        | Post
        | Put
        | Delete

    type FakeUserAgent(name:string, userAgent:string) = 
        member x.Name with get () = name
        member x.UserAgent with get () = userAgent

    type RawRequest(verb:string, url:Uri, httpVersion:Version, headers:List<KeyValuePair<string, string>>, body:byte array, encoding:Encoding) =
        member x.Verb with get () = verb
        member x.Url with get () = url
        member x.HttpVersion with get () = httpVersion
        member x.Headers with get () = headers
        member x.Body with get () = body
        member x.Encoding with get () = encoding
        override x.ToString() =
            let builder = new StringBuilder()
            builder.AppendFormat("{0} {1} HTTP/{2}.{3}\r\n", verb, url, httpVersion.Major, httpVersion.Minor) |> ignore
            for header in headers do
                builder.AppendFormat("{0}: {1}\r\n", header.Key, header.Value)  |> ignore
            builder.AppendFormat("\r\n")  |> ignore
            if body = null |> not && body.Length > 0 then
                builder.AppendFormat("{0}\r\n", Encoding.UTF8.GetString(body, 0, body.Length))  |> ignore
            builder.ToString()
        
    type RawResponse(httpVersion:Version, statusCode:HttpStatusCode, statusDescription:string, headers:List<KeyValuePair<string, string>>, body:byte array, encoding:Encoding) =
        member x.HttpVersion with get () = httpVersion
        member x.StatusCode with get () = statusCode
        member x.StatusDescription with get () = statusDescription
        member x.Headers with get () = headers
        member x.Body with get () = body
        member x.Encoding with get () = encoding
        override x.ToString() =
            let builder = new StringBuilder()
            builder.AppendFormat("HTTP/{0}.{1} {2} {3}\r\n", httpVersion.Major, httpVersion.Minor, statusCode, statusDescription) |> ignore
            for header in headers do
                builder.AppendFormat("{0}: {1}\r\n", header.Key, header.Value)  |> ignore
            builder.AppendFormat("\r\n") |> ignore
            if body = null |> not && body.Length > 0 then
                builder.AppendFormat("{0}\r\n", Encoding.UTF8.GetString(body, 0, body.Length))  |> ignore
            builder.ToString()

    type WebResource(content:MemoryStream, lastModified:string, absoluteUrl:Uri, forceDownload:bool, contentType:string) =
        interface IDisposable with
            member x.Dispose() =
                content.Dispose()
        member x.Content with get () = content
        member x.LastModified with get () = lastModified
        member x.AbsoluteUrl with get () = absoluteUrl
        member x.ForceDownload with get () = forceDownload
        member x.ContentType with get () = contentType
        member x.GetTextContent() =
            content.Position <- 0L
            use reader = new StreamReader(content)
            reader.ReadToEnd()

    type WebPage(rawRequest:RawRequest, rawResponse:RawResponse) =
        let resourceTags = [("img", "src")
                            ("script", "src");
                            ("link", "href")] |> dict
        let mutable html:HtmlNode = null
        let mutable autoDetectCharsetEncoding = false

        let loadHtml () =
            html <- rawRequest.Encoding.GetString(rawRequest.Body,0,rawRequest.Body.Length) |> toHtmlNode
            let getCharset () = 
                let cs = html.Descendants("meta") 
                                |> Seq.map(fun meta -> meta.GetAttributeValue("charset", "").Trim())
                                |> Seq.filter(fun v -> not(String.IsNullOrEmpty(v)))
                                |> Seq.tryHead
                match cs with
                | Some charset -> 
                    let ct = html.Descendants("meta")
                                        |> Seq.filter (fun m -> m.GetAttributeValue("http-equiv", "") = "content-type")
                                        |> Seq.tryHead
                    match ct with
                    | Some contentType ->
                        let contentTypeContent = contentType.GetAttributeValue("content", "")
                        let posContentType = contentTypeContent.IndexOf("charset=", StringComparison.Ordinal)
                        if posContentType = -1 |> not then
                            Some (contentTypeContent.Substring(posContentType + "charset=".Length))
                        else
                            Some charset
                    | None -> None
                | None -> None

            if autoDetectCharsetEncoding = true then
                match getCharset() with
                | Some charset -> 
                    html <- Encoding.GetEncoding(charset).GetString(rawRequest.Body,0,rawRequest.Body.Length) |> toHtmlNode
                | None -> ()

        member val AbsoluteUrl:Uri = null with get,set
        member x.RawRequest with get () = rawRequest
        member x.RawResponse with get () = rawResponse
        member x.AutoDetectCharsetEncoding 
            with get () = autoDetectCharsetEncoding
            and set (v) = autoDetectCharsetEncoding <- v
        member val Content = "" with get,set
        member val Resources:WebResource list = List.empty with get,set
        member val Html:HtmlNode = null with get,set
        member val BaseUrl = "" with get,set

    type IHttpRequestSender =
        abstract member Get : Uri -> string
        abstract member Post : Uri -> string
        abstract member Put : Uri -> string
        abstract member Delete : Uri -> string

        abstract member GetAsync : Uri -> Async<string>
        abstract member PostAsync : Uri -> unit
        abstract member PutAsync : Uri -> unit
        abstract member DeleteAsync : Uri -> unit

    type IScrapingBrowser =
        abstract member ClearCookies : unit -> unit
        abstract member Proxy : IWebProxy with get,set
        abstract member CachePolicy : RequestCacheLevel with get,set
        abstract member AllowMetaRedirect : bool with get,set
        abstract member AutoDownloadPagesResources : bool with get,set
        abstract member TransferEncoding : string with get,set
        abstract member AllowAutoRedirect : bool with get,set
        abstract member UseDefaultCookiesParser : bool with get,set
        abstract member IgnoreCookies : bool with get,set
        abstract member Timeout : TimeSpan with get,set
        abstract member Language : CultureInfo with get,set
        abstract member ProtocolVersion : Version with get,set
        abstract member KeepAlive : bool with get,set
        abstract member Referer : Uri with get
        abstract member GetCookie : Uri -> string -> Cookie

//        abstract member NavigateTo : (Uri, HttpVerb, string) -> string
//        abstract member NavigateTo : (Uri, HttpVerb, NameValueCollection) -> string

        abstract member Send : NameValueCollection -> IHttpRequestSender
        abstract member Send : string -> IHttpRequestSender


