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
    unsafe class ET_AppConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ET.AppConfig);

            field = type.GetField("app_ver", flag);
            app.RegisterCLRFieldGetter(field, get_app_ver_0);
            app.RegisterCLRFieldSetter(field, set_app_ver_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_app_ver_0, AssignFromStack_app_ver_0);
            field = type.GetField("app_url", flag);
            app.RegisterCLRFieldGetter(field, get_app_url_1);
            app.RegisterCLRFieldSetter(field, set_app_url_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_app_url_1, AssignFromStack_app_url_1);
            field = type.GetField("jump_channel", flag);
            app.RegisterCLRFieldGetter(field, get_jump_channel_2);
            app.RegisterCLRFieldSetter(field, set_jump_channel_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_jump_channel_2, AssignFromStack_jump_channel_2);


        }



        static object get_app_ver_0(ref object o)
        {
            return ((ET.AppConfig)o).app_ver;
        }

        static StackObject* CopyToStack_app_ver_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ET.AppConfig)o).app_ver;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_app_ver_0(ref object o, object v)
        {
            ((ET.AppConfig)o).app_ver = (System.Collections.Generic.Dictionary<System.String, ET.Resver>)v;
        }

        static StackObject* AssignFromStack_app_ver_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, ET.Resver> @app_ver = (System.Collections.Generic.Dictionary<System.String, ET.Resver>)typeof(System.Collections.Generic.Dictionary<System.String, ET.Resver>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((ET.AppConfig)o).app_ver = @app_ver;
            return ptr_of_this_method;
        }

        static object get_app_url_1(ref object o)
        {
            return ((ET.AppConfig)o).app_url;
        }

        static StackObject* CopyToStack_app_url_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ET.AppConfig)o).app_url;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_app_url_1(ref object o, object v)
        {
            ((ET.AppConfig)o).app_url = (System.String)v;
        }

        static StackObject* AssignFromStack_app_url_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @app_url = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((ET.AppConfig)o).app_url = @app_url;
            return ptr_of_this_method;
        }

        static object get_jump_channel_2(ref object o)
        {
            return ((ET.AppConfig)o).jump_channel;
        }

        static StackObject* CopyToStack_jump_channel_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((ET.AppConfig)o).jump_channel;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_jump_channel_2(ref object o, object v)
        {
            ((ET.AppConfig)o).jump_channel = (System.String)v;
        }

        static StackObject* AssignFromStack_jump_channel_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @jump_channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((ET.AppConfig)o).jump_channel = @jump_channel;
            return ptr_of_this_method;
        }



    }
}
