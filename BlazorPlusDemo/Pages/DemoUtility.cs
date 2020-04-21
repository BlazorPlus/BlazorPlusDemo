using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.Encodings.Web;

internal class DemoUtility
{
    static public string GetTypeMemberHTML(MemberInfo member)
    {
        string str = "";

        var property = member as PropertyInfo;
        var method = member as MethodBase;
        var field = member as FieldInfo;
        var eventinfo = member as EventInfo;

        var isstatic = false;
        var ispublic = false;
        var isprotected = false;
        var isinternal = false;
        var isoverride = false;
        var isvirtual = false;
        var isabstract = false;
        Type memtype = null;
        string membername = member.Name;
        if (membername == ".ctor")
            membername = member.DeclaringType.Name;

        if (field != null)
        {
            isstatic = field.IsStatic;
            ispublic = field.IsPublic;
            isprotected = field.IsFamily || field.IsFamilyOrAssembly;
            isinternal = field.IsAssembly || field.IsFamilyOrAssembly;
            memtype = field.FieldType;
        }
        if (property != null)
        {
            if (property.CanRead)
            {
                method = property.GetGetMethod();
            }
            else if (property.CanWrite)
            {
                method = property.GetSetMethod();
            }
            memtype = property.PropertyType;
        }
        if (eventinfo != null)
        {
            method = eventinfo.GetAddMethod();
            memtype = eventinfo.EventHandlerType;
        }

        if (method != null)
        {
            isstatic = method.IsStatic;
            ispublic = method.IsPublic;
            isprotected = method.IsFamily || method.IsFamilyOrAssembly;
            isinternal = method.IsAssembly || method.IsFamilyOrAssembly;
            isabstract = method.IsAbstract;
            if (method.IsVirtual)
            {
                if (method.DeclaringType == method.ReflectedType)
                    isvirtual = true;
                else
                    isoverride = true;
            }
            var mi = method as MethodInfo;
            if (mi != null && property == null && eventinfo == null)
            {
                memtype = mi.ReturnType;
            }
        }


        if (isstatic) str += " static";
        if (ispublic) str += " public";
        if (isprotected) str += " protected";
        if (isinternal) str += " internal";
        if (isabstract) str += " abstract";
        if (isvirtual) str += " virtual";
        if (isoverride) str += " override";
        str += " " + GetTypeNameHtml(memtype);
        str += " <span style='color:darkred;font-weight:bold'>" + membername + "</span>";
        if (property != null)
        {
            str += " {";
            if (property.CanRead) str += " <span style='color:navy'>get</span>;";
            if (property.CanWrite) str += " <span style='color:navy'>set</span>;";
            str += " }";
        }
        else if (method != null)
        {
            if (method.IsGenericMethod)
            {
                str += "&lt;";
                str += string.Join(",", method.GetGenericArguments().Select(v => GetTypeNameHtml(v)).ToArray());
                str += "&gt;";
            }

            str += "(";
            var pis = method.GetParameters();
            for (int i = 0; i < pis.Length; i++)
            {
                if (i != 0)
                    str += ",";
                var pi = pis[i];
                if (pi.IsOut)
                    str += "out ";
                Type ptype = pi.ParameterType;
                if (ptype.IsByRef)
                {
                    str += "byref ";
                    ptype = ptype.GetElementType();
                }
                str += GetTypeNameHtml(ptype);
                str += " <span style='color:#666;font-style:italic'>" + pi.Name + "</span>";
            }
            str += ")";
        }

        return str.Trim();
    }

    static public string GetTypeNameHtml(Type type)
    {
        if (type == null)
            return "";
        return "<span style='color:blue'>" + HtmlEncoder.Default.Encode(GetTypeNameText(type)) + "</span>";
    }

    static public string GetTypeNameText(Type type)
    {
        string typename = type.FullName;
        switch (typename)
        {
            case "System.Void": return "void";
            case "System.Int32": return "int";
            case "System.Int64": return "long";
            case "System.String": return "string";
            case "System.Boolean": return "bool";
            case "System.Threading.Tasks.Task": return "task";
        }

        if (type.IsGenericType)
        {
            Type gtd = type.GetGenericTypeDefinition();
            string gtdname = gtd.Name.Split('`')[0];
            var tps = type.GetGenericArguments();

            if (gtdname == "Nullable" && tps.Length == 1)
                return GetTypeNameText(tps[0]) + "?";

            return gtdname + "<" + string.Join(",", tps.Select(v => GetTypeNameText(v)).ToArray()) + ">";
        }
        if (type.IsGenericTypeParameter)
            return type.Name;// <T>

        if (type.IsGenericTypeDefinition)
            return "#2:" + type.Name;

        if (type.IsConstructedGenericType)
            return "#4:" + type.Name;

        return type.Name;
    }
}
