using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using OfficeOpenXml;
using ProtoBuf;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace ET
{
    public enum ConfigType:byte
    {
        Client,
        Server,
    }

    struct HeadInfo
    {
        public string FieldAttribute;
        public string FieldDesc;
        public string FieldName;
        public string FieldType;

        public HeadInfo(string cs, string desc, string name, string type)
        {
            this.FieldAttribute = cs;
            this.FieldDesc = desc;
            this.FieldName = name;
            this.FieldType = type.Replace(" ","").ToLower();
        }
    }

    public static class ExcelExporter
    {
        private static string template;

        private const string clientClassDir = "../Unity/Codes/Model/Generate/Config";
        private static string ClientClassDir
        {
            get
            {
                if (IsCheck) return "./Temp/ClientClass";
                return clientClassDir;
            }
        }
        private const string serverClassDir = "../Server/Model/Generate/Config";
        private static string ServerClassDir
        {
            get
            {
                if (IsCheck) return "./Temp/ServerClass";
                return serverClassDir;
            }
        }
        private const string excelDir = "../Excel";

        private const string jsonDir = "./Json/{0}";

        private const string clientProtoDir = "../Unity/Assets/AssetsPackage/Config";
        private static string ClientProtoDir
        {
            get
            {
                if (IsCheck) return "./Temp/ClientProto";
                return clientProtoDir;
            }
        }
        private const string serverProtoDir = "../Config";
        private static string ServerProtoDir
        {
            get
            {
                if (IsCheck) return "./Temp/ServerProto";
                return serverProtoDir;
            }
        }
        private static bool IsCheck;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isCheck">是否只校验</param>
        public static void Export(bool isCheck = false)
        {
            IsCheck = isCheck;
            if(isCheck)
                Console.WriteLine("校验");
            else
                Console.WriteLine("导表");
            try
            {

                if (Directory.Exists(ServerClassDir))
                    Directory.Delete(ServerClassDir, true);
                if (Directory.Exists(ClientClassDir))
                    Directory.Delete(ClientClassDir, true);
            
                //if (Directory.Exists(serverProtoDir))
                //    Directory.Delete(serverProtoDir, true);
                if(Directory.Exists(ClientProtoDir))
                    Directory.Delete(ClientProtoDir, true);
                
                template = File.ReadAllText("Template.txt");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (string path in FindFile(excelDir))
                {
                    string fileName = Path.GetFileName(path);
                    if (!fileName.EndsWith(".xlsx") || fileName.StartsWith("~$"))
                    {
                        continue;
                    }
                    using Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using ExcelPackage p = new ExcelPackage(stream);
                    string name = Path.GetFileNameWithoutExtension(path);
                    if (name.StartsWith("C_") || name.StartsWith("A_"))
                    {
                        var real = name.Split("_")[1];
                        ExportExcelClass(p, real, ConfigType.Client);
                        ExportExcelJson(p, real, ConfigType.Client);
                    }
                    if (name.StartsWith("S_") || name.StartsWith("A_"))
                    {
                        var real = name.Split("_")[1];
                        ExportExcelClass(p, real, ConfigType.Server);
                        ExportExcelJson(p, real, ConfigType.Server);
                    }
                    if (name.StartsWith("P_"))
                    {
                        var real = name.Split("_")[1];

                        ExportExcelClass(p, real, ConfigType.Client);
                        ExportExcelJson(p, real, ConfigType.Client);
                    }
                }
                ExportExcelProtobuf(ConfigType.Client);
                ExportExcelProtobuf(ConfigType.Server);
                if (Directory.Exists(ClientClassDir))
                    foreach (string f in Directory.GetFileSystemEntries(ClientClassDir))
                        if (f.Contains("Chapter") && File.Exists(f))
                            File.Delete(f);
                Log.Console("Export Excel Sucess!");
            }
            catch (Exception e)
            {
                Log.Console(e.ToString());
            }
        }
        public static List<string> FindFile(string dirPath) //参数dirPath为指定的目录
        {
            List<string> res = new List<string>();
            //在指定目录及子目录下查找文件,在listBox1中列出子目录及文件
            DirectoryInfo Dir = new DirectoryInfo(dirPath);
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories()) //查找子目录
                {
                    res.AddRange(FindFile(dirPath + "\\"+d.Name));
                }
                foreach (var f in Directory.GetFiles(dirPath)) //查找文件
                {
                    res.Add(f); 
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return res;
        }
        private static string GetProtoDir(ConfigType configType)
        {
            if (configType == ConfigType.Client)
            {
                return ClientProtoDir;
            }
            return ServerProtoDir;
        }

        private static string GetClassDir(ConfigType configType)
        {
            if (configType == ConfigType.Client)
            {
                return ClientClassDir;
            }
            return ServerClassDir;
        }

        #region 导出class
        static void ExportExcelClass(ExcelPackage p, string name, ConfigType configType)
        {
            List<HeadInfo> classField = new List<HeadInfo>();
            HashSet<string> uniqeField = new HashSet<string>();
            foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
            {
                try
                {
                    if(worksheet.Dimension==null||worksheet.Dimension.End==null) continue;
                    Console.WriteLine("ExportSheetClass "+name);
                    ExportSheetClass(worksheet, classField, uniqeField, configType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(name+"--"+worksheet.Name + "     有错误 "+ ex);
                }
            }
            ExportClass(name, classField, configType);
        }

        static void ExportSheetClass(ExcelWorksheet worksheet, List<HeadInfo> classField, HashSet<string> uniqeField, ConfigType configType)
        {
            const int row = 2;
            for (int col = 3; col <= worksheet.Dimension.End.Column; ++col)
            {
                string fieldName = worksheet.Cells[row + 2, col].Text.Trim();
                if (fieldName == "")
                {
                    continue;
                }
                if (!uniqeField.Add(fieldName))
                {
                    continue;
                }
                string fieldCS = worksheet.Cells[row, col].Text.Trim();
                string fieldDesc = worksheet.Cells[row + 1, col].Text.Trim();
                string fieldType = worksheet.Cells[row + 3, col].Text.Trim();

                classField.Add(new HeadInfo(fieldCS, fieldDesc, fieldName, fieldType));
            }
        }

        static void ExportClass(string protoName, List<HeadInfo> classField, ConfigType configType)
        {
            string dir = GetClassDir(configType);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string exportPath = Path.Combine(dir, $"{protoName}.cs");

            using FileStream txt = new FileStream(exportPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < classField.Count; i++)
            {
                HeadInfo headInfo = classField[i];
                if (headInfo.FieldAttribute.StartsWith("#"))
                {
                    continue;
                }
                sb.Append($"\t\t[ProtoMember({i + 1})]\n");
                sb.Append($"\t\tpublic {headInfo.FieldType} {headInfo.FieldName} {{ get; set; }}\n");
            }
            string content = template.Replace("(ConfigName)", protoName).Replace(("(Fields)"), sb.ToString());
            sw.Write(content);
        }
        #endregion

        #region 导出json
        static void ExportExcelJson(ExcelPackage p, string name, ConfigType configType)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{\"list\":[");
            foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
            {
                if(worksheet.Dimension==null||worksheet.Dimension.End==null) continue;
                Console.WriteLine("ExportExcelJson "+name);
                ExportSheetJson(worksheet, name, configType, sb);
            }
            sb.AppendLine("]}");

            string dir = string.Format(jsonDir, configType.ToString());
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string jsonPath = Path.Combine(dir, $"{name}.txt");
            using FileStream txt = new FileStream(jsonPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);
            sw.Write(sb.ToString());
        }

        static void ExportSheetJson(ExcelWorksheet worksheet, string name, ConfigType configType, StringBuilder sb)
        {
            int infoRow = 2;
            HeadInfo[] headInfos = new HeadInfo[100];
            for (int col = 3; col <= worksheet.Dimension.End.Column; ++col)
            {
                string fieldCS = worksheet.Cells[infoRow, col].Text.Trim();
                if (fieldCS.Contains("#"))
                {
                    continue;
                }

                string fieldName = worksheet.Cells[infoRow + 2, col].Text.Trim();
                if (fieldName == "")
                {
                    continue;
                }

                string fieldDesc = worksheet.Cells[infoRow + 1, col].Text.Trim();
                string fieldType = worksheet.Cells[infoRow + 3, col].Text.Trim();

                headInfos[col] = new HeadInfo(fieldCS, fieldDesc, fieldName, fieldType);
            }

            for (int row = 6; row <= worksheet.Dimension.End.Row; ++row)
            {
                if (worksheet.Cells[row, 3].Text.Trim() == "")
                {
                    continue;
                }
                sb.Append("{");
                sb.Append($"\"_t\":\"{name}\",");
                for (int col = 3; col <= worksheet.Dimension.End.Column; ++col)
                {
                    HeadInfo headInfo = headInfos[col];
                    if (headInfo.FieldAttribute == null)
                    {
                        continue;
                    }
                    if (headInfo.FieldAttribute.Contains("#"))
                    {
                        continue;
                    }

                    if (headInfo.FieldName == "Id")
                    {
                        headInfo.FieldName = "_id";
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    sb.Append($"\"{headInfo.FieldName}\":{Convert(headInfo.FieldType, worksheet.Cells[row, col].Text.Trim())}");
                }
                sb.Append("},\n");
            }
        }
        
        private static string Convert(string type, string value)
        {
            switch (type)
            {
                case "int[]":
                case "int32[]":
                case "long[]":
                case "float[]":
                    return $"[{value.Replace(";",",")}]";
                case "string[]":
                    return $"[{value}]";
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "float":
                case "double":
                    if (value == "")
                    {
                        return "0";
                    }
                    return value;
                case "string":
                    return $"\"{value}\"";
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }
        #endregion

        // 根据生成的类，动态编译把json转成protobuf
        // 根据生成的类，动态编译把json转成protobuf
        private static void ExportExcelProtobuf(ConfigType configType)
        {
            string classPath = GetClassDir(configType);
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            List<string> protoNames = new List<string>();
            foreach (string classFile in Directory.GetFiles(classPath, "*.cs"))
            {
                protoNames.Add(Path.GetFileNameWithoutExtension(classFile));
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(classFile)));
            }

            List<PortableExecutableReference> references = new List<PortableExecutableReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    if (assembly.IsDynamic)
                    {
                        continue;
                    }

                    if (assembly.Location == "")
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                PortableExecutableReference reference = MetadataReference.CreateFromFile(assembly.Location);
                references.Add(reference);
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                null,
                syntaxTrees.ToArray(),
                references.ToArray(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using MemoryStream memSteam = new MemoryStream();

            EmitResult emitResult = compilation.Emit(memSteam);
            if (!emitResult.Success)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Diagnostic t in emitResult.Diagnostics)
                {
                    stringBuilder.AppendLine(t.GetMessage());
                }
                throw new Exception($"动态编译失败:\n{stringBuilder}");
            }

            memSteam.Seek(0, SeekOrigin.Begin);

            Assembly ass = Assembly.Load(memSteam.ToArray());

            string dir = GetProtoDir(configType);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            foreach (string protoName in protoNames)
            {
                Type type = ass.GetType($"ET.{protoName}Category");
                Type subType = ass.GetType($"ET.{protoName}");
                Serializer.NonGeneric.PrepareSerializer(type);
                Serializer.NonGeneric.PrepareSerializer(subType);


                string json = File.ReadAllText(Path.Combine(string.Format(jsonDir, configType), $"{protoName}.txt"));
                object deserialize = JsonHelper.FromJson(type,json);

                string path = Path.Combine(dir, $"{protoName}Category.bytes");

                using FileStream file = File.Create(path);
                Serializer.Serialize(file, deserialize);
            }
        }
    }
}