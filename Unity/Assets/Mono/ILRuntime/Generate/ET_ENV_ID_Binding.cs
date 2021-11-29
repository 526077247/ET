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
    unsafe class ET_ENV_ID_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ET.ENV_ID);

            field = type.GetField("DEVELOP", flag);
            app.RegisterCLRFieldGetter(field, get_DEVELOP_0);
            app.RegisterCLRFieldSetter(field, set_DEVELOP_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_DEVELOP_0, AssignFromStack_DEVELOP_0);


        }



        static object get_DEVELOP_0(ref object o)
        {
            return ET.ENV_ID.DEVELOP;
        }

        static StackObject* CopyToStack_DEVELOP_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ET.ENV_ID.DEVELOP;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_DEVELOP_0(ref object o, object v)
        {
            ET.ENV_ID.DEVELOP = (System.Int32)v;
        }

        static StackObject* AssignFromStack_DEVELOP_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @DEVELOP = ptr_of_this_method->Value;
            ET.ENV_ID.DEVELOP = @DEVELOP;
            return ptr_of_this_method;
        }



    }
}
