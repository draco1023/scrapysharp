namespace ScrapySharp.Core

    open System
    open System.IO
    open System.Net
    open System.Runtime.Serialization.Formatters.Binary
    open System.Text
    open HtmlAgilityPack
    open System.Linq


    type CssSelectorExecutor(nodes:System.Collections.Generic.List<HtmlNode>, tokens:System.Collections.Generic.List<Token>) = 
        let mutable nodes = Array.toList(nodes.ToArray())
        let mutable tokens = Array.toList(tokens.ToArray())
        
        member public x.GetElements() =
            let elements = x.selectElements()
            elements |> List.toArray

        member private x.selectElements() = 
            
            let rec selectElements' (acc:List<HtmlNode>) source =
                match source with
                | Token.TagName(o, name) :: t -> 
                    let selectedNodes = nodes |> List.map (fun x -> x.Descendants(name)) |> Seq.collect (fun x -> x) |> Seq.toList
                    selectElements' selectedNodes t
                
                | Token.ClassPrefix(o) :: Token.CssClass(o2, className) :: t -> 
                    
                    let selectedNodes = nodes |> List.map (fun x -> x.Descendants()) |> Seq.collect (fun x -> x) 
                                        |> Seq.filter (fun x -> x.GetAttributeValue("class", String.Empty).Trim() = className)
                                        |> Seq.toList

                    selectElements' selectedNodes t

                | [] -> List.rev acc
                | _ -> failwith "failed"

            selectElements' nodes tokens


