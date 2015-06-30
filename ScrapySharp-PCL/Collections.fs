module Collections
    
    open System
    open System.Text
    open System.Collections.Generic

    open Utils

    type NameValueCollection() =
        let items = new List<KeyValuePair<string, string>>()
        
        member x.Allkeys 
            with get () = items |> Seq.map (fun kv -> kv.Key) |> Seq.toList

        member x.Values
            with get () = items |> Seq.map (fun kv -> kv.Value) |> Seq.toList

        member x.Get key = items |> Seq.filter (fun kv -> kv.Key = key) |> Seq.toList

        member x.Add (key, value) = items.Add (new KeyValuePair<string, string>(key, value))

        override x.ToString() =
            let builder = new StringBuilder()
            for i in 0..items.Count do
                let kv = items.Item(i)
                builder.AppendFormat("{0}={1}", urlEncode(kv.Key), urlEncode(kv.Value)) |> ignore
                if i < items.Count - 1 then 
                    builder.Append("&") |> ignore
            builder.ToString()



