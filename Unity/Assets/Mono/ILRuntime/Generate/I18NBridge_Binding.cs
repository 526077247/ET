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

namespace ILRuntime.Runtime.Generated
{
    unsafe class I18NBridge_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::I18NBridge);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);

            field = type.GetField("i18nTextKeyDic", flag);
            app.RegisterCLRFieldGetter(field, get_i18nTextKeyDic_0);
            app.RegisterCLRFieldSetter(field, set_i18nTextKeyDic_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_i18nTextKeyDic_0, AssignFromStack_i18nTextKeyDic_0);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::I18NBridge.Instance;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_i18nTextKeyDic_0(ref object o)
        {
            return ((global::I18NBridge)o).i18nTextKeyDic;
        }

        static StackObject* CopyToStack_i18nTextKeyDic_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::I18NBridge)o).i18nTextKeyDic;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_i18nTextKeyDic_0(ref object o, object v)
        {
            ((global::I18NBridge)o).i18nTextKeyDic = (System.Collections.Generic.Dictionary<System.String, System.String>)v;
        }

        static StackObject* AssignFromStack_i18nTextKeyDic_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, System.String> @i18nTextKeyDic = (System.Collections.Generic.Dictionary<System.String, System.String>)typeof(System.Collections.Generic.Dictionary<System.String, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::I18NBridge)o).i18nTextKeyDic = @i18nTextKeyDic;
            return ptr_of_this_method;
        }



    }
}
