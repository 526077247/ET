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
    unsafe class ET_UpdateConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ET.UpdateConfig);

            field = type.GetField("app_list", flag);
            app.RegisterCLRFieldGetter(field, get_app_list_0);
            app.RegisterCLRFieldSetter(field, set_app_list_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_app_list_0, AssignFromStack_app_list_0);
            field = type.GetField("res_list", flag);
            app.RegisterCLRFieldGetter(field, get_res_list_1);
            app.RegisterCLRFieldSetter(field, set_res_list_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_res_list_1, AssignFromStack_res_list_1);


        }



        static object get_app_list_0(ref object o)
        {
            return ((ET.UpdateConfig)o).app_list;
        }

        static StackObject* CopyToStack_app_list_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((ET.UpdateConfig)o).app_list;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_app_list_0(ref object o, object v)
        {
            ((ET.UpdateConfig)o).app_list = (System.Collections.Generic.Dictionary<System.String, ET.AppConfig>)v;
        }

        static StackObject* AssignFromStack_app_list_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, ET.AppConfig> @app_list = (System.Collections.Generic.Dictionary<System.String, ET.AppConfig>)typeof(System.Collections.Generic.Dictionary<System.String, ET.AppConfig>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ET.UpdateConfig)o).app_list = @app_list;
            return ptr_of_this_method;
        }

        static object get_res_list_1(ref object o)
        {
            return ((ET.UpdateConfig)o).res_list;
        }

        static StackObject* CopyToStack_res_list_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((ET.UpdateConfig)o).res_list;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_res_list_1(ref object o, object v)
        {
            ((ET.UpdateConfig)o).res_list = (System.Collections.Generic.Dictionary<System.String, System.Collections.Generic.Dictionary<System.String, ET.Resver>>)v;
        }

        static StackObject* AssignFromStack_res_list_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, System.Collections.Generic.Dictionary<System.String, ET.Resver>> @res_list = (System.Collections.Generic.Dictionary<System.String, System.Collections.Generic.Dictionary<System.String, ET.Resver>>)typeof(System.Collections.Generic.Dictionary<System.String, System.Collections.Generic.Dictionary<System.String, ET.Resver>>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ET.UpdateConfig)o).res_list = @res_list;
            return ptr_of_this_method;
        }



    }
}
