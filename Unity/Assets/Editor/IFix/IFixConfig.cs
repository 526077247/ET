using IFix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[Configure]
public class IFixConfig
{
    static string[] Assemblys = new string[]
    {
        "Assembly-CSharp",
        "Unity.Mono",
    };
    [IFix]
    static IEnumerable<Type> ToProcess
    {
        get
        {
            var types = new List<Type>();
            for (int i = 0; i < Assemblys.Length; i++)
            {
                types.AddRange((from type in Assembly.Load(Assemblys[i]).GetTypes()
                                where type.Namespace == "ET"
                                select type));
            }
            return types;
        }
    }
}
