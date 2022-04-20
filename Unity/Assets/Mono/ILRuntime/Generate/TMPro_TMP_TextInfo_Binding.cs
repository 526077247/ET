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
    unsafe class TMPro_TMP_TextInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(TMPro.TMP_TextInfo);

            field = type.GetField("characterInfo", flag);
            app.RegisterCLRFieldGetter(field, get_characterInfo_0);
            app.RegisterCLRFieldSetter(field, set_characterInfo_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_characterInfo_0, AssignFromStack_characterInfo_0);
            field = type.GetField("characterCount", flag);
            app.RegisterCLRFieldGetter(field, get_characterCount_1);
            app.RegisterCLRFieldSetter(field, set_characterCount_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_characterCount_1, AssignFromStack_characterCount_1);


        }



        static object get_characterInfo_0(ref object o)
        {
            return ((TMPro.TMP_TextInfo)o).characterInfo;
        }

        static StackObject* CopyToStack_characterInfo_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((TMPro.TMP_TextInfo)o).characterInfo;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_characterInfo_0(ref object o, object v)
        {
            ((TMPro.TMP_TextInfo)o).characterInfo = (TMPro.TMP_CharacterInfo[])v;
        }

        static StackObject* AssignFromStack_characterInfo_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            TMPro.TMP_CharacterInfo[] @characterInfo = (TMPro.TMP_CharacterInfo[])typeof(TMPro.TMP_CharacterInfo[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((TMPro.TMP_TextInfo)o).characterInfo = @characterInfo;
            return ptr_of_this_method;
        }

        static object get_characterCount_1(ref object o)
        {
            return ((TMPro.TMP_TextInfo)o).characterCount;
        }

        static StackObject* CopyToStack_characterCount_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((TMPro.TMP_TextInfo)o).characterCount;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_characterCount_1(ref object o, object v)
        {
            ((TMPro.TMP_TextInfo)o).characterCount = (System.Int32)v;
        }

        static StackObject* AssignFromStack_characterCount_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @characterCount = ptr_of_this_method->Value;
            ((TMPro.TMP_TextInfo)o).characterCount = @characterCount;
            return ptr_of_this_method;
        }



    }
}
