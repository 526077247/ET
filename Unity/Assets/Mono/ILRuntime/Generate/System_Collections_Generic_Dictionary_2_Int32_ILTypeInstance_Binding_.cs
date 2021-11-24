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
<<<<<<< HEAD:Unity/Assets/Mono/ILRuntime/Generate/System_Collections_Generic_Dictionary_2_Int32_ET_ISupportInitializeAd_t1.cs
    unsafe class System_Collections_Generic_Dictionary_2_Int32_ET_ISupportInitializeAdapter_Binding_Adapter_Binding_Enumerator_Binding
=======
    unsafe class System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_ValueCollection_Binding
>>>>>>> 65be985a2fc6ec3d19fb4439b70466975ced1b08:Unity/Assets/Mono/ILRuntime/Generate/System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_.cs
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
<<<<<<< HEAD:Unity/Assets/Mono/ILRuntime/Generate/System_Collections_Generic_Dictionary_2_Int32_ET_ISupportInitializeAd_t1.cs
            Type type = typeof(System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator);
=======
            Type type = typeof(System.Collections.Generic.Dictionary<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>.ValueCollection);
>>>>>>> 65be985a2fc6ec3d19fb4439b70466975ced1b08:Unity/Assets/Mono/ILRuntime/Generate/System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_.cs
            args = new Type[]{};
            method = type.GetMethod("get_Current", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Current_0);
            args = new Type[]{};
            method = type.GetMethod("MoveNext", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MoveNext_1);

            app.RegisterCLRCreateDefaultInstance(type, () => new System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator());


        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, IList<object> __mStack, ref System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator instance_of_this_method)
        {
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            switch(ptr_of_this_method->ObjectType)
            {
                case ObjectTypes.Object:
                    {
                        __mStack[ptr_of_this_method->Value] = instance_of_this_method;
                    }
                    break;
                case ObjectTypes.FieldReference:
                    {
                        var ___obj = __mStack[ptr_of_this_method->Value];
                        if(___obj is ILTypeInstance)
                        {
                            ((ILTypeInstance)___obj)[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            var t = __domain.GetType(___obj.GetType()) as CLRType;
                            t.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, instance_of_this_method);
                        }
                    }
                    break;
                case ObjectTypes.StaticFieldReference:
                    {
                        var t = __domain.GetType(ptr_of_this_method->Value);
                        if(t is ILType)
                        {
                            ((ILType)t).StaticInstance[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            ((CLRType)t).SetStaticFieldValue(ptr_of_this_method->ValueLow, instance_of_this_method);
                        }
                    }
                    break;
                 case ObjectTypes.ArrayReference:
                    {
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }

        static StackObject* get_Current_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
<<<<<<< HEAD:Unity/Assets/Mono/ILRuntime/Generate/System_Collections_Generic_Dictionary_2_Int32_ET_ISupportInitializeAd_t1.cs
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator instance_of_this_method = (System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator)typeof(System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
=======
            System.Collections.Generic.Dictionary<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>.ValueCollection instance_of_this_method = (System.Collections.Generic.Dictionary<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>.ValueCollection)typeof(System.Collections.Generic.Dictionary<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>.ValueCollection).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);
>>>>>>> 65be985a2fc6ec3d19fb4439b70466975ced1b08:Unity/Assets/Mono/ILRuntime/Generate/System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_.cs

            var result_of_this_method = instance_of_this_method.Current;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);

            __intp.Free(ptr_of_this_method);
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* MoveNext_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator instance_of_this_method = (System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator)typeof(System.Collections.Generic.Dictionary<System.Int32, ET.ISupportInitializeAdapter.Adapter>.Enumerator).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);

            var result_of_this_method = instance_of_this_method.MoveNext();

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);

            __intp.Free(ptr_of_this_method);
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }



    }
}
