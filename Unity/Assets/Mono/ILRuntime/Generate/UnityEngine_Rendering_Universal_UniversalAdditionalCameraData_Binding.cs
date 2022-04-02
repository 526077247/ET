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
    unsafe class UnityEngine_Rendering_Universal_UniversalAdditionalCameraData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData);
            args = new Type[]{typeof(UnityEngine.Rendering.Universal.CameraRenderType)};
            method = type.GetMethod("set_renderType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_renderType_0);
            args = new Type[]{};
            method = type.GetMethod("get_cameraStack", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_cameraStack_1);


        }


        static StackObject* set_renderType_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Rendering.Universal.CameraRenderType @value = (UnityEngine.Rendering.Universal.CameraRenderType)typeof(UnityEngine.Rendering.Universal.CameraRenderType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)20);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.renderType = value;

            return __ret;
        }

        static StackObject* get_cameraStack_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData instance_of_this_method = (UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.cameraStack;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
