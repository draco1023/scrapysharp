using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;
using smnetjs;

namespace ScrapySharp.JavaScript
{
    [SMEmbedded(AllowInheritedMembers = false)]
    class DynamicJsObject : ISMDynamic
    {
        string Name = "WHAT";

        public object OnPropertyGetter(SMScript script, string name)
        {

            if (name == "name")
                return Name;

            return null;
        }

        public void OnPropertySetter(SMScript script, string name, object value)
        {

            if (name == "name")
                Name = (string)value;
        }

        [DllImport("mozjs185-1.0.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void JS_GetContextPrivate(IntPtr cx);
    }

    //[SMEmbedded(AccessibleName = "DynamicObject", Name = "dynamicObject")]
    //public class DynamicJsObject : DynamicObject
    //{
    //    private readonly Dictionary<string, object> members = new Dictionary<string, object>();

    //    public override bool TrySetMember(SetMemberBinder binder, object value)
    //    {
    //        if (members.ContainsKey(binder.Name))
    //            members[binder.Name] = value;
    //        else
    //        {
    //            members.Add(binder.Name, value);
    //        }

    //        return true;
    //    }

    //    public override bool TryGetMember(GetMemberBinder binder, out object result)
    //    {
    //        if (!members.ContainsKey(binder.Name))
    //        {
    //            result = null;
    //            return false;
    //        }
    //        result = members[binder.Name];

    //        return true;
    //    }
    //}
}