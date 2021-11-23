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
    unsafe class ET_CodeLoader_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ET.CodeLoader);

            field = type.GetField("Update", flag);
            app.RegisterCLRFieldGetter(field, get_Update_0);
            app.RegisterCLRFieldSetter(field, set_Update_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Update_0, AssignFromStack_Update_0);
            field = type.GetField("LateUpdate", flag);
            app.RegisterCLRFieldGetter(field, get_LateUpdate_1);
            app.RegisterCLRFieldSetter(field, set_LateUpdate_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_LateUpdate_1, AssignFromStack_LateUpdate_1);
            field = type.GetField("OnApplicationQuit", flag);
            app.RegisterCLRFieldGetter(field, get_OnApplicationQuit_2);
            app.RegisterCLRFieldSetter(field, set_OnApplicationQuit_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnApplicationQuit_2, AssignFromStack_OnApplicationQuit_2);


        }



        static object get_Update_0(ref object o)
        {
            return ((ET.CodeLoader)o).Update;
        }

        static StackObject* CopyToStack_Update_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ET.CodeLoader)o).Update;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Update_0(ref object o, object v)
        {
            ((ET.CodeLoader)o).Update = (System.Action)v;
        }

        static StackObject* AssignFromStack_Update_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @Update = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ET.CodeLoader)o).Update = @Update;
            return ptr_of_this_method;
        }

        static object get_LateUpdate_1(ref object o)
        {
            return ((ET.CodeLoader)o).LateUpdate;
        }

        static StackObject* CopyToStack_LateUpdate_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ET.CodeLoader)o).LateUpdate;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LateUpdate_1(ref object o, object v)
        {
            ((ET.CodeLoader)o).LateUpdate = (System.Action)v;
        }

        static StackObject* AssignFromStack_LateUpdate_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @LateUpdate = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ET.CodeLoader)o).LateUpdate = @LateUpdate;
            return ptr_of_this_method;
        }

        static object get_OnApplicationQuit_2(ref object o)
        {
            return ((ET.CodeLoader)o).OnApplicationQuit;
        }

        static StackObject* CopyToStack_OnApplicationQuit_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ET.CodeLoader)o).OnApplicationQuit;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnApplicationQuit_2(ref object o, object v)
        {
            ((ET.CodeLoader)o).OnApplicationQuit = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnApplicationQuit_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnApplicationQuit = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((ET.CodeLoader)o).OnApplicationQuit = @OnApplicationQuit;
            return ptr_of_this_method;
        }



    }
}
