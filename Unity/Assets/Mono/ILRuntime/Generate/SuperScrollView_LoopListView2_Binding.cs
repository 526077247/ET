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
    unsafe class SuperScrollView_LoopListView2_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(SuperScrollView.LoopListView2);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("NewListViewItem", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, NewListViewItem_0);
            args = new Type[]{};
            method = type.GetMethod("ClearListView", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ClearListView_1);
            args = new Type[]{typeof(System.Int32), typeof(System.Func<SuperScrollView.LoopListView2, System.Int32, SuperScrollView.LoopListViewItem2>), typeof(SuperScrollView.LoopListViewInitParam)};
            method = type.GetMethod("InitListView", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InitListView_2);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean)};
            method = type.GetMethod("SetListItemCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetListItemCount_3);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("GetShownItemByItemIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetShownItemByItemIndex_4);
            args = new Type[]{typeof(System.Int32), typeof(System.Single)};
            method = type.GetMethod("MovePanelToItemIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MovePanelToItemIndex_5);
            args = new Type[]{};
            method = type.GetMethod("RefreshAllShownItem", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RefreshAllShownItem_6);

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
            field = type.GetField("mOnSnapNearestChanged", flag);
            app.RegisterCLRFieldGetter(field, get_mOnSnapNearestChanged_3);
            app.RegisterCLRFieldSetter(field, set_mOnSnapNearestChanged_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_mOnSnapNearestChanged_3, AssignFromStack_mOnSnapNearestChanged_3);


        }


        static StackObject* NewListViewItem_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @itemPrefabName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            SuperScrollView.LoopListView2 instance_of_this_method = (SuperScrollView.LoopListView2)typeof(SuperScrollView.LoopListView2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.NewListViewItem(@itemPrefabName);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ClearListView_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            SuperScrollView.LoopListView2 instance_of_this_method = (SuperScrollView.LoopListView2)typeof(SuperScrollView.LoopListView2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ClearListView();

            return __ret;
        }

        static StackObject* InitListView_2(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            SuperScrollView.LoopListViewInitParam @initParam = (SuperScrollView.LoopListViewInitParam)typeof(SuperScrollView.LoopListViewInitParam).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Func<SuperScrollView.LoopListView2, System.Int32, SuperScrollView.LoopListViewItem2> @onGetItemByIndex = (System.Func<SuperScrollView.LoopListView2, System.Int32, SuperScrollView.LoopListViewItem2>)typeof(System.Func<SuperScrollView.LoopListView2, System.Int32, SuperScrollView.LoopListViewItem2>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @itemTotalCount = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            SuperScrollView.LoopListView2 instance_of_this_method = (SuperScrollView.LoopListView2)typeof(SuperScrollView.LoopListView2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InitListView(@itemTotalCount, @onGetItemByIndex, @initParam);

            return __ret;
        }

        static StackObject* SetListItemCount_3(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @resetPos = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @itemCount = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            SuperScrollView.LoopListView2 instance_of_this_method = (SuperScrollView.LoopListView2)typeof(SuperScrollView.LoopListView2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetListItemCount(@itemCount, @resetPos);

            return __ret;
        }

        static StackObject* GetShownItemByItemIndex_4(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @itemIndex = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            SuperScrollView.LoopListView2 instance_of_this_method = (SuperScrollView.LoopListView2)typeof(SuperScrollView.LoopListView2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetShownItemByItemIndex(@itemIndex);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* MovePanelToItemIndex_5(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @offset = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @itemIndex = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            SuperScrollView.LoopListView2 instance_of_this_method = (SuperScrollView.LoopListView2)typeof(SuperScrollView.LoopListView2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MovePanelToItemIndex(@itemIndex, @offset);

            return __ret;
        }

        static StackObject* RefreshAllShownItem_6(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            SuperScrollView.LoopListView2 instance_of_this_method = (SuperScrollView.LoopListView2)typeof(SuperScrollView.LoopListView2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RefreshAllShownItem();

            return __ret;
        }


        static object get_mOnBeginDragAction_0(ref object o)
        {
            return ((SuperScrollView.LoopListView2)o).mOnBeginDragAction;
        }

        static StackObject* CopyToStack_mOnBeginDragAction_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((SuperScrollView.LoopListView2)o).mOnBeginDragAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mOnBeginDragAction_0(ref object o, object v)
        {
            ((SuperScrollView.LoopListView2)o).mOnBeginDragAction = (System.Action)v;
        }

        static StackObject* AssignFromStack_mOnBeginDragAction_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @mOnBeginDragAction = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((SuperScrollView.LoopListView2)o).mOnBeginDragAction = @mOnBeginDragAction;
            return ptr_of_this_method;
        }

        static object get_mOnDragingAction_1(ref object o)
        {
            return ((SuperScrollView.LoopListView2)o).mOnDragingAction;
        }

        static StackObject* CopyToStack_mOnDragingAction_1(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((SuperScrollView.LoopListView2)o).mOnDragingAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mOnDragingAction_1(ref object o, object v)
        {
            ((SuperScrollView.LoopListView2)o).mOnDragingAction = (System.Action)v;
        }

        static StackObject* AssignFromStack_mOnDragingAction_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @mOnDragingAction = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((SuperScrollView.LoopListView2)o).mOnDragingAction = @mOnDragingAction;
            return ptr_of_this_method;
        }

        static object get_mOnEndDragAction_2(ref object o)
        {
            return ((SuperScrollView.LoopListView2)o).mOnEndDragAction;
        }

        static StackObject* CopyToStack_mOnEndDragAction_2(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((SuperScrollView.LoopListView2)o).mOnEndDragAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mOnEndDragAction_2(ref object o, object v)
        {
            ((SuperScrollView.LoopListView2)o).mOnEndDragAction = (System.Action)v;
        }

        static StackObject* AssignFromStack_mOnEndDragAction_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @mOnEndDragAction = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((SuperScrollView.LoopListView2)o).mOnEndDragAction = @mOnEndDragAction;
            return ptr_of_this_method;
        }

        static object get_mOnSnapNearestChanged_3(ref object o)
        {
            return ((SuperScrollView.LoopListView2)o).mOnSnapNearestChanged;
        }

        static StackObject* CopyToStack_mOnSnapNearestChanged_3(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((SuperScrollView.LoopListView2)o).mOnSnapNearestChanged;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mOnSnapNearestChanged_3(ref object o, object v)
        {
            ((SuperScrollView.LoopListView2)o).mOnSnapNearestChanged = (System.Action<SuperScrollView.LoopListView2, SuperScrollView.LoopListViewItem2>)v;
        }

        static StackObject* AssignFromStack_mOnSnapNearestChanged_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<SuperScrollView.LoopListView2, SuperScrollView.LoopListViewItem2> @mOnSnapNearestChanged = (System.Action<SuperScrollView.LoopListView2, SuperScrollView.LoopListViewItem2>)typeof(System.Action<SuperScrollView.LoopListView2, SuperScrollView.LoopListViewItem2>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((SuperScrollView.LoopListView2)o).mOnSnapNearestChanged = @mOnSnapNearestChanged;
            return ptr_of_this_method;
        }



    }
}
