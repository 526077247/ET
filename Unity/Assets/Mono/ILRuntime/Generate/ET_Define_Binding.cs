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
    unsafe class ET_Define_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ET.Define);

            field = type.GetField("Networked", flag);
            app.RegisterCLRFieldGetter(field, get_Networked_0);
            app.RegisterCLRFieldSetter(field, set_Networked_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Networked_0, AssignFromStack_Networked_0);
            field = type.GetField("ForceUpdate", flag);
            app.RegisterCLRFieldGetter(field, get_ForceUpdate_1);
            app.RegisterCLRFieldSetter(field, set_ForceUpdate_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_ForceUpdate_1, AssignFromStack_ForceUpdate_1);
            field = type.GetField("IsEditor", flag);
            app.RegisterCLRFieldGetter(field, get_IsEditor_2);
            app.RegisterCLRFieldSetter(field, set_IsEditor_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_IsEditor_2, AssignFromStack_IsEditor_2);
            field = type.GetField("DesignScreen_Width", flag);
            app.RegisterCLRFieldGetter(field, get_DesignScreen_Width_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_DesignScreen_Width_3, null);
            field = type.GetField("DesignScreen_Height", flag);
            app.RegisterCLRFieldGetter(field, get_DesignScreen_Height_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_DesignScreen_Height_4, null);
            field = type.GetField("Debug", flag);
            app.RegisterCLRFieldGetter(field, get_Debug_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_Debug_5, null);


        }



        static object get_Networked_0(ref object o)
        {
            return ET.Define.Networked;
        }

        static StackObject* CopyToStack_Networked_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ET.Define.Networked;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_Networked_0(ref object o, object v)
        {
            ET.Define.Networked = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_Networked_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @Networked = ptr_of_this_method->Value == 1;
            ET.Define.Networked = @Networked;
            return ptr_of_this_method;
        }

        static object get_ForceUpdate_1(ref object o)
        {
            return ET.Define.ForceUpdate;
        }

        static StackObject* CopyToStack_ForceUpdate_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ET.Define.ForceUpdate;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_ForceUpdate_1(ref object o, object v)
        {
            ET.Define.ForceUpdate = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_ForceUpdate_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @ForceUpdate = ptr_of_this_method->Value == 1;
            ET.Define.ForceUpdate = @ForceUpdate;
            return ptr_of_this_method;
        }

        static object get_IsEditor_2(ref object o)
        {
            return ET.Define.IsEditor;
        }

        static StackObject* CopyToStack_IsEditor_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ET.Define.IsEditor;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IsEditor_2(ref object o, object v)
        {
            ET.Define.IsEditor = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IsEditor_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IsEditor = ptr_of_this_method->Value == 1;
            ET.Define.IsEditor = @IsEditor;
            return ptr_of_this_method;
        }

        static object get_DesignScreen_Width_3(ref object o)
        {
            return ET.Define.DesignScreen_Width;
        }

        static StackObject* CopyToStack_DesignScreen_Width_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ET.Define.DesignScreen_Width;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static object get_DesignScreen_Height_4(ref object o)
        {
            return ET.Define.DesignScreen_Height;
        }

        static StackObject* CopyToStack_DesignScreen_Height_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ET.Define.DesignScreen_Height;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static object get_Debug_5(ref object o)
        {
            return ET.Define.Debug;
        }

        static StackObject* CopyToStack_Debug_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ET.Define.Debug;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }



    }
}
