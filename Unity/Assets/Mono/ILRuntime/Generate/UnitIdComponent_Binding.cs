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
    unsafe class UnitIdComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UnitIdComponent);

            field = type.GetField("UnitId", flag);
            app.RegisterCLRFieldGetter(field, get_UnitId_0);
            app.RegisterCLRFieldSetter(field, set_UnitId_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_UnitId_0, AssignFromStack_UnitId_0);


        }



        static object get_UnitId_0(ref object o)
        {
            return ((global::UnitIdComponent)o).UnitId;
        }

        static StackObject* CopyToStack_UnitId_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UnitIdComponent)o).UnitId;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_UnitId_0(ref object o, object v)
        {
            ((global::UnitIdComponent)o).UnitId = (System.Int64)v;
        }

        static StackObject* AssignFromStack_UnitId_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @UnitId = *(long*)&ptr_of_this_method->Value;
            ((global::UnitIdComponent)o).UnitId = @UnitId;
            return ptr_of_this_method;
        }



    }
}
