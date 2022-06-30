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
    unsafe class ET_SoundManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ET.SoundManager);
            args = new Type[]{};
            method = type.GetMethod("CreateClipSource", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CreateClipSource_0);

            field = type.GetField("Instance", flag);
            app.RegisterCLRFieldGetter(field, get_Instance_0);
            app.RegisterCLRFieldSetter(field, set_Instance_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Instance_0, AssignFromStack_Instance_0);
            field = type.GetField("Sound", flag);
            app.RegisterCLRFieldGetter(field, get_Sound_1);
            app.RegisterCLRFieldSetter(field, set_Sound_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Sound_1, AssignFromStack_Sound_1);
            field = type.GetField("BGM", flag);
            app.RegisterCLRFieldGetter(field, get_BGM_2);
            app.RegisterCLRFieldSetter(field, set_BGM_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_BGM_2, AssignFromStack_BGM_2);
            field = type.GetField("m_bgm", flag);
            app.RegisterCLRFieldGetter(field, get_m_bgm_3);
            app.RegisterCLRFieldSetter(field, set_m_bgm_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_bgm_3, AssignFromStack_m_bgm_3);


        }


        static StackObject* CreateClipSource_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ET.SoundManager instance_of_this_method = (ET.SoundManager)typeof(ET.SoundManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CreateClipSource();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_Instance_0(ref object o)
        {
            return ET.SoundManager.Instance;
        }

        static StackObject* CopyToStack_Instance_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ET.SoundManager.Instance;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Instance_0(ref object o, object v)
        {
            ET.SoundManager.Instance = (ET.SoundManager)v;
        }

        static StackObject* AssignFromStack_Instance_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            ET.SoundManager @Instance = (ET.SoundManager)typeof(ET.SoundManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ET.SoundManager.Instance = @Instance;
            return ptr_of_this_method;
        }

        static object get_Sound_1(ref object o)
        {
            return ((ET.SoundManager)o).Sound;
        }

        static StackObject* CopyToStack_Sound_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((ET.SoundManager)o).Sound;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Sound_1(ref object o, object v)
        {
            ((ET.SoundManager)o).Sound = (UnityEngine.Audio.AudioMixer)v;
        }

        static StackObject* AssignFromStack_Sound_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Audio.AudioMixer @Sound = (UnityEngine.Audio.AudioMixer)typeof(UnityEngine.Audio.AudioMixer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ET.SoundManager)o).Sound = @Sound;
            return ptr_of_this_method;
        }

        static object get_BGM_2(ref object o)
        {
            return ((ET.SoundManager)o).BGM;
        }

        static StackObject* CopyToStack_BGM_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((ET.SoundManager)o).BGM;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_BGM_2(ref object o, object v)
        {
            ((ET.SoundManager)o).BGM = (UnityEngine.Audio.AudioMixer)v;
        }

        static StackObject* AssignFromStack_BGM_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Audio.AudioMixer @BGM = (UnityEngine.Audio.AudioMixer)typeof(UnityEngine.Audio.AudioMixer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ET.SoundManager)o).BGM = @BGM;
            return ptr_of_this_method;
        }

        static object get_m_bgm_3(ref object o)
        {
            return ((ET.SoundManager)o).m_bgm;
        }

        static StackObject* CopyToStack_m_bgm_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((ET.SoundManager)o).m_bgm;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_bgm_3(ref object o, object v)
        {
            ((ET.SoundManager)o).m_bgm = (UnityEngine.AudioSource)v;
        }

        static StackObject* AssignFromStack_m_bgm_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.AudioSource @m_bgm = (UnityEngine.AudioSource)typeof(UnityEngine.AudioSource).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((ET.SoundManager)o).m_bgm = @m_bgm;
            return ptr_of_this_method;
        }



    }
}
