namespace ScrapySharp.Core

    open System
    open System.IO
    open System.Net
    open System.Runtime.Serialization.Formatters.Binary
    open System.Text

    type Attribute(name:string, value:string) = 
        member t.Name = name
        member t.Value = value

    type Tag(name:string, innerText:string, attributes:List<Attribute>, children:List<Tag>) =
        member t.Name = name
        member t.InnerText = innerText
        member t.Attributes = attributes
        member t.Children = children

    type FastHtmlParser(source: string) =
        let mutable source = source
        let mutable position = 0

        let decisiveChars = [|'=';'"';''';'<';'>';'/'|]
        let contains source item = source |> Seq.exists(fun i -> i = item)

        let rec readString (acc:string) = function
            | c :: '<' :: n :: t when Char.IsLetter(n)  ->
                position <- position + acc.Length + 1
                (acc + (c.ToString())), '<' :: n :: t
            | c :: t -> readString (acc + (c.ToString())) t
            | [] -> 
                position <- position + acc.Length + 1
                acc, []
            | _ -> failwith "reading algorithm error"
        
        let rec skipSpaces = function
            | c :: t when Char.IsWhiteSpace(c) -> skipSpaces t
            | c :: t -> c :: t
            | [] -> []
            | _ -> failwith "skipSpaces algorithm error"

        let rec readName (acc:string) = function
            | c :: t when Char.IsWhiteSpace(c) ->
                position <- position + acc.Length + 1
                acc, t
//            | c :: t when (decisiveChars |> Seq.exists(fun i -> i = c)) ->
            | c :: t when contains decisiveChars c ->
                position <- position + acc.Length + 1
                acc, c :: t
            | c :: t -> readName (acc + (c.ToString())) t
            | [] -> 
                position <- position + acc.Length + 1
                acc, []
            | _ -> failwith "reading algorithm error"
        
        let mutable inQuotes = false
        let mutable endChar:char = new Char()
        
        let readAttributeValue (chars:list<char>) =
            let rec readQuotedString acc = function
                | '\'' :: t -> 
                    if inQuotes then
                        inQuotes <- false
                        acc, t
                    else
                        inQuotes <- true
                        readQuotedString acc t
                | '\\' :: '\'' :: t when inQuotes -> readQuotedString (acc + ('\''.ToString())) t
                | p :: c :: t when c = endChar && p <> '\\' -> (acc + (p.ToString())), t
                | c :: t -> readQuotedString (acc + (c.ToString())) t
                | [] -> acc, []
                | _ -> failwith "Invalid attribute syntax"

            let rec checkIfAttributeHasValue = function
                | c :: t when Char.IsWhiteSpace(c) -> checkIfAttributeHasValue t
                | '=' :: t -> true, t
                | '/' :: '>' :: t -> false, '/' :: '>' :: t
                | c :: t when contains (decisiveChars |> Seq.filter (fun i -> i <> '=')) c -> false, t
                | [] -> false, []
                | _ -> failwith "Invalid attribute syntax"

            let hasValue, right = checkIfAttributeHasValue chars
            if hasValue then
                if right.Head = '"' || right.Head = ''' then
                    endChar <- right.Head
                    readQuotedString "" (right |> Seq.skip(1) |> Seq.toList)
                else
                    endChar <- new Char()
                    readQuotedString "" right
            else
                "", right

        let readAttribute (chars:list<char>) = 
            let name, t1 = chars |> skipSpaces |> readName ""
            let value, t2 = readAttributeValue t1
            if String.IsNullOrEmpty name then
                None, t2
            else
                Some(new Attribute(name, value)), t2

        let rec readAttributes (acc:list<Attribute>) = function
            | c :: t -> 
                let attr, right = readAttribute (c :: t)
                match attr with
                    | Some(a) -> 
                        let matched = a :: acc
                        readAttributes matched right
//                        acc |> Seq.append [|a|] |> Seq.toList, right
                    | None -> acc, c :: right
            | [] -> acc, []
            | _ -> failwith "reading algorithm error"

        let rec checkIfSelfClosed = function
                | c :: t when Char.IsWhiteSpace(c) -> checkIfSelfClosed t
                | '/' :: '>' :: t -> true, t
                | c :: t -> false, t
                | [] -> true, []
                | _ -> failwith "Invalid attribute syntax"

        let rec parseTags acc = function
                    | '<' :: html -> 
                        let name, t1 = readName "" (html |> Seq.toList)
                        let attributes, t2 = readAttributes List<Attribute>.Empty t1
                        let isSelfClosed, t3 = checkIfSelfClosed t2
                        position <- source.Length - t3.Length

                        let tags, t4 = if isSelfClosed then
                                            new Tag(name, "", attributes, List<Tag>.Empty) :: acc, t3
                                       else
                                            let children, right = parseTags acc t3
                                            new Tag(name, "", attributes, children) :: acc, right
                        let nextSiblings, t5 = parseTags acc t4

                        position <- source.Length - t5.Length

                        tags |> List.append(nextSiblings) |> List.append(acc) , t5
                    | c :: html -> 
                        let text, t1 = readString "" (c :: html)
                        let nextSiblings, right = parseTags acc t1

                        new Tag("", text, List<Attribute>.Empty, List<Tag>.Empty) :: nextSiblings, right
//                        new Tag("", text, List<Attribute>.Empty) :: acc, t'
                    | [] -> acc, []
                    | _ -> failwith "parsing algorithm error"

        member public x.ReadElement() = 
            let html = source |> Seq.skip(position) |> Seq.toList
            let s, rest = readString "" html
            s

        member public x.ReadTags() = 
            let html = source |> Seq.skip(position) |> Seq.toList
            
            let tags, right = parseTags List<Tag>.Empty html

//            let tag = match html.Head with
//                    | '<' -> 
//                        let name, t1 = readName "" (html |> Seq.skip(1) |> Seq.toList)
//                        let attributes, t2 = readAttributes List<Attribute>.Empty t1
//                        let isSelfClosed, t3 = checkIfSelfClosed t2
//                        position <- source.Length - t3.Length
//
//                        new Tag(name, "", attributes)
//                    | c -> 
//                        let text, t' = readString "" html
//                        new Tag("", text, List<Attribute>.Empty)
//                    | _ -> failwith "parsing algorithm error"
            new System.Collections.Generic.List<Tag>(tags)



