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

            field = type.GetField("GetValueById", flag);
            app.RegisterCLRFieldGetter(field, get_GetValueById_0);
            app.RegisterCLRFieldSetter(field, set_GetValueById_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_GetValueById_0, AssignFromStack_GetValueById_0);
            field = type.GetField("GetValueByKey", flag);
            app.RegisterCLRFieldGetter(field, get_GetValueByKey_1);
            app.RegisterCLRFieldSetter(field, set_GetValueByKey_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_GetValueByKey_1, AssignFromStack_GetValueByKey_1);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::I18NBridge.Instance;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_GetValueById_0(ref object o)
        {
            return ((global::I18NBridge)o).GetValueById;
        }

        static StackObject* CopyToStack_GetValueById_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::I18NBridge)o).GetValueById;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GetValueById_0(ref object o, object v)
        {
            ((global::I18NBridge)o).GetValueById = (System.Func<System.Int32, System.String>)v;
        }

        static StackObject* AssignFromStack_GetValueById_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Func<System.Int32, System.String> @GetValueById = (System.Func<System.Int32, System.String>)typeof(System.Func<System.Int32, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::I18NBridge)o).GetValueById = @GetValueById;
            return ptr_of_this_method;
        }

        static object get_GetValueByKey_1(ref object o)
        {
            return ((global::I18NBridge)o).GetValueByKey;
        }

        static StackObject* CopyToStack_GetValueByKey_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::I18NBridge)o).GetValueByKey;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GetValueByKey_1(ref object o, object v)
        {
            ((global::I18NBridge)o).GetValueByKey = (System.Func<System.String, System.String>)v;
        }

        static StackObject* AssignFromStack_GetValueByKey_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Func<System.String, System.String> @GetValueByKey = (System.Func<System.String, System.String>)typeof(System.Func<System.String, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((global::I18NBridge)o).GetValueByKey = @GetValueByKey;
            return ptr_of_this_method;
        }



    }
}
