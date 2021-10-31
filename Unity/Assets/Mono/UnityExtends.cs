using AssetBundles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 功能：unity的一些扩展
/// </summary>
/// 

public class UnityExtends
{
    
    //判断当前的点击事件是否在
    public static Vector2 IsInCurRectTransform(PointerEventData eventData, RectTransform rectTransform)
    {
        Vector2 localPosition;
        bool ret;
        ret = UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPosition);
        return localPosition;
    }

    //射线检测
    public static RaycastHit ScreenRaycast(Camera mainCamera, Vector2 screenPosition, string mask)
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        int layMask = LayerMask.GetMask(mask);
        Physics.Raycast(ray, out hit, 10000,layMask);
        return hit;

    }

    //射线检测
    public static RaycastHit[] ScreenRaycastAll(Camera mainCamera, Vector2 screenPosition, string mask)
    {
        RaycastHit[] hits;
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        int layMask = LayerMask.GetMask(mask);
        hits = Physics.RaycastAll(ray, 10000, layMask);
        return hits;
    }

    //屏幕的点到UI内的坐标
    public static Vector2 ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam)
    {
        Vector2 localPosition;
        bool ret;
        ret = UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, cam, out localPosition);
        return localPosition;
    }

    //屏幕的点到世界坐标
    public static Vector3 ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam)
    {
        Vector3 worldPosition;
        bool ret;
        ret = UnityEngine.RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, cam, out worldPosition);
        return worldPosition;

    }

    //设置layer
    public static void SetObjectLayer(GameObject game_object, int layervalue, bool isIncludeChild)
    {
        if (game_object == null)
        {
            return;
        }
        game_object.layer = layervalue;
        if (isIncludeChild)
        {
            Transform transform = game_object.transform;
            Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < allChildren.Length; i++)
            {
                allChildren[i].gameObject.layer = layervalue;
            }
        }

    }

    //设置cullingmask
    public static void SetLightCullingMask(GameObject game_object, int layervalue, bool isIncludeChild)
    {
        if (game_object == null)
        {
            return;
        }
        Transform transform = game_object.transform;
        Light lihgt = transform.GetComponent<Light>();
        if (lihgt)
        {
            lihgt.cullingMask = layervalue;
        }
        if (isIncludeChild)
        {
            Light[] allChildren = transform.GetComponentsInChildren<Light>(true);
            for (int i = 0; i < allChildren.Length; i++)
            {
                allChildren[i].cullingMask = 1 << layervalue;
            }
        }

    }

    //clone组件
    public static GameObject CloneSelf(GameObject src_object)
    {
        if (src_object == null)
        {
            return null;
        }
        Transform parent = src_object.transform.parent;
        var cloneGo = parent == null ? GameObject.Instantiate(src_object) : GameObject.Instantiate(src_object, parent);
        return cloneGo;
    }

    //UI坐标转换
    public static Vector2 CalPositionAt(RectTransform from, RectTransform at, Camera uiCamera)
    {
        //将from转换到屏幕坐标
        Vector2 V2fromInScreen = RectTransformUtility.WorldToScreenPoint(uiCamera, from.transform.position);
        //将屏幕坐标转换到at的局部坐标中
        Vector2 V2InAt;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(at, V2fromInScreen, uiCamera, out V2InAt);

        return V2InAt;
    }

    public static Vector3 CalWorldPositionToLocalRect(RectTransform rect, Vector3 worldPosition, Camera uiCamera)
    {
        //1.将3D坐标转到屏幕坐标 一般MainCamera是3D对象摄像机  如果不是获取对应摄像机
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, uiCamera, out uiPosition);
        return uiPosition;
    }

    public static void SetBtnGray(GameObject go, Material grayMaterial, bool isGray, bool includeText)
    {
        if (go == null)
        {
            return;
        }
        Material mt = null;
        if (isGray)
        {
            mt = grayMaterial;
        }
        var coms = go.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < coms.Length; i++)
        {
            coms[i].material = mt;
        }

        if (includeText)
        {
            var textComs = go.GetComponentsInChildren<Text>();
            for (int i = 0; i < textComs.Length; i++)
            {
                var uITextColorCtrl = UITextColorCtrl.Get(textComs[i].gameObject);
                if (isGray)
                {
                    uITextColorCtrl.SetTextColor(new Color(89 / 255f, 93 / 255f, 93 / 255f));
                }
                else
                {
                    uITextColorCtrl.ClearTextColor();
                }
            }
        }
    }
    
    public static void SetImageGray(GameObject go, Material grayMaterial, bool isGray)
    {
        if (go == null)
        {
            return;
        }
        Material mt = null;
        if (isGray)
        {
            mt = grayMaterial;
        }

        Image image = null;
        RawImage rawImage = null;
        if (go.TryGetComponent<Image>(out image))
        {
            image.material = mt;
        }

        if (go.TryGetComponent<RawImage>(out rawImage))
        {
            rawImage.material = mt;
        }
    }

    public static string GetInputString()
    {
        return UnityEngine.Input.inputString;
    }

    public static int RecursiveGetChildCount(Transform trans, string path, ref Dictionary<string, int> record)
    {
        int total_child_count = trans.childCount;
        for (int i = 0; i < trans.childCount; i++)
        {
            var child = trans.GetChild(i);
            if (child.name.Contains("Input Caret") || child.name.Contains("TMP SubMeshUI") || child.name.Contains("TMP UI SubObject"))
            {
                //Input控件在运行时会自动生成个光标子控件，而prefab中是没有的，所以得过滤掉
                //TextMesh会生成相应字体子控件
                total_child_count = total_child_count - 1;
            }
            else
            {
                string cpath = path + "/" + child.name;
                if (record.ContainsKey(cpath))
                {
                    record[cpath] += 1;
                }
                else
                {
                    record[cpath] = 1;
                }
                total_child_count += RecursiveGetChildCount(child, cpath, ref record);
            }
        }
        return total_child_count;
    }
    
    //设置图片颜色
    public static void SetImgColor(Component cmp, string colorString)
    {
        if (cmp == null)
        {
            return;
        }
        UnityEngine.Color colors = new UnityEngine.Color();
        ColorUtility.TryParseHtmlString(colorString, out colors);
        Image img = (Image) cmp;
        img.color = colors;
    }
    
    //手机震动
    //public static void DoVibrate()
    //{
    //    Handheld.Vibrate();
    //}
    
    
    //设置横屏
    public static void SetGameLandscape()
    {
        //Debug.LogError("==============SetGameLandscape==========");
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    
    //设置竖屏
    public static void SetGamePortrait()
    {
        //Debug.LogError("==============SetGamePortrait==========");
        Screen.orientation = ScreenOrientation.Portrait;
    }
    
    //获取手机横竖屏状态
    public static bool IsScreenLandscape()
    {
        var isLandscape = Screen.width > Screen.height;
        return isLandscape;
    }

    /// <summary>
    /// 正则字符串验证（Lua的真心不好用）
    /// </summary>
    /// <param name="str"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static bool RegexUtil(string str, string pattern)
    {
        Regex regex = new Regex(pattern);
        return regex.IsMatch(str);
    }

    /// <summary>
    /// 替换字符串
    /// </summary>
    /// <returns></returns>
    public static string ReplaceUtil(string str, string pattern,string param)
    {
       return str.Replace(pattern,param);
    }
    /// <summary>
    /// 是否包含字符串
    /// </summary>
    /// <param name="str">原字符串</param>
    /// <param name="param">包含字符串</param>
    /// <param name="caseSensitive">大小写敏感</param>
    /// <returns></returns>
    public static bool ContainsUtil(string str, string param,bool caseSensitive)
    {
        if (caseSensitive)
            return str.Contains(param);
        else
            return str.ToLower().Contains(param.ToLower());
    }
    /// <summary>
    /// 删除字符串中的中文
    /// </summary>
    public static string DeleteChinese(string str)
    {
        string retValue = str;
        if (System.Text.RegularExpressions.Regex.IsMatch(str, @"[\u4e00-\u9fa5]"))
        {
            retValue = string.Empty;
            var strsStrings = str.ToCharArray();
            for (int index = 0; index < strsStrings.Length; index++)
            {
                if (strsStrings[index] >= 0x4e00 && strsStrings[index] <= 0x9fa5)
                {
                    continue;
                }
                retValue += strsStrings[index];
            }
        }
        return retValue;
    }
    //刷新横向或纵向layout中包含contentSizeFilter的组件
    public static void ContentSizeFilterSetLayout(GameObject obj, int layout)
    {
        var contentSizeFitters = obj.GetComponentsInChildren<ContentSizeFitter>(true);
                
        if (contentSizeFitters != null)
        {
            for (int i = 0; i < contentSizeFitters.Length; i++)
            {
                switch (layout)
                {
                    case 1:
                        contentSizeFitters[i].SetLayoutVertical();
                        break;
                    case 2:
                        contentSizeFitters[i].SetLayoutHorizontal();
                        break;
                    default:
                        contentSizeFitters[i].SetLayoutVertical();
                        contentSizeFitters[i].SetLayoutHorizontal();
                        break;
                }
            }
        }
    }
    
    

    public static string Calculationmd5(string textStr)
    {
        byte[] input = Encoding.Default.GetBytes(textStr.Trim());
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] output = md5.ComputeHash(input);
        string md5_string = System.BitConverter.ToString(output).Replace("-", "");
        return md5_string;
    }

    public static string GetAppVersion() {
        return Application.version;
    }

    public static string GetEngineVersion()
    {
        return AssetBundleConfig.Instance.EngineVer;
    }

    public static string GetResVersion() {
        return AssetBundleConfig.Instance.ResVer;
    }

    public static bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public static void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public static void SetScreenTimeout(int time)
    {
        Screen.sleepTimeout = time;
    }

    public static void ForceUpdateLayout(RectTransform rectTransform)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public static long GetCurTicks()
    {
        return DateTime.Now.Ticks;
    }
}
