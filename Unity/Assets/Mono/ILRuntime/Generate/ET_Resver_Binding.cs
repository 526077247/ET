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
    unsafe class ET_Resver_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ET.Resver);

            field = type.GetField("channel", flag);
            app.RegisterCLRFieldGetter(field, get_channel_0);
            app.RegisterCLRFieldSetter(field, set_channel_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_channel_0, AssignFromStack_channel_0);
            field = type.GetField("update_tailnumber", flag);
            app.RegisterCLRFieldGetter(field, get_update_tailnumber_1);
            app.RegisterCLRFieldSetter(field, set_update_tailnumber_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_update_tailnumber_1, AssignFromStack_update_tailnumber_1);
            field = type.GetField("force_update", flag);
            app.RegisterCLRFieldGetter(field, get_force_update_2);
            app.RegisterCLRFieldSetter(field, set_force_update_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_force_update_2, AssignFromStack_force_update_2);


        }



        static object get_channel_0(ref object o)
        {
            return ((ET.Resver)o).channel;
        }

        static StackObject* CopyToStack_channel_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((ET.Resver)o).channel;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_channel_0(ref object o, object v)
        {
            ((ET.Resver)o).channel = (System.Collections.Generic.List<System.String>)v;
        }

        static StackObject* AssignFromStack_channel_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.String> @channel = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ET.Resver)o).channel = @channel;
            return ptr_of_this_method;
        }

        static object get_update_tailnumber_1(ref object o)
        {
            return ((ET.Resver)o).update_tailnumber;
        }

        static StackObject* CopyToStack_update_tailnumber_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((ET.Resver)o).update_tailnumber;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_update_tailnumber_1(ref object o, object v)
        {
            ((ET.Resver)o).update_tailnumber = (System.Collections.Generic.List<System.String>)v;
        }

        static StackObject* AssignFromStack_update_tailnumber_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.String> @update_tailnumber = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ET.Resver)o).update_tailnumber = @update_tailnumber;
            return ptr_of_this_method;
        }

        static object get_force_update_2(ref object o)
        {
            return ((ET.Resver)o).force_update;
        }

        static StackObject* CopyToStack_force_update_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((ET.Resver)o).force_update;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_force_update_2(ref object o, object v)
        {
            ((ET.Resver)o).force_update = (System.Int32)v;
        }

        static StackObject* AssignFromStack_force_update_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @force_update = ptr_of_this_method->Value;
            ((ET.Resver)o).force_update = @force_update;
            return ptr_of_this_method;
        }



    }
}
