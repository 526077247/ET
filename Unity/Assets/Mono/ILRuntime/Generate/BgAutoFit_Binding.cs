using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif
namespace ILRuntime.Runtime.Generated
{
    unsafe class BgAutoFit_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::BgAutoFit);

            field = type.GetField("bgSprite", flag);
            app.RegisterCLRFieldGetter(field, get_bgSprite_0);
            app.RegisterCLRFieldSetter(field, set_bgSprite_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_bgSprite_0, AssignFromStack_bgSprite_0);


        }



        static object get_bgSprite_0(ref object o)
        {
            return ((global::BgAutoFit)o).bgSprite;
        }

        static StackObject* CopyToStack_bgSprite_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((global::BgAutoFit)o).bgSprite;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_bgSprite_0(ref object o, object v)
        {
            ((global::BgAutoFit)o).bgSprite = (UnityEngine.Sprite)v;
        }

        static StackObject* AssignFromStack_bgSprite_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Sprite @bgSprite = (UnityEngine.Sprite)typeof(UnityEngine.Sprite).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::BgAutoFit)o).bgSprite = @bgSprite;
            return ptr_of_this_method;
        }



    }
}
