namespace ScrapySharp.Core

    open System
    open System.IO
    open System.Net
    open System.Runtime.Serialization.Formatters.Binary
    open System.Text
    open HtmlAgilityPack
    open System.Linq

    type FilterLevel = 
        | Root
        | Children
        | Descendants

    type CssSelectorExecutor(nodes:System.Collections.Generic.List<HtmlNode>, tokens:System.Collections.Generic.List<Token>) = 
        let mutable nodes = Array.toList(nodes.ToArray())
        let mutable tokens = Array.toList(tokens.ToArray())
        let mutable level = FilterLevel.Descendants
        
        member public x.GetElements() =
            let elements = x.selectElements()
            elements |> List.toArray

        member private x.selectElements() = 
            
            let getTargets (acc:List<HtmlNode>) = 
                if level = FilterLevel.Children then
                    acc |> List.map (fun x -> x.ChildNodes) |> Seq.collect (fun x -> x)
                elif level = FilterLevel.Descendants then
                    acc |> List.map (fun x -> x.Descendants()) |> Seq.collect (fun x -> x)
                else
                    acc |> Seq.ofList

            let rec selectElements' (acc:List<HtmlNode>) source =
                match source with
                | Token.TagName(o, name) :: t -> 
                    let selectedNodes = acc |> getTargets |> Seq.filter(fun x -> x.Name = name) |> Seq.toList
                    level <- FilterLevel.Root
                    selectElements' selectedNodes t
                
                | Token.ClassPrefix(o) :: Token.CssClass(o2, className) :: t -> 
                    let selectedNodes = acc |> getTargets 
                                        |> Seq.filter (fun x -> x.GetAttributeValue("class", String.Empty).Split([|' '; '\t'; '\r'; '\n'|]).Contains(className))
                                        |> Seq.toList
                    level <- FilterLevel.Root
                    selectElements' selectedNodes t

                | Token.IdPrefix(o) :: Token.CssId(o2, id) :: t ->
                    let selectedNodes = acc |> getTargets 
                                        |> Seq.filter (fun x -> x.Id = id)
                                        |> Seq.toList
                    level <- FilterLevel.Root
                    selectElements' selectedNodes t

                | Token.OpenAttribute(o) :: Token.AttributeName(o1, name) :: Token.Assign(o2) :: Token.AttributeValue(o3, value) :: Token.CloseAttribute(o4) :: t ->
                    let selectedNodes = acc |> getTargets 
                                        |> Seq.filter (fun x -> x.GetAttributeValue(name, String.Empty) = value)
                                        |> Seq.toList
                    level <- FilterLevel.Root
                    selectElements' selectedNodes t

                | Token.OpenAttribute(o) :: Token.AttributeName(o1, name) :: Token.EndWith(o2) :: Token.AttributeValue(o3, value) :: Token.CloseAttribute(o4) :: t ->
                    let selectedNodes = acc |> getTargets 
                                        |> Seq.filter (fun x -> x.GetAttributeValue(name, String.Empty).EndsWith(value))
                                        |> Seq.toList
                    level <- FilterLevel.Root
                    selectElements' selectedNodes t

                | Token.OpenAttribute(o) :: Token.AttributeName(o1, name) :: Token.StartWith(o2) :: Token.AttributeValue(o3, value) :: Token.CloseAttribute(o4) :: t ->
                    let selectedNodes = acc |> getTargets 
                                        |> Seq.filter (fun x -> x.GetAttributeValue(name, String.Empty).StartsWith(value))
                                        |> Seq.toList
                    level <- FilterLevel.Root
                    selectElements' selectedNodes t

                | Token.AllChildren(o) :: t -> 
                    level <- FilterLevel.Descendants
                    selectElements' acc t

                | Token.DirectChildren(o) :: t -> 
                    level <- FilterLevel.Children
                    selectElements' acc t

                | [] -> acc
                | _ :: t -> failwith "Invalid token"

            selectElements' nodes tokens


