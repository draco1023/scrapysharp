namespace ScrapySharp.Core

    open System
    open System.IO
    open System.Net
    open System.Runtime.Serialization.Formatters.Binary
    open System.Text

    type INavigationProvider<'t> = 
        abstract member ChildNodes : System.Collections.Generic.List<'t> -> System.Collections.Generic.List<'t>
        abstract member Descendants : System.Collections.Generic.List<'t> -> System.Collections.Generic.List<'t>
        abstract member ParentNodes : System.Collections.Generic.List<'t> -> System.Collections.Generic.List<'t>
        abstract member AncestorsAndSelf : System.Collections.Generic.List<'t> -> System.Collections.Generic.List<'t>

    type AgilityNavigationProvider() = 
        interface INavigationProvider<HtmlAgilityPack.HtmlNode> with
            member this.ChildNodes(nodes) = 
                let resutls = nodes |> Seq.map (fun x -> x.ChildNodes) |> Seq.collect (fun x -> x)
                new System.Collections.Generic.List<'t>(resutls)
            member this.Descendants(nodes) = 
                let resutls = nodes |> Seq.map (fun x -> x.Descendants()) |> Seq.collect (fun x -> x)
                new System.Collections.Generic.List<'t>(resutls)
            member this.ParentNodes(nodes) = 
                let results = nodes |> Seq.map (fun x -> x.ParentNode)
                new System.Collections.Generic.List<'t>(results)
            member this.AncestorsAndSelf(nodes) = 
                let results = nodes |> Seq.map (fun x -> x.AncestorsAndSelf()) |> Seq.collect (fun x -> x)
                new System.Collections.Generic.List<'t>(results)

