using ProtoBuf;

namespace ScrapySharp.Cache
{
    [ProtoContract]
    public class WebResourceEntity
    {
        [ProtoMember(1)]
        public string LastModified { get; set; }

        [ProtoMember(2)]
        public string AbsoluteUrl { get; set; }

        [ProtoMember(3)]
        public bool ForceDownload { get; set; }
    }
}