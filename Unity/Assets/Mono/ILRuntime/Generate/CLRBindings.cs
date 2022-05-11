using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }

        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2> s_UnityEngine_Vector2_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3> s_UnityEngine_Vector3_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion> s_UnityEngine_Quaternion_Binding_Binder = null;

        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_String_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            ET_Log_Binding.Register(app);
            ET_ListComponent_1_Int32_Binding.Register(app);
            ET_ListComponent_1_ILTypeInstance_Binding.Register(app);
            System_Single_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Int32_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Binding.Register(app);
            System_Math_Binding.Register(app);
            System_Char_Binding.Register(app);
            ET_Define_Binding.Register(app);
            UnityEngine_Networking_UnityWebRequest_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_String_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_String_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Text_Encoding_Binding.Register(app);
            UnityEngine_JsonUtility_Binding.Register(app);
            UnityEngine_Networking_UploadHandlerRaw_Binding.Register(app);
            UnityEngine_Networking_DownloadHandlerBuffer_Binding.Register(app);
            UnityEngine_Networking_UnityWebRequestTexture_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_String_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_String_Array_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_List_1_WhiteConfig_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_UpdateConfig_Binding.Register(app);
            System_Security_Cryptography_MD5CryptoServiceProvider_Binding.Register(app);
            System_Security_Cryptography_HashAlgorithm_Binding.Register(app);
            System_BitConverter_Binding.Register(app);
            UnityEngine_Application_Binding.Register(app);
            GameUtility_Binding.Register(app);
            System_Uri_Binding.Register(app);
            System_Text_StringBuilder_Binding.Register(app);
            System_Object_Binding.Register(app);
            ET_AcceptAllCertificate_Binding.Register(app);
            ET_ETTask_1_Boolean_Binding.Register(app);
            UnityEngine_Networking_DownloadHandler_Binding.Register(app);
            ET_StringHelper_Binding.Register(app);
            LitJson_JsonMapper_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            ET_TimeHelper_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_Binding.Register(app);
            ET_ETTask_1_Int32_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            System_Int64_Binding.Register(app);
            System_Int32_Binding.Register(app);
            UnityEngine_PlayerPrefs_Binding.Register(app);
            ET_ETTask_1_ILTypeInstance_Binding.Register(app);
            ET_NetworkHelper_Binding.Register(app);
            ET_RandomHelper_Binding.Register(app);
            ET_ETTask_1_String_Binding.Register(app);
            System_Exception_Binding.Register(app);
            ET_ETTask_Binding.Register(app);
            System_Action_Binding.Register(app);
            ET_ListComponent_1_Vector3_Binding.Register(app);
            System_Collections_Generic_List_1_Single_Binding.Register(app);
            System_Collections_Generic_List_1_Vector3_Binding.Register(app);
            UnityEngine_Quaternion_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_Boolean_Binding.Register(app);
            System_Action_1_Boolean_Binding.Register(app);
            System_Collections_Generic_List_1_Vector3_Binding_Enumerator_Binding.Register(app);
            ET_ETCancellationToken_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_Int32_Binding.Register(app);
            ET_ListComponent_1_ETTask_Binding.Register(app);
            System_Collections_Generic_List_1_ETTask_Binding.Register(app);
            ET_ETTaskHelper_Binding.Register(app);
            System_IO_MemoryStream_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_List_1_Int64_Binding.Register(app);
            System_Collections_Generic_List_1_Int64_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_SortedDictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_SortedDictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_SortedDictionary_2_Int32_ILTypeInstance_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_SortedDictionary_2_Int32_ILTypeInstance_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Collections_Generic_List_1_Type_Binding.Register(app);
            System_Collections_Generic_List_1_Type_Binding_Enumerator_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Byte_List_1_ILTypeInstance_Binding.Register(app);
            System_Nullable_1_Vector3_Binding.Register(app);
            System_Nullable_1_Quaternion_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ILTypeInstance_Int32_Binding.Register(app);
            System_DateTime_Binding.Register(app);
            System_Diagnostics_StackTrace_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Byte_List_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Byte_List_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_Byte_Binding.Register(app);
            ET_DictionaryComponent_2_ILTypeInstance_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ILTypeInstance_Int32_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_ILTypeInstance_Int32_Binding.Register(app);
            ET_DictionaryComponent_2_Byte_Boolean_Binding.Register(app);
            ET_HashSetComponent_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Byte_Boolean_Binding.Register(app);
            System_Collections_Generic_HashSet_1_ILTypeInstance_Binding.Register(app);
            ET_ListComponent_1_String_Binding.Register(app);
            System_Boolean_Binding.Register(app);
            System_Action_2_ILTypeInstance_Int32_Binding.Register(app);
            System_Collections_Generic_HashSet_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            ET_DictionaryComponent_2_Int32_ILTypeInstance_Binding.Register(app);
            ET_DictionaryComponent_2_Int32_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_List_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Array_Binding.Register(app);
            ET_DictionaryComponent_2_Int32_List_1_Object_Array_Binding.Register(app);
            ET_DictionaryComponent_2_Int32_List_1_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_List_1_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_List_1_Object_Array_Binding.Register(app);
            System_Collections_Generic_List_1_Object_Array_Binding.Register(app);
            ET_ETTask_1_TextAsset_Binding.Register(app);
            UnityEngine_TextAsset_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Int64_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_Object_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Byte_Array_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            ET_ListComponent_1_Task_Binding.Register(app);
            System_Threading_Tasks_Task_Binding.Register(app);
            System_Collections_Generic_List_1_Task_Binding.Register(app);
            System_Runtime_CompilerServices_TaskAwaiter_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_UInt16_List_1_ILTypeInstance_Binding.Register(app);
            ET_KService_Binding.Register(app);
            ET_AService_Binding.Register(app);
            ET_ThreadSynchronizationContext_Binding.Register(app);
            ET_ForeachHelper_Binding.Register(app);
            System_Collections_Generic_HashSet_1_AService_Binding.Register(app);
            ET_TimeInfo_Binding.Register(app);
            ET_RpcException_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Int64_Binding.Register(app);
            System_Func_2_String_Byte_Array_Binding.Register(app);
            ET_Recast_Binding.Register(app);
            ET_MathHelper_Binding.Register(app);
            AssetBundles_AddressablesManager_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_TextAsset_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_GameObject_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_SpriteAtlas_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_Sprite_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_Material_Binding.Register(app);
            System_Action_1_TextAsset_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            System_Action_1_GameObject_Binding.Register(app);
            ET_ETTask_1_GameObject_Binding.Register(app);
            System_Action_1_SpriteAtlas_Binding.Register(app);
            ET_ETTask_1_SpriteAtlas_Binding.Register(app);
            System_Action_1_Sprite_Binding.Register(app);
            ET_ETTask_1_Sprite_Binding.Register(app);
            System_Action_1_Material_Binding.Register(app);
            ET_ETTask_1_Material_Binding.Register(app);
            System_Net_IPAddress_Binding.Register(app);
            System_Net_IPEndPoint_Binding.Register(app);
            System_Net_Sockets_Socket_Binding.Register(app);
            System_Runtime_InteropServices_OSPlatform_Binding.Register(app);
            System_Runtime_InteropServices_RuntimeInformation_Binding.Register(app);
            System_Byte_Binding.Register(app);
            System_Convert_Binding.Register(app);
            ET_ETTask_1_String_Array_Binding.Register(app);
            System_UInt32_Binding.Register(app);
            ET_ByteHelper_Binding.Register(app);
            System_Collections_Generic_List_1_WhiteConfig_Binding.Register(app);
            System_Collections_Generic_List_1_WhiteConfig_Binding_Enumerator_Binding.Register(app);
            ET_WhiteConfig_Binding.Register(app);
            ET_PlatformUtil_Binding.Register(app);
            ET_UpdateConfig_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_AppConfig_Binding.Register(app);
            ET_AppConfig_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Resver_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Resver_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_Resver_Binding.Register(app);
            ET_VersionCompare_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Dictionary_2_String_Resver_Binding.Register(app);
            ET_Resver_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_ILTypeInstance_Binding.Register(app);
            I18NBridge_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_ILTypeInstance_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_ILTypeInstance_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ListComponent_1_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ListComponent_1_String_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ListComponent_1_String_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_Dictionary_2_String_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_Int32_Binding.Register(app);
            System_Linq_Enumerable_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Dictionary_2_Type_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Type_ILTypeInstance_Binding.Register(app);
            System_Action_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Byte_LinkedList_1_String_Binding.Register(app);
            System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Binding.Register(app);
            System_Collections_Generic_LinkedList_1_String_Binding.Register(app);
            System_Collections_Generic_LinkedListNode_1_String_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Byte_LinkedList_1_String_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Byte_LinkedList_1_String_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Boolean_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Array_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_List_1_ILTypeInstance_Binding.Register(app);
            ET_ETTaskCompleted_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Dictionary_2_String_String_Binding.Register(app);
            ET_ETTask_1_KeyCode_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Vector3_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_LayerMask_Binding.Register(app);
            ET_InputHelper_Binding.Register(app);
            UnityEngine_Camera_Binding.Register(app);
            UnityEngine_Input_Binding.Register(app);
            UnityEngine_Physics_Binding.Register(app);
            UnityEngine_RaycastHit_Binding.Register(app);
            ET_CodeLoader_Binding.Register(app);
            ET_SoundManager_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            System_Collections_Generic_List_1_AudioSource_Binding.Register(app);
            System_Collections_ArrayList_Binding.Register(app);
            UnityEngine_Audio_AudioMixer_Binding.Register(app);
            System_Collections_Hashtable_Binding.Register(app);
            UnityEngine_AudioSource_Binding.Register(app);
            ET_ETTask_1_AudioClip_Binding.Register(app);
            UnityEngine_AudioClip_Binding.Register(app);
            SuperScrollView_LoopListView2_Binding.Register(app);
            SuperScrollView_LoopListViewItem2_Binding.Register(app);
            System_Action_1_Int32_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_Int64_Binding.Register(app);
            ET_ETAsyncTaskMethodBuilder_1_Dictionary_2_String_String_Binding.Register(app);
            AssetBundles_AssetBundleConfig_Binding.Register(app);
            ET_ETTask_1_List_1_WhiteConfig_Binding.Register(app);
            ET_ETTask_1_UpdateConfig_Binding.Register(app);
            AssetBundleMgr_Binding.Register(app);
            ET_ETTask_1_Int64_Binding.Register(app);
            System_Double_Binding.Register(app);
            ET_ETTask_1_Dictionary_2_String_String_Binding.Register(app);
            ET_KeyListener_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            UnityEngine_Color_Binding.Register(app);
            UnityEngine_Rect_Binding.Register(app);
            System_Nullable_1_Int32_Binding.Register(app);
            DG_Tweening_DOTween_Binding.Register(app);
            UnityEngine_Animator_Binding.Register(app);
            UnityEngine_RuntimeAnimatorController_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_AnimationClip_Binding.Register(app);
            UnityEngine_AnimatorControllerParameter_Binding.Register(app);
            System_Collections_Generic_HashSet_1_String_Binding.Register(app);
            UnityEngine_AnimationClip_Binding.Register(app);
            UnitIdComponent_Binding.Register(app);
            UnityEngine_Collider_Binding.Register(app);
            ReferenceCollector_Binding.Register(app);
            TMPro_TMP_Text_Binding.Register(app);
            UnityEngine_UI_Image_Binding.Register(app);
            RaycastHelper_Binding.Register(app);
            System_Action_1_Vector3_Binding.Register(app);
            UnityEngine_UI_Graphic_Binding.Register(app);
            UnityEngine_Cursor_Binding.Register(app);
            UnityEngine_Ray_Binding.Register(app);
            ET_ListComponent_1_ETTask_1_GameObject_Binding.Register(app);
            System_Collections_Generic_List_1_ETTask_1_GameObject_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Int64_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_Int64_Binding.Register(app);
            UnityEngine_Rendering_Universal_CameraExtensions_Binding.Register(app);
            UnityEngine_Rendering_Universal_UniversalAdditionalCameraData_Binding.Register(app);
            System_Collections_Generic_List_1_Camera_Binding.Register(app);
            BestHTTP_HTTPUpdateDelegator_Binding.Register(app);
            BestHTTP_HTTPManager_Binding.Register(app);
            System_Collections_Generic_LinkedList_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_FileStream_Binding.Register(app);
            System_IO_File_Binding.Register(app);
            System_Collections_Generic_LinkedListNode_1_ILTypeInstance_Binding.Register(app);
            System_Threading_ThreadPool_Binding.Register(app);
            BestHTTP_HTTPRequest_Binding.Register(app);
            System_TimeSpan_Binding.Register(app);
            BestHTTP_HTTPResponse_Binding.Register(app);
            System_IO_FileStream_Binding.Register(app);
            System_IO_Stream_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Int64_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_Int64_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_FileStream_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_FileStream_Binding.Register(app);
            System_Threading_Thread_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_List_1_GameObject_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_GameObject_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Dictionary_2_String_Int32_Binding.Register(app);
            System_Collections_Generic_List_1_GameObject_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_List_1_GameObject_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_List_1_GameObject_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Int32_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_Int32_Binding.Register(app);
            UnityEngine_U2D_SpriteAtlas_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Queue_1_Action_1_Sprite_Binding.Register(app);
            System_Collections_Generic_Queue_1_Action_1_Sprite_Binding.Register(app);
            UnityEngine_ImageConversion_Binding.Register(app);
            UnityEngine_Networking_DownloadHandlerTexture_Binding.Register(app);
            UnityEngine_Texture_Binding.Register(app);
            UnityEngine_Sprite_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Material_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ILTypeInstance_ILTypeInstance_Binding.Register(app);
            System_Xml_XmlDocument_Binding.Register(app);
            System_Xml_XmlNode_Binding.Register(app);
            System_Xml_XmlNodeList_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            System_Xml_XmlAttributeCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_List_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ILTypeInstance_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_ILTypeInstance_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_Object_Binding.Register(app);
            System_GC_Binding.Register(app);
            UnityEngine_Resources_Binding.Register(app);
            UnityEngine_AsyncOperation_Binding.Register(app);
            ET_DictionaryComponent_2_Int64_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_Int32_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int64_Int32_Binding.Register(app);
            System_Action_1_Single_Binding.Register(app);
            UnityEngine_UI_Button_Binding.Register(app);
            UnityEngine_Events_UnityEvent_Binding.Register(app);
            UnityEngine_Events_UnityAction_Binding.Register(app);
            UnityEngine_Behaviour_Binding.Register(app);
            UnityEngine_UI_Selectable_Binding.Register(app);
            TextColorCtrl_Binding.Register(app);
            ET_CopyGameObject_Binding.Register(app);
            UnityEngine_UI_Dropdown_Binding.Register(app);
            UnityEngine_Events_UnityEvent_1_Int32_Binding.Register(app);
            UnityEngine_ColorUtility_Binding.Register(app);
            BgAutoFit_Binding.Register(app);
            UnityEngine_UI_InputField_Binding.Register(app);
            TMPro_TMP_InputField_Binding.Register(app);
            SuperScrollView_LoopGridView_Binding.Register(app);
            PointerClick_Binding.Register(app);
            UnityEngine_UI_RawImage_Binding.Register(app);
            BgRawAutoFit_Binding.Register(app);
            UnityEngine_UI_Slider_Binding.Register(app);
            UnityEngine_Events_UnityEvent_1_Single_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            TMPro_TMP_TextInfo_Binding.Register(app);
            TMPro_TMP_CharacterInfo_Binding.Register(app);
            TMPro_TMP_Vertex_Binding.Register(app);
            UnityEngine_Canvas_Binding.Register(app);
            UnityEngine_UI_CanvasScaler_Binding.Register(app);
            UnityEngine_Screen_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Byte_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Byte_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Byte_ILTypeInstance_Binding.Register(app);
            ET_Options_Binding.Register(app);
            System_Collections_Generic_List_1_Action_Binding.Register(app);
            System_Collections_Generic_List_1_Action_Binding_Enumerator_Binding.Register(app);
            ET_MonoPool_Binding.Register(app);
            System_Reflection_FieldInfo_Binding.Register(app);
            System_Reflection_PropertyInfo_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Type_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Type_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_Type_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_List_1_Type_Binding.Register(app);
            ET_UnOrderMultiMap_2_Type_Type_Binding.Register(app);
            ET_UnOrderMultiMap_2_Type_Object_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_List_1_Object_Binding.Register(app);
            System_Collections_Generic_List_1_Object_Binding_t1.Register(app);
            System_Reflection_Assembly_Binding.Register(app);
            System_Reflection_AssemblyName_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Assembly_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Assembly_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Assembly_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Queue_1_Int64_Binding.Register(app);
            ET_ObjectHelper_Binding.Register(app);
            System_Collections_Generic_HashSet_1_Type_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int64_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_HashSet_1_Type_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_IEnumerable_1_KeyValuePair_2_Type_Int32_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_KeyValuePair_2_Type_Int32_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Type_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_UnOrderMultiMap_2_Type_Object_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_Queue_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Queue_1_ILTypeInstance_Binding.Register(app);
            ProtoBuf_Meta_RuntimeTypeModel_Binding.Register(app);
            ProtoBuf_Meta_TypeModel_Binding.Register(app);
            ProtoBuf_Serializer_Binding.Register(app);
            ET_MultiMap_2_Int64_Int64_Binding.Register(app);
            System_Collections_Generic_SortedDictionary_2_Int64_List_1_Int64_Binding.Register(app);
            ET_WrapVector3_Binding.Register(app);
            ET_WrapQuaternion_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            System_Func_3_ILTypeInstance_ILTypeInstance_Int32_Binding.Register(app);
            System_Threading_ReaderWriterLockSlim_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_GameObject_Binding.Register(app);
            System_Func_3_String_GameObject_Boolean_Binding.Register(app);
            System_Action_2_String_GameObject_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_GameObject_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_GameObject_Binding.Register(app);
            System_Func_3_String_ILTypeInstance_Boolean_Binding.Register(app);
            System_Action_2_String_ILTypeInstance_Binding.Register(app);
            System_ValueTuple_3_Int32_Int64_Int32_Binding.Register(app);
            System_Collections_Generic_Queue_1_ValueTuple_3_Int32_Int64_Int32_Binding.Register(app);
            ET_MultiMap_2_Int64_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_SortedDictionary_2_Int64_List_1_ILTypeInstance_Binding.Register(app);
            System_ValueTuple_2_UInt16_MemoryStream_Binding.Register(app);
            System_Collections_Generic_HashSet_1_UInt16_Binding.Register(app);
            ET_ILog_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_UInt16_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_UInt16_Type_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_Type_Binding.Register(app);
            ET_ErrorCore_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_Boolean_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_TextAsset_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_TextAsset_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_TextAsset_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector2));
            s_UnityEngine_Vector2_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector3));
            s_UnityEngine_Vector3_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Quaternion));
            s_UnityEngine_Quaternion_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion>;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            s_UnityEngine_Vector2_Binding_Binder = null;
            s_UnityEngine_Vector3_Binding_Binder = null;
            s_UnityEngine_Quaternion_Binding_Binder = null;
        }
    }
}
