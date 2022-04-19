using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CippSharp.Members
{
    public static class MemberInfoWriterParser
    {
        private static readonly string LogName = StringUtils.LogName(typeof(MemberInfoWriterParser));
        
        private const string FieldsPrefix = "Fields";
        private const string PropertiesPrefix = "Properties";
        private const string MethodsPrefix = "Methods";
      
        private const string Separator = MembersConstants.Separator;
        private const string CommaSeparator = MembersConstants.CommaSeparator;
        private const string Space = MembersConstants.Space;
        
        public const string PrettyBooleanName = "bool";
        public const string PrettyBooleanArrayName = "bool[]";
        public const string PrettyIntegerName = "int";
        public const string PrettyIntegerArrayName = "int[]";
        public const string PrettySingleName = "float";
        public const string PrettySingleArrayName = "float";
        
        /// <summary>
        /// Filters and write member infos
        /// </summary>
        /// <param name="filterAttribute"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static List<GUIContent> FilterAndWriteMembersInfos(MemberFilterAttribute filterAttribute, MemberInfo[] members)
        {
            List<GUIContent> results = new List<GUIContent>();
            if (ArrayUtils.IsNullOrEmpty(members))
            {
                return results;
            }
            if (filterAttribute == null)
            {
                filterAttribute = MemberFilterAttribute.Default;
            }

            foreach (var member in members)
            {
                if (!filterAttribute.IsValidMember(member))
                {
                    continue;
                }
                
                if (TryWriteMemberInfo(member, out string result))
                {
                    results.Add(new GUIContent(result));
                }
            }

            return results;
        }

        /// <summary>
        /// Write down a member info
        /// </summary>
        /// <param name="member"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryWriteMemberInfo(MemberInfo member, out string result)
        {
            if (ReflectionUtils.IsFieldInfo(member, out FieldInfo f))
            {
                result = $"{FieldsPrefix}{Separator}{WriteTypeName(f.FieldType)}{Space}{f.Name}";
                return true;
            }
            else if (ReflectionUtils.IsPropertyInfo(member, out PropertyInfo p))
            {
                result = $"{PropertiesPrefix}{Separator}{WriteTypeName(p.PropertyType)}{Space}{p.Name}";
                return true;
            }
            else if (ReflectionUtils.IsMethodInfo(member, out MethodInfo method))
            {
                ParameterInfo[] parameters = method.GetParameters();
                string parametersAsString = "(";
                if (!ArrayUtils.IsNullOrEmpty(parameters))
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        ParameterInfo parameter = parameters[i];
                        string parameterAsString = $"{WriteTypeName(parameter.ParameterType)}{Space}{parameter.Name}, ";
                        if (i == parameters.Length - 1)
                        {
                            parameterAsString = parameterAsString.TrimEnd(new []{' ', ',',});
                        }
                        parametersAsString += parameterAsString;
                    }
                }
                parametersAsString += ")";
                result = $"{MethodsPrefix}/{WriteTypeName(method.ReturnType)}{Space}{method.Name}{Space}{parametersAsString}";
                return true;
            }
            else
            {
                result = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// Write down a type name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string WriteTypeName(Type type)
        {
            if (type == typeof(bool))
            {
                return PrettyBooleanName;
            }
            else if (type == typeof(int))
            {
                return PrettyIntegerName;
            }
            else if (type == typeof(float))
            {
                return PrettySingleName;
            }
            else if (type == typeof(string) || type == typeof(string[]) || type == typeof(void))
            {
                return StringUtils.FirstCharToLower(type.Name);
            }
            else if (type == typeof(bool[]))
            {
                return PrettyBooleanArrayName;
            }
            else if (type == typeof(int[]))
            {
                return PrettyIntegerArrayName;
            }
            else if (type == typeof(float[]))
            {
                return PrettySingleArrayName;
            }

            return type.Name;
        }

        /// <summary>
        /// Try to parse out the member info of <see cref="IMemberReferenceBase.Target"/>
        /// </summary>
        /// <param name="memberReference"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static bool TryParseMemberInfo(IMemberReferenceBase memberReference, out MemberInfo memberInfo)
        {
            memberInfo = null;
            if (memberReference == null)
            {
                Debug.LogError(LogName+$"{nameof(TryParseMemberInfo)} {nameof(memberReference)} is null!");
                return false;
            }
            
            object target = memberReference.Target;
            if (target == null)
            {
                Debug.LogError(LogName+$"{nameof(TryParseMemberInfo)} {nameof(memberReference)}.{nameof(target)} is null!");
                return false;
            }
            
            Type targetType = target.GetType();
            BindingFlags flags = memberReference.Flags;
            string writtenMember = memberReference.WrittenMember;
            string reducedWrittenMember;
            if (writtenMember.StartsWith(FieldsPrefix))
            {
                #region Get FieldInfo as MemberInfo
                    
                reducedWrittenMember = writtenMember.Replace(FieldsPrefix, string.Empty).Replace(Separator, string.Empty);
                string[] split = StringUtils.CheckedSplit(reducedWrittenMember, new[] {Space}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    FieldInfo fieldInfo = targetType.GetMembers(flags).Where(FieldInfoPredicate).Cast<FieldInfo>().FirstOrDefault();
                    bool FieldInfoPredicate(MemberInfo m)
                    {
                        return ReflectionUtils.IsFieldInfo(m, out FieldInfo f) && (split[0] == WriteTypeName(f.FieldType) && split[1] == f.Name);
                    }

                    memberInfo = fieldInfo;
                    return memberInfo != null;
                }
                
                #endregion
            }
            else if (writtenMember.StartsWith(PropertiesPrefix))
            {
                #region Get PropertyInfo as MemberInfo
                    
                reducedWrittenMember = writtenMember.Replace(PropertiesPrefix, string.Empty).Replace(Separator, string.Empty);
                string[] split = StringUtils.CheckedSplit(reducedWrittenMember, new[] {Space}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    PropertyInfo propertyInfo = targetType.GetMembers(flags).Where(PropertyInfoPredicate).Cast<PropertyInfo>().FirstOrDefault();
                    bool PropertyInfoPredicate(MemberInfo m)
                    {
                        return ReflectionUtils.IsPropertyInfo(m, out PropertyInfo p) && (split[0] == WriteTypeName(p.PropertyType) && split[1] == p.Name);
                    }

                    memberInfo = propertyInfo;
                    return memberInfo != null;
                }
                
                #endregion
            }
            else if (writtenMember.StartsWith(MethodsPrefix))
            {
                #region Get MethodInfo as MemberInfo
                
                reducedWrittenMember = writtenMember.Replace(MethodsPrefix, string.Empty).Replace(Separator, string.Empty);
                //Split before and after parameters
                string[] split = StringUtils.CheckedSplit(reducedWrittenMember, new[] {"("}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                {
                    Debug.LogWarning(LogName+$"{nameof(TryParseMemberInfo)} MethodInfo region, methods split length by ( is not 2! Flat Split Result = {StringUtils.ToFlatArray(split, " | ")}.");
                    return false;
                }

                string[] writtenMethod = StringUtils.CheckedSplit(split[0], new[] {Space}, StringSplitOptions.RemoveEmptyEntries);
                if (writtenMethod.Length != 2)
                {
                    return false;
                }
                
                string[] writtenParameters = StringUtils.CheckedSplit(split[1].TrimEnd(new []{')'}), new [] {CommaSeparator}, StringSplitOptions.RemoveEmptyEntries);
                MethodInfo methodInfo = targetType.GetMembers(flags).Where(MethodPredicatePredicate).Cast<MethodInfo>().FirstOrDefault();
                bool MethodPredicatePredicate(MemberInfo m)
                {
                    bool b = ReflectionUtils.IsMethodInfo(m, out MethodInfo tmpMethod) && (writtenMethod[0] == WriteTypeName(tmpMethod.ReturnType) && writtenMethod[1] == tmpMethod.Name);
                    ParameterInfo[] methodParameters = tmpMethod != null ? tmpMethod.GetParameters() : new ParameterInfo[0];
                    int writtenParametersLength = writtenParameters.Length;
                    if (b && writtenParametersLength == 0 && methodParameters.Length == writtenParametersLength)
                    {
                        return true;
                    }
                    else if (b && methodParameters.Length == writtenParametersLength)
                    {
                        bool allParametersAreCorrect = true;
                        for (int i = 0; i < writtenParametersLength; i++)
                        {
                            string[] writtenParameterSplit = StringUtils.CheckedSplit(writtenParameters[i], new[] {Space}, StringSplitOptions.RemoveEmptyEntries);
                            ParameterInfo methodParameter = methodParameters[i];
                            if (writtenParameterSplit.Length == 2)
                            {
                                allParametersAreCorrect = writtenParameterSplit[0] == WriteTypeName(methodParameter.ParameterType) && writtenParameterSplit[1] == methodParameter.Name;
                                if (!allParametersAreCorrect)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"Fail on {tmpMethod.Name} ({methodParameter.Name}) "+writtenParameterSplit.Length.ToString());
                                continue;
                            }
                        }
                        b = allParametersAreCorrect;
                    }
                    else
                    {
                        b = false;
                    }
                    return b;
                }


                memberInfo = methodInfo;
                return memberInfo != null;

                #endregion
            }

            return false;
        }
    }
}
