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
    unsafe class SuperScrollView_LoopListViewItem2_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(SuperScrollView.LoopListViewItem2);
            args = new Type[]{};
            method = type.GetMethod("get_IsInitHandlerCalled", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsInitHandlerCalled_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_IsInitHandlerCalled", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_IsInitHandlerCalled_1);
            args = new Type[]{};
            method = type.GetMethod("get_ItemId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ItemId_2);


        }


        static StackObject* get_IsInitHandlerCalled_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            SuperScrollView.LoopListViewItem2 instance_of_this_method = (SuperScrollView.LoopListViewItem2)typeof(SuperScrollView.LoopListViewItem2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsInitHandlerCalled;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* set_IsInitHandlerCalled_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            SuperScrollView.LoopListViewItem2 instance_of_this_method = (SuperScrollView.LoopListViewItem2)typeof(SuperScrollView.LoopListViewItem2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.IsInitHandlerCalled = value;

            return __ret;
        }

        static StackObject* get_ItemId_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            SuperScrollView.LoopListViewItem2 instance_of_this_method = (SuperScrollView.LoopListViewItem2)typeof(SuperScrollView.LoopListViewItem2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ItemId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }



    }
}