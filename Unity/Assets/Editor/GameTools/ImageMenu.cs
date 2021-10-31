
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

/*
 *  编辑器扩展入口类，统一写在这里，便于后面查看，不要乱写在其他地方
 *  
 *  入口统一放在Tools下面，然后根据功能进行划分
 *  
 *  Addressable:  unity addressable 相关编辑器代码
 *  Build: 打包脚本相关代码
 *  Common: 一些编辑器公共代码
 *  GameTools: 一些工具
 *  ImportHelper: 资源导入处理类
 *  UI: UI相关扩展
 *  XLua: XLua相关扩展
 *  XLuaMenu:lua代码相关
 */

public class EditorMenu : Editor
{

    [MenuItem("Tools/ArtTools/搜索或批量替换Sprite", false, 503)]
    public static void ReplaceImage()
    {
        Rect _rect = new Rect(0, 0, 900, 600);
        ReplaceImage window = EditorWindow.GetWindowWithRect<ReplaceImage>(_rect, true, "搜索或批量替换Sprite");
        window.Show();
    }

    [MenuItem("Tools/ArtTools/查找未使用的图片", false, 503)]
    public static void CheckUnUseImage()
    {
        Rect _rect = new Rect(0, 0, 900, 600);
        CheckUnuseImage window = EditorWindow.GetWindowWithRect<CheckUnuseImage>(_rect, true, "查找未使用的图片");
        window.Show();
    }

    [MenuItem("Tools/ArtTools/检查丢失image", false, 504)]
    public static void CheckLossImage()
    {
        Rect _rect = new Rect(0, 0, 900, 600);
        CheckEmptyImage window = EditorWindow.GetWindowWithRect<CheckEmptyImage>(_rect, true, "检查预设丢失image");
        window.Show();
    }
}
