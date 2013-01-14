namespace ScrapySharp.Core

    open System
    open System.IO
    open System.Net
    open System.Runtime.Serialization.Formatters.Binary
    open System.Text

    type StringChar = int
    
//    type StringPart = int * int
    
//    [<Struct>]
//    type StringPart(s:int, len:int) =
//        member this.Start = s
//        member this.Length = len

    type Attribute(name:string, value:string) = 
        member t.Name = name
        member t.Value = value

    type Tag(name:string, innerText:string, attributes:List<Attribute>, children:List<Tag>) =
        member t.Name = name
        member t.InnerText = innerText
        member t.Attributes = attributes
        member t.Children = children

    type Position(column:int, line:int) =
        struct
            member t.Column = column
            member t.Line = line
        end

    type FastHtmlParser(source: string) =
        let stringChar s = 0

//        let emptyPart = StringPart(0, 0)

//        let content (s:StringPart) = source.Substring(s.Start, s.Length)
        let content ((p,l)) = source.Substring(p, l)

        let head (s:StringChar) = source.[s]
            
        let next (s:StringChar) = s + 1

        let rec nextn n (s:StringChar) = s + n

        let rec nextnfast n (s:StringChar) = s + n

        let isEnd (s: StringChar) = s >= source.Length

        let isEndn n (s: StringChar) = s + n  > source.Length

        let nth n (s: StringChar) = source.[s+n]

        let (|NC|_|) (s: StringChar) =
            if isEnd s then
                None
            else
                Some(head s, next s)

        let (|NEC|_|) c (s: StringChar) =
            if isEnd s || head s <> c then
                None
            else
                Some(next s)
        
        let isWhiteSpace c = Char.IsWhiteSpace(c)

        let (|WS|_|) (s: StringChar) =
            if isEnd s or not( head s |> isWhiteSpace) then
                None
            else
                let mutable e = s
                while isWhiteSpace source.[e] do
                    e <- e + 1
                Some(e)

        let (|NC2|_|) (s: StringChar) =
            if isEndn 2 s then
                None
            else
                Some(head s, nth 1 s , nextn 2 s)

        let (|NC3|_|) (s: StringChar) =
            if isEndn 3 s then
                None
            else
                Some(head s, nth 1 s, nth 2 s, nextn 3 s)

        let (|End|_|) (s: StringChar) =
            if isEnd s then
                Some()
            else 
                None

        let (|NS|_|) (s: string) (sp: StringChar) =
            if String.Compare(s,0,source,sp,s.Length, StringComparison.InvariantCulture) = 0 then
                Some(nextnfast s.Length sp)
            else
                None

        let decisiveChars = [|'=';'"';''';'<';'>';'/'|]
        let contains source item = source |> Seq.exists(fun i -> i = item)

        let mutable inQuotes = false
        let mutable endChar = new Char()

        let readString (acc:int) (s: StringChar) = 
            let mutable i = s
            let mutable acc2 = acc
            let mutable doloop = not(isEnd i)
            let mutable ret = acc2,i

            while doloop do
                if String.Compare("</", 0, source, i, "</".Length, StringComparison.InvariantCulture) = 0 then
                    ret <- acc2, i
                    doloop <- false
                elif i < source.Length-3 && source.[i+1] = '<' && Char.IsLetter(source.[i+2]) then
                    ret <- acc2+1, next i
                    doloop <- false
                else
                    acc2 <- acc2+1
                    ret <- acc2,i
                    doloop <- not(isEnd i)
                i <- i+1
            ret

        let skipSpaces (s: StringChar) = 
            match s with
                | WS t -> t
                | NC(_,_) -> s
                | End -> s
                | _ -> failwith "skipSpaces algorithm error"

        let rec skipEndTagContent (s: StringChar) = 
            match s with
                | NEC '>' t -> next s
                | NC(_, t) -> skipEndTagContent t
                | End -> s
                | _ -> failwith "reading algorithm error"

        let rec readName (acc:int) (s: StringChar) = 
            match s with 
                | WS t -> acc+1, t
                | NC(c, t) when contains decisiveChars c -> acc, s 
                | NC(c, t) -> readName (acc + 1) t
                | End -> acc, s
                | _ -> failwith "reading algorithm error"
               
        let rec readQuotedString (acc:int) (s: StringChar) = 
            match s with
                | NEC ''' t -> 
                    if inQuotes then
                        inQuotes <- false
                        acc, t
                    else
                        inQuotes <- true
                        readQuotedString acc t
                | NS "\\'" t when inQuotes -> readQuotedString (acc + 2) t
                | NC2 (p, c, t) when c = endChar && p <> '\\' -> (acc + 2), t
                | NC(c, t) -> readQuotedString (acc + 1) t
                | End -> acc, s
                | _ -> failwith "Invalid attribute syntax"

        let rec checkIfAttributeHasValue (s: StringChar) = 
            match s with
                | WS t -> checkIfAttributeHasValue t
                | NEC '=' t -> true, t
                | NS "/>" _ -> false, s
                | NC(c, t) when contains (decisiveChars |> Seq.filter (fun i -> i <> '=')) c -> false, t
                | End -> false, s
                | _ -> failwith "Invalid attribute syntax"

        let readAttributeValue (s: StringChar) =
            let hasValue, right = checkIfAttributeHasValue s
            if hasValue then
                if head right = '"' || head right = ''' then
                    endChar <- head right
                    let p,l = readQuotedString 0 (next right)
                    Some(l-p),l
                else
                    endChar <- new Char()
                    let p,l = readQuotedString 0 right
                    Some(l-p),l
            else
                None, right

        let readAttribute (s: StringChar) = 
            let trimmed = s |> skipSpaces
            let n, t1 =  trimmed |> readName 0
            let name = content (trimmed, n)
            let st, t2 = readAttributeValue t1
            match st with
                | Some(a) -> 
                    let v = content (a, t2-a-1)
                    new Attribute(name, v), t2
                | None -> new Attribute(name, name), t2

        let rec readAttributes (acc:list<Attribute>) (s: StringChar) = 
            match s with
                | NS "/>" t -> acc, s
                | NEC '>' t -> acc, s
                | WS(t) -> readAttributes acc (s |> skipSpaces)
                | NC(_, t) -> 
                    let attr, right = readAttribute s
                    let matched = attr :: acc
                    readAttributes matched right
                | End -> acc, s
                | _ -> failwith "reading algorithm error"

        let rec checkIfSelfClosed (s: StringChar) =
            match s with
                | WS t -> checkIfSelfClosed t
                | NS "/>"  t -> true, t
                | NC(_, t) -> false, t
                | End -> true, s
                | _ -> failwith "Invalid attribute syntax"

        let rec parseTags acc (s: StringChar) =
            match s with
                | NS "</" html ->
                    let name, t1 = readName 0 html
                    let right = html |> skipEndTagContent
                    acc, right
                | NEC '<' html -> 
                    let n, t1 = readName 0 html
                    let name = content (html,n)
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
                | NC(_, html) -> 
                    let text, t1 = readString 0 s
                    let nextSiblings, right = parseTags acc t1
                    new Tag("", content (s, text), List<Attribute>.Empty, List<Tag>.Empty) :: nextSiblings, right
                | End -> acc, s
                | _ -> failwith "parsing algorithm error"

        let rec parseHtml acc = function
                    | [] -> acc, []
                    | _ -> failwith "parsing algorithm error"

        member public x.ReadTags() = 
                let html = stringChar source
                let tags, right = parseTags List<Tag>.Empty html
                new System.Collections.Generic.List<Tag>(tags)

