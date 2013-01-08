namespace ScrapySharp.Core

    open System
    open System.IO
    open System.Net
    open System.Runtime.Serialization.Formatters.Binary
    open System.Text

    type Attribute(name:string, value:string) = 
        member t.Name = name
        member t.Value = value

    type Tag(name:string, innerText:string, attributes:List<Attribute>) =
        member t.Name = name
        member t.InnerText = innerText
        member t.Attributes = attributes

    type FastHtmlParser(source: string) =
        let mutable source = source
        let mutable position = 0

        let decisiveChars = [|'=';'"';''';'<';'>';'/'|]

        let rec readString (acc:string) = function
            | c :: '<' :: n :: t when Char.IsLetter(n)  ->
                position <- position + acc.Length + 1
                (acc + (c.ToString())), t
            | c :: t -> readString (acc + (c.ToString())) t
            | [] -> 
                position <- position + acc.Length + 1
                acc, []
            | _ -> failwith "reading algorithm error"
        
        let rec readName (acc:string) = function
            | c :: t when Char.IsWhiteSpace(c) ->
                position <- position + acc.Length + 1
                acc, t
            | c :: t when (decisiveChars |> Seq.exists(fun i -> i = c)) ->
                position <- position + acc.Length + 1
                acc, c :: t
            | c :: t -> readName (acc + (c.ToString())) t
            | [] -> 
                position <- position + acc.Length + 1
                acc, []
            | _ -> failwith "reading algorithm error"
        
        let mutable inQuotes = false
        
        let readAttributeValue (chars:list<char>) =
            let rec readQuotedString acc = function
                | '\'' :: t -> 
                    if inQuotes then
                        inQuotes <- false
                        acc, t
                    else
                        inQuotes <- true
                        readString acc t
            
                | '\\' :: '\'' :: t when inQuotes ->
                    readString (acc + ('\''.ToString())) t

                | c :: t when inQuotes ->
                    readString (acc + (c.ToString())) t
                | c :: t -> acc, c :: t
                | [] -> 
                    acc, []
                | _ ->
                    failwith "Invalid css selector syntax"

//            let checkIfAttributeHasValue acc = function
                

            readQuotedString "" chars

        let readAttribute (chars:list<char>) = 
            let name, t' = readName "" chars
            let value, t2' = readAttributeValue t'
            if String.IsNullOrEmpty name then
                None, t'
            else
                Some(new Attribute(name, value)), t'

        let rec readAttributes (acc:list<Attribute>) = function
            | c :: t -> 
                let attr, t' = readAttribute (c :: t)
                match attr with
                    | Some(a) -> acc |> Seq.append [|a|] |> Seq.toList, t'
                    | None -> acc, t'
            | [] -> acc, []
            | _ -> failwith "reading algorithm error"

        member public x.ReadElement() = 
            let html = source |> Seq.skip(position) |> Seq.toList
            let s, rest = readString "" html
            s

        member public x.ReadTag() = 
            let html = source |> Seq.skip(position) |> Seq.toList
            let tag = match html.Head with
                    | '<' -> 
                        let name, t' = readName "" (html |> Seq.skip(1) |> Seq.toList)
                        let attributes, t2 = readAttributes List<Attribute>.Empty t'

                        new Tag(name, "", attributes)
                    | c -> 
                        let text, t' = readString "" html
                        new Tag("", text, List<Attribute>.Empty)
                    | _ -> failwith "parsing algorithm error"
            tag



