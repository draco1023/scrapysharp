module Utils
    
    open System

    let toChars (s:string) =
        [for i in 0..s.Length -> s.Chars i]

    let toHex (c:char) = Convert.ToInt32(c) |> fun i -> i.ToString("X")
    let urlEncode (s:string) = 
        let parts = s |> toChars |> Seq.map (fun c -> "%" + (c |> toHex))
        String.Join("", parts)

    let (|HexaChar|_|) (s:char list) =
        if s.Length > 0 && s.Head = '%' then
            let chars = s |> Seq.skip 1 |> Seq.take 2 |> Array.ofSeq
            let h = new String(chars)
            let num = Convert.ToInt32(h, 16)
            let tail = s |> Seq.skip (chars.Length+1) |> List.ofSeq
            Some ((Convert.ToChar num), tail)
        else
            None

    let urlDecode (text:string) =
        let rec decode s acc = 
            match s with
            | HexaChar (c, t) -> decode t (c :: acc)
            | c :: t          -> decode t (c :: acc)
            | []              -> new string(acc |> List.rev |> Array.ofList)
        decode (text |> toChars) []
