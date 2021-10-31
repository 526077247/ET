using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.ProjectWindowCallback;
using System.Text;
using System.Text.RegularExpressions;
 
// 通过Project 窗口Create 按钮添加脚本模板
public class ScriptTemplate {
	// 脚本模板所在目录
	private const string MY_SCRIPT_DEFAULT = "Assets/Editor/ScriptTemplates/C# Script-UIBaseViewScript.cs.txt";
 
	[MenuItem("Assets/Create/C# UIBaseView", false, 80)]
	static void CreateMyScript(){
		string locationPath = GetSeletedPathOrFallback();
		ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
			ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
			locationPath + "/NewUIBaseView.cs",
			null, MY_SCRIPT_DEFAULT);
	}
	static string GetSeletedPathOrFallback(){
		string path = "Assets";
		foreach(UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)){
			path = AssetDatabase.GetAssetPath(obj);
			if(!string.IsNullOrEmpty(path) && File.Exists(path)){
				path = Path.GetDirectoryName(path);
				break;
			}
		}
		return path;
	}
}
// 继承EndNameEditAction 监听用户输入文件名
class MyDoCreateScriptAsset : EndNameEditAction{
	// 后两个参数是ProjectWindowUtil.StartNameEditingIfProjectWindowExists 传入
	public override void Action(int instanceId, string pathName, string resourceFile){
		Debug.Log("pathname: " + pathName);//pathname: Assets/MyNewBehaviourScript.cs
		Debug.Log("resourceFile: " + resourceFile);//resourceFile: Assets/Editor/ScriptTemplates/C# Script-MyNewBehaviourScript.cs.txt
		UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
		ProjectWindowUtil.ShowCreatedAsset(o);
	}
	internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile){
		string fullPath = Path.GetFullPath(pathName);
		Debug.Log("fullPath: " + fullPath);//fullPath: E:\UnityFramework\QFramework\QFamework\unit3d_yxkf\Assets\MyNewBehaviourScript.cs
		StreamReader streamReader = new StreamReader(resourceFile);
		string text = streamReader.ReadToEnd();
		streamReader.Close();
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
		// 替换文件名
		text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
		bool encoderShouleEmitUTF8Identifier = true;
		bool throwOnInvalidBytes = false;
		UTF8Encoding encoding = new UTF8Encoding(encoderShouleEmitUTF8Identifier, throwOnInvalidBytes);
		bool append = false;
		StreamWriter sw = new StreamWriter(fullPath, append, encoding);
		sw.Write(text);
		sw.Close();
		AssetDatabase.ImportAsset(pathName);
		return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
	}
}
