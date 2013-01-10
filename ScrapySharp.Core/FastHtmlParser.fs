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

    type StringPart = string * int * (int *int)
    
    type FastHtmlParser(source: string) =
        let stringPart s = (s, 0, (0, 0))

        let line ((_,_,(l,_)): StringPart) = l
        let col ((_,_,(_,c)):StringPart) = c

        

        let head ((s,i, _): StringPart) =
            s.[i]

        let next ((s,i, (line, col)): StringPart) =
            let c = s.[i]
            if c = '\n' then
                (s, i+1, (line+1, 0))
            else
                (s, i+1, (line, col+1))

        let rec nextn n s =
            if n = 0 then
                s
            else 
                nextn (n-1) (next s)

        let rec nextnfast n (s,i,(line, col)) =
            (s, i+n, (line, col + n))

        let isEnd ((s,i, _): StringPart) =
            i >= s.Length

        let isEndn n ((s,i, _): StringPart) =
            i + n  > s.Length

        let nth n ((s, i, _): StringPart) = 
            s.[i+n]

        let (|NC|_|) (s: StringPart) =
            if isEnd s then
                None
            else
                Some(head s, next s)

        let (|NEC|_|) c (s: StringPart) =
            if isEnd s || head s <> c then
                None
            else
                Some(next s)
        
        let isWhiteSpace c = Char.IsWhiteSpace(c)

        let (|WS|_|) (s: StringPart) =
            if isEnd s or not( head s |> isWhiteSpace) then
                None
            else
                let (str, i, _)  = s
                let mutable e = i
                while isWhiteSpace str.[e] do
                    e <- e + 1
                
                Some(nextn (e-i) s)

        let (|NC2|_|) (s: StringPart) =
            if isEndn 2 s then
                None
            else
                Some(head s, nth 1 s , nextn 2 s)

        let (|NC3|_|) (s: StringPart) =
            if isEndn 3 s then
                None
            else
                Some(head s, nth 1 s, nth 2 s, nextn 3 s)


        let (|End|_|) (s: StringPart) =
            if isEnd s then
                Some()
            else 
                None

        let (|NS|_|) (s: string) (sp: StringPart) =
            let sps,i,_ = sp
            if String.Compare(s,0,sps,i,s.Length, StringComparison.InvariantCulture) = 0 then
                Some(nextnfast s.Length sp)
            else
                None

        let decisiveChars = [|'=';'"';''';'<';'>';'/'|]
        let contains source item = source |> Seq.exists(fun i -> i = item)

        let mutable inQuotes = false
        let mutable endChar = new Char()

        let rec readString (acc:string) (s: StringPart) = 
            match s with
                | NC3(c, '<', n,  t)  when Char.IsLetter(n)  ->
                    (acc + (c.ToString())), next s
                | NS "</" t -> acc, s
                | NC(c, t) -> readString (acc + (c.ToString())) t
                | End -> acc, s
                | _ -> failwith "reading algorithm error"
        
        let skipSpaces (s: StringPart) = 
            match s with
                | WS t -> t
                | NC(_,_) -> s
                | End -> s
                | _ -> failwith "skipSpaces algorithm error"

        let rec skipEndTagContent (s: StringPart) = 
            match s with
                | NEC '>' t -> next s
                | NC(_, t) -> skipEndTagContent t
                | End -> s
                | _ -> failwith "reading algorithm error"

        let rec readName (acc:string) (s: StringPart) = 
            match s with 
                | WS t -> acc, t
                | NC(c, t) when contains decisiveChars c -> acc, s 
                | NC(c, t) -> readName (acc + (c.ToString())) t
                | End -> acc, s
                | _ -> failwith "reading algorithm error"
               
        let rec readQuotedString (acc:string) (s: StringPart) = 
            match s with
                | NEC ''' t -> 
                    if inQuotes then
                        inQuotes <- false
                        acc, t
                    else
                        inQuotes <- true
                        readQuotedString acc t
                | NS "\\'" t when inQuotes -> readQuotedString (acc + ('\''.ToString())) t
                | NC2 (p, c, t) when c = endChar && p <> '\\' -> (acc + (p.ToString())), t
                | NC(c, t) -> readQuotedString (acc + (c.ToString())) t
                | End -> acc, s
                | _ -> failwith "Invalid attribute syntax"

        let rec checkIfAttributeHasValue (s: StringPart) = 
            match s with
                | NC(c, t) when Char.IsWhiteSpace(c) -> checkIfAttributeHasValue t
                | NEC '=' t -> true, t
                | NS "/>" _ -> false, s
                | NC(c, t) when contains (decisiveChars |> Seq.filter (fun i -> i <> '=')) c -> false, t
                | End -> false, s
                | _ -> failwith "Invalid attribute syntax"

        let readAttributeValue (s: StringPart) =
            let hasValue, right = checkIfAttributeHasValue s
            if hasValue then
                if head right = '"' || head right = ''' then
                    endChar <- head right
                    readQuotedString "" (next right)
                else
                    endChar <- new Char()
                    readQuotedString "" right
            else
                "", right

        let readAttribute (s: StringPart) = 
            let name, t1 = s |> skipSpaces |> readName ""
            let value, t2 = readAttributeValue t1
            if String.IsNullOrEmpty name then
                None, t2
            else
                Some(new Attribute(name, value)), t2

        let rec readAttributes (acc:list<Attribute>) (s: StringPart) = 
            match s with
                | NS "/>" t -> acc, s
                | NEC '>' t -> acc, s
                | NC(c, t) -> 
                    let attr, right = readAttribute s
                    match attr with
                        | Some(a) -> 
                            let matched = a :: acc
                            readAttributes matched right
                        | None -> acc, s
                | End -> acc, s
                | _ -> failwith "reading algorithm error"

        let rec checkIfSelfClosed (s: StringPart) =
            match s with
                | NC(c, t) when Char.IsWhiteSpace(c) -> checkIfSelfClosed t
                | NS "/>"  t -> true, t
                | NC(_, t) -> false, t
                | End -> true, s
                | _ -> failwith "Invalid attribute syntax"

        let rec parseTags acc (s: StringPart) =
            match s with
                | NS "</" html ->
                    let name, t1 = readName "" html
                    let right = html |> skipEndTagContent
                    acc, right
                | NEC '<' html -> 
                    let name, t1 = readName "" html
                    let attributes, t2 = readAttributes List<Attribute>.Empty t1
                    let isSelfClosed, t3 = checkIfSelfClosed t2
                    let tags, t4 = if isSelfClosed then
                                        new Tag(name, "", attributes, List<Tag>.Empty) :: acc, t3
                                    else
                                        let children, right = parseTags acc t3
                                        new Tag(name, "", attributes, children |> List.rev) :: acc, right

                    if isEnd t4 then
                        tags, t4
                    else
                        let nextSiblings, t5 = parseTags acc t4
                        acc |> List.append(tags) |> List.append(nextSiblings), t5
                | NC(c, html) -> 
                    let text, t1 = readString "" s
                    let nextSiblings, right = parseTags acc t1
                    new Tag("", text, List<Attribute>.Empty, List<Tag>.Empty) :: nextSiblings, right
                | End -> acc, s
                | _ -> failwith "parsing algorithm error"

        let rec parseHtml acc = function
                    | [] -> acc, []
                    | _ -> failwith "parsing algorithm error"

        member public x.ReadTags() = 
                let html = stringPart source
                let tags, right = parseTags List<Tag>.Empty html
                new System.Collections.Generic.List<Tag>(tags)

