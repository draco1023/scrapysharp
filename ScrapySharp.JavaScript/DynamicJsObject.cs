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
        private readonly Dictionary<string, object> dynamicMembers = new Dictionary<string, object>();

        public object OnPropertyGetter(SMScript script, string name)
        {
            if (!dynamicMembers.ContainsKey(name))
                return null;

            var jsval = dynamicMembers[name];

            if (jsval is ulong)
            {
                var fromJsVal = Interop.FromJSVal(script, (ulong) jsval);

                return fromJsVal;
            }
            return jsval;
        }

        public void OnPropertySetter(SMScript script, string name, object value)
        {
            //var eval = script.Eval<object>("return window." + name + ";");

            if (dynamicMembers.ContainsKey(name))
                dynamicMembers[name] = value;
            else
                dynamicMembers.Add(name, value);

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