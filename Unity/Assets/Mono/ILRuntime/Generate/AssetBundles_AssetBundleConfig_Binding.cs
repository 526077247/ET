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
    unsafe class AssetBundles_AssetBundleConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(AssetBundles.AssetBundleConfig);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[]{};
            method = type.GetMethod("SyncLoadGlobalAssetBundle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SyncLoadGlobalAssetBundle_1);
            args = new Type[]{};
            method = type.GetMethod("get_EngineVer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_EngineVer_2);
            args = new Type[]{};
            method = type.GetMethod("get_ResVer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ResVer_3);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = AssetBundles.AssetBundleConfig.Instance;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SyncLoadGlobalAssetBundle_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            AssetBundles.AssetBundleConfig instance_of_this_method = (AssetBundles.AssetBundleConfig)typeof(AssetBundles.AssetBundleConfig).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SyncLoadGlobalAssetBundle();

            return __ret;
        }

        static StackObject* get_EngineVer_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            AssetBundles.AssetBundleConfig instance_of_this_method = (AssetBundles.AssetBundleConfig)typeof(AssetBundles.AssetBundleConfig).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.EngineVer;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_ResVer_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            AssetBundles.AssetBundleConfig instance_of_this_method = (AssetBundles.AssetBundleConfig)typeof(AssetBundles.AssetBundleConfig).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ResVer;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
