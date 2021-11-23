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
    unsafe class AssetBundles_AddressableUpdateAsyncOperation_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(AssetBundles.AddressableUpdateAsyncOperation);
            args = new Type[]{};
            method = type.GetMethod("get_catalog", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_catalog_0);
            args = new Type[]{};
            method = type.GetMethod("get_downloadSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_downloadSize_1);
            args = new Type[]{};
            method = type.GetMethod("GetNeedDownloadinfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetNeedDownloadinfo_2);
            args = new Type[]{};
            method = type.GetMethod("get_isSuccess", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_isSuccess_3);


        }


        static StackObject* get_catalog_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            AssetBundles.AddressableUpdateAsyncOperation instance_of_this_method = (AssetBundles.AddressableUpdateAsyncOperation)typeof(AssetBundles.AddressableUpdateAsyncOperation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.catalog;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_downloadSize_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            AssetBundles.AddressableUpdateAsyncOperation instance_of_this_method = (AssetBundles.AddressableUpdateAsyncOperation)typeof(AssetBundles.AddressableUpdateAsyncOperation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.downloadSize;

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetNeedDownloadinfo_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            AssetBundles.AddressableUpdateAsyncOperation instance_of_this_method = (AssetBundles.AddressableUpdateAsyncOperation)typeof(AssetBundles.AddressableUpdateAsyncOperation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetNeedDownloadinfo();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_isSuccess_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            AssetBundles.AddressableUpdateAsyncOperation instance_of_this_method = (AssetBundles.AddressableUpdateAsyncOperation)typeof(AssetBundles.AddressableUpdateAsyncOperation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.isSuccess;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }



    }
}
