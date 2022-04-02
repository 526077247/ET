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
    unsafe class SuperScrollView_LoopGridView_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(SuperScrollView.LoopGridView);
            args = new Type[]{};
            method = type.GetMethod("ClearListView", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ClearListView_0);
            args = new Type[]{typeof(System.Int32), typeof(System.Func<SuperScrollView.LoopGridView, System.Int32, System.Int32, System.Int32, SuperScrollView.LoopGridViewItem>), typeof(SuperScrollView.LoopGridViewSettingParam), typeof(SuperScrollView.LoopGridViewInitParam)};
            method = type.GetMethod("InitGridView", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InitGridView_1);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean)};
            method = type.GetMethod("SetListItemCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetListItemCount_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("GetShownItemByItemIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetShownItemByItemIndex_3);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32), typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("MovePanelToItemByRowColumn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MovePanelToItemByRowColumn_4);
            args = new Type[]{};
            method = type.GetMethod("RefreshAllShownItem", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RefreshAllShownItem_5);
            args = new Type[]{typeof(UnityEngine.Vector2)};
            method = type.GetMethod("SetItemSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetItemSize_6);

            field = type.GetField("mOnBeginDragAction", flag);
            app.RegisterCLRFieldGetter(field, get_mOnBeginDragAction_0);
            app.RegisterCLRFieldSetter(field, set_mOnBeginDragAction_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_mOnBeginDragAction_0, AssignFromStack_mOnBeginDragAction_0);
            field = type.GetField("mOnDragingAction", flag);
            app.RegisterCLRFieldGetter(field, get_mOnDragingAction_1);
            app.RegisterCLRFieldSetter(field, set_mOnDragingAction_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_mOnDragingAction_1, AssignFromStack_mOnDragingAction_1);
            field = type.GetField("mOnEndDragAction", flag);
            app.RegisterCLRFieldGetter(field, get_mOnEndDragAction_2);
            app.RegisterCLRFieldSetter(field, set_mOnEndDragAction_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_mOnEndDragAction_2, AssignFromStack_mOnEndDragAction_2);


        }


        static StackObject* ClearListView_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            SuperScrollView.LoopGridView instance_of_this_method = (SuperScrollView.LoopGridView)typeof(SuperScrollView.LoopGridView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ClearListView();

            return __ret;
        }

        static StackObject* InitGridView_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            SuperScrollView.LoopGridViewInitParam @initParam = (SuperScrollView.LoopGridViewInitParam)typeof(SuperScrollView.LoopGridViewInitParam).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            SuperScrollView.LoopGridViewSettingParam @settingParam = (SuperScrollView.LoopGridViewSettingParam)typeof(SuperScrollView.LoopGridViewSettingParam).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Func<SuperScrollView.LoopGridView, System.Int32, System.Int32, System.Int32, SuperScrollView.LoopGridViewItem> @onGetItemByRowColumn = (System.Func<SuperScrollView.LoopGridView, System.Int32, System.Int32, System.Int32, SuperScrollView.LoopGridViewItem>)typeof(System.Func<SuperScrollView.LoopGridView, System.Int32, System.Int32, System.Int32, SuperScrollView.LoopGridViewItem>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Int32 @itemTotalCount = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            SuperScrollView.LoopGridView instance_of_this_method = (SuperScrollView.LoopGridView)typeof(SuperScrollView.LoopGridView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InitGridView(@itemTotalCount, @onGetItemByRowColumn, @settingParam, @initParam);

            return __ret;
        }

        static StackObject* SetListItemCount_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @resetPos = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @itemCount = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            SuperScrollView.LoopGridView instance_of_this_method = (SuperScrollView.LoopGridView)typeof(SuperScrollView.LoopGridView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetListItemCount(@itemCount, @resetPos);

            return __ret;
        }

        static StackObject* GetShownItemByItemIndex_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @itemIndex = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            SuperScrollView.LoopGridView instance_of_this_method = (SuperScrollView.LoopGridView)typeof(SuperScrollView.LoopGridView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetShownItemByItemIndex(@itemIndex);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* MovePanelToItemByRowColumn_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @offsetY = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @offsetX = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @column = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Int32 @row = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            SuperScrollView.LoopGridView instance_of_this_method = (SuperScrollView.LoopGridView)typeof(SuperScrollView.LoopGridView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MovePanelToItemByRowColumn(@row, @column, @offsetX, @offsetY);

            return __ret;
        }

        static StackObject* RefreshAllShownItem_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            SuperScrollView.LoopGridView instance_of_this_method = (SuperScrollView.LoopGridView)typeof(SuperScrollView.LoopGridView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RefreshAllShownItem();

            return __ret;
        }

        static StackObject* SetItemSize_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector2 @newSize = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @newSize, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @newSize = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            SuperScrollView.LoopGridView instance_of_this_method = (SuperScrollView.LoopGridView)typeof(SuperScrollView.LoopGridView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetItemSize(@newSize);

            return __ret;
        }


        static object get_mOnBeginDragAction_0(ref object o)
        {
            return ((SuperScrollView.LoopGridView)o).mOnBeginDragAction;
        }

        static StackObject* CopyToStack_mOnBeginDragAction_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((SuperScrollView.LoopGridView)o).mOnBeginDragAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mOnBeginDragAction_0(ref object o, object v)
        {
            ((SuperScrollView.LoopGridView)o).mOnBeginDragAction = (System.Action<UnityEngine.EventSystems.PointerEventData>)v;
        }

        static StackObject* AssignFromStack_mOnBeginDragAction_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.EventSystems.PointerEventData> @mOnBeginDragAction = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((SuperScrollView.LoopGridView)o).mOnBeginDragAction = @mOnBeginDragAction;
            return ptr_of_this_method;
        }

        static object get_mOnDragingAction_1(ref object o)
        {
            return ((SuperScrollView.LoopGridView)o).mOnDragingAction;
        }

        static StackObject* CopyToStack_mOnDragingAction_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((SuperScrollView.LoopGridView)o).mOnDragingAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mOnDragingAction_1(ref object o, object v)
        {
            ((SuperScrollView.LoopGridView)o).mOnDragingAction = (System.Action<UnityEngine.EventSystems.PointerEventData>)v;
        }

        static StackObject* AssignFromStack_mOnDragingAction_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.EventSystems.PointerEventData> @mOnDragingAction = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((SuperScrollView.LoopGridView)o).mOnDragingAction = @mOnDragingAction;
            return ptr_of_this_method;
        }

        static object get_mOnEndDragAction_2(ref object o)
        {
            return ((SuperScrollView.LoopGridView)o).mOnEndDragAction;
        }

        static StackObject* CopyToStack_mOnEndDragAction_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((SuperScrollView.LoopGridView)o).mOnEndDragAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mOnEndDragAction_2(ref object o, object v)
        {
            ((SuperScrollView.LoopGridView)o).mOnEndDragAction = (System.Action<UnityEngine.EventSystems.PointerEventData>)v;
        }

        static StackObject* AssignFromStack_mOnEndDragAction_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.EventSystems.PointerEventData> @mOnEndDragAction = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((SuperScrollView.LoopGridView)o).mOnEndDragAction = @mOnEndDragAction;
            return ptr_of_this_method;
        }



    }
}
