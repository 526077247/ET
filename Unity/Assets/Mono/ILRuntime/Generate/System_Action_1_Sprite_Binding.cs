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
<<<<<<< HEAD:Unity/Assets/Mono/ILRuntime/Generate/System_Action_1_Sprite_Binding.cs
    unsafe class System_Action_1_Sprite_Binding
=======
    unsafe class System_Func_2_String_Byte_Array_Binding
>>>>>>> upstream/master:Unity/Assets/Mono/ILRuntime/Generate/System_Func_2_String_Byte_Array_Binding.cs
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
<<<<<<< HEAD:Unity/Assets/Mono/ILRuntime/Generate/System_Action_1_Sprite_Binding.cs
            Type type = typeof(System.Action<UnityEngine.Sprite>);
            args = new Type[]{typeof(UnityEngine.Sprite)};
=======
            Type type = typeof(System.Func<System.String, System.Byte[]>);
            args = new Type[]{typeof(System.String)};
>>>>>>> upstream/master:Unity/Assets/Mono/ILRuntime/Generate/System_Func_2_String_Byte_Array_Binding.cs
            method = type.GetMethod("Invoke", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Invoke_0);


        }


        static StackObject* Invoke_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
<<<<<<< HEAD:Unity/Assets/Mono/ILRuntime/Generate/System_Action_1_Sprite_Binding.cs
            UnityEngine.Sprite @obj = (UnityEngine.Sprite)typeof(UnityEngine.Sprite).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<UnityEngine.Sprite> instance_of_this_method = (System.Action<UnityEngine.Sprite>)typeof(System.Action<UnityEngine.Sprite>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Invoke(@obj);
=======
            System.String @arg = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Func<System.String, System.Byte[]> instance_of_this_method = (System.Func<System.String, System.Byte[]>)typeof(System.Func<System.String, System.Byte[]>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Invoke(@arg);
>>>>>>> upstream/master:Unity/Assets/Mono/ILRuntime/Generate/System_Func_2_String_Byte_Array_Binding.cs

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
