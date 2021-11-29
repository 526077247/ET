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
    unsafe class ET_WhiteConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ET.WhiteConfig);

            field = type.GetField("env_id", flag);
            app.RegisterCLRFieldGetter(field, get_env_id_0);
            app.RegisterCLRFieldSetter(field, set_env_id_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_env_id_0, AssignFromStack_env_id_0);
            field = type.GetField("account", flag);
            app.RegisterCLRFieldGetter(field, get_account_1);
            app.RegisterCLRFieldSetter(field, set_account_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_account_1, AssignFromStack_account_1);


        }



        static object get_env_id_0(ref object o)
        {
            return ((ET.WhiteConfig)o).env_id;
        }

        static StackObject* CopyToStack_env_id_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ET.WhiteConfig)o).env_id;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_env_id_0(ref object o, object v)
        {
            ((ET.WhiteConfig)o).env_id = (System.Int32)v;
        }

        static StackObject* AssignFromStack_env_id_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @env_id = ptr_of_this_method->Value;
            ((ET.WhiteConfig)o).env_id = @env_id;
            return ptr_of_this_method;
        }

        static object get_account_1(ref object o)
        {
            return ((ET.WhiteConfig)o).account;
        }

        static StackObject* CopyToStack_account_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ET.WhiteConfig)o).account;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_account_1(ref object o, object v)
        {
            ((ET.WhiteConfig)o).account = (System.String)v;
        }

        static StackObject* AssignFromStack_account_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @account = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((ET.WhiteConfig)o).account = @account;
            return ptr_of_this_method;
        }



    }
}
