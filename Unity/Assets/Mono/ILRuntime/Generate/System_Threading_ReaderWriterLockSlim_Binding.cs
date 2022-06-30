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
    unsafe class System_Threading_ReaderWriterLockSlim_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(System.Threading.ReaderWriterLockSlim);
            args = new Type[]{};
            method = type.GetMethod("EnterWriteLock", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EnterWriteLock_0);
            args = new Type[]{};
            method = type.GetMethod("ExitWriteLock", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ExitWriteLock_1);
            args = new Type[]{};
            method = type.GetMethod("EnterUpgradeableReadLock", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EnterUpgradeableReadLock_2);
            args = new Type[]{};
            method = type.GetMethod("ExitUpgradeableReadLock", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ExitUpgradeableReadLock_3);
            args = new Type[]{};
            method = type.GetMethod("EnterReadLock", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EnterReadLock_4);
            args = new Type[]{};
            method = type.GetMethod("ExitReadLock", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ExitReadLock_5);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* EnterWriteLock_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Threading.ReaderWriterLockSlim instance_of_this_method = (System.Threading.ReaderWriterLockSlim)typeof(System.Threading.ReaderWriterLockSlim).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.EnterWriteLock();

            return __ret;
        }

        static StackObject* ExitWriteLock_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Threading.ReaderWriterLockSlim instance_of_this_method = (System.Threading.ReaderWriterLockSlim)typeof(System.Threading.ReaderWriterLockSlim).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ExitWriteLock();

            return __ret;
        }

        static StackObject* EnterUpgradeableReadLock_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Threading.ReaderWriterLockSlim instance_of_this_method = (System.Threading.ReaderWriterLockSlim)typeof(System.Threading.ReaderWriterLockSlim).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.EnterUpgradeableReadLock();

            return __ret;
        }

        static StackObject* ExitUpgradeableReadLock_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Threading.ReaderWriterLockSlim instance_of_this_method = (System.Threading.ReaderWriterLockSlim)typeof(System.Threading.ReaderWriterLockSlim).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ExitUpgradeableReadLock();

            return __ret;
        }

        static StackObject* EnterReadLock_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Threading.ReaderWriterLockSlim instance_of_this_method = (System.Threading.ReaderWriterLockSlim)typeof(System.Threading.ReaderWriterLockSlim).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.EnterReadLock();

            return __ret;
        }

        static StackObject* ExitReadLock_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Threading.ReaderWriterLockSlim instance_of_this_method = (System.Threading.ReaderWriterLockSlim)typeof(System.Threading.ReaderWriterLockSlim).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ExitReadLock();

            return __ret;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new System.Threading.ReaderWriterLockSlim();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
