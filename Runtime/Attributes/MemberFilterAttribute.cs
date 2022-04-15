using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CippSharp.Members
{
    /// <summary>
    /// Filters the list of members displayed in the inspector drawer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class MemberFilterAttribute : PropertyAttribute
    {
        /// <summary>
        /// Default Attribute
        /// </summary>
        public static readonly MemberFilterAttribute Default = new MemberFilterAttribute();
        
        /// <summary>
        /// Allow to search for field
        /// </summary>
        public bool Fields { get; private set; } = true;
        /// <summary>
        /// Allow to search for properties
        /// </summary>
        public bool Properties { get; private set; } = true;
        /// <summary>
        /// Allow to search for methods
        /// </summary>
        public bool Methods { get; private set; } = true;
        
        /// <summary>
        /// Reflection Flags
        /// </summary>
        public BindingFlags Flags { get; private set; } = BindingFlags.Public | BindingFlags.Instance;

        #region Options
        
        //See also documentation file for this section

        public const string ReturnTypeOption = "r:";
        public const string NameContainsOption = "nc:";
        public const string NameEqualOption = "ne:";
        public const string MethodParametersCountOption = "mpc:";
        public const string MethodParameterNameContainsOption = "mpnc:";
        public const string MethodParameterNameEqualOption = "mpne:";
        public const string MethodParameterTypeOption = "mpt:";
        public const string CommaSeparator = ",";
        public const string SpaceSeparator = " ";
        
        /// <summary>
        /// Filter Attribute Special Options
        /// </summary>
        public string[] Options { get; private set; } = null;
        
        #endregion

        private MemberFilterAttribute()
        {
            
        }

        public MemberFilterAttribute(BindingFlags flags) : this ()
        {
            this.Flags = flags;
        }

        public MemberFilterAttribute(bool fields, bool properties, bool methods, params string[] options) 
            : this()
        {
            this.Fields = fields;
            this.Properties = properties;
            this.Methods = methods;
            this.Options = options;
        }
        
        public MemberFilterAttribute(bool fields, bool properties, bool methods, BindingFlags flags, params string[] options) 
            : this(fields, properties, methods, options)
        {
            this.Flags = flags;
        }

        public MemberFilterAttribute(params string[] options) : this()
        {
            this.Options = options;
        }

        #region Is Valid Member

        public bool IsValidMember(MemberInfo member)
        {
            if (ReflectionUtils.IsFieldInfo(member, out FieldInfo f))
            {
                if (Fields)
                {
                    return FilterMemberByOptions(f.FieldType, f.Name, null);
                }
            }
            else if (ReflectionUtils.IsPropertyInfo(member, out PropertyInfo p))
            {
                if (Properties)
                {
                    return FilterMemberByOptions(p.PropertyType, p.Name, null);
                }
            }
            else if (ReflectionUtils.IsMethodInfo(member, out MethodInfo m))
            {
                if (Methods)
                {
                    return FilterMemberByOptions(m.ReturnType, m.Name, m);
                }
            }

            return false;
        }

        private bool FilterMemberByOptions(Type memberType, string memberName, MethodInfo method = null)
        {
            if (ArrayUtils.IsNullOrEmpty(Options))
            {
                return true;
            }

            //Cache Method stuffs
            bool isMethod = method != null;
            ParameterInfo[] parameters = isMethod ? method.GetParameters() : new ParameterInfo[0];
            
            //Cache Field Type Name and 'secondary' Name
            string fieldTypeName = memberType.Name;
            bool isSpecialType = IsSpecialType(memberType, out string otherFieldTypeName);
            
            //if any of the options is not verified... the loop breaks and return false.
            bool isValidForAllOptions = true;
            
            int length = Options.Length;
            for (int i = 0; i < length; i++)
            {
                string option = Options[i].Replace(SpaceSeparator, string.Empty);
                
                string reducedOption;
                string[] split = null;
                
                //Return Type Option
                if (option.StartsWith(ReturnTypeOption))
                {
                    #region Return Type Option
                    
                    reducedOption = option.Replace(ReturnTypeOption, string.Empty);
                    split = StringUtils.CheckedSplit(reducedOption, new[] {CommaSeparator}, StringSplitOptions.RemoveEmptyEntries);
                    
                    isValidForAllOptions = string.IsNullOrEmpty(reducedOption) || (StringUtils.EqualAnyString(fieldTypeName, split) || StringUtils.EqualAnyString(otherFieldTypeName, split));
                    if (!isValidForAllOptions)
                    {
                        break;
                    }
                    
                    #endregion
                }
                //Member Name Contains Option
                else if (option.StartsWith(NameContainsOption))
                {
                    reducedOption = option.Replace(NameContainsOption, string.Empty);
                    split = StringUtils.CheckedSplit(reducedOption, new[] {CommaSeparator}, StringSplitOptions.RemoveEmptyEntries);
                    
                    isValidForAllOptions = string.IsNullOrEmpty(reducedOption) || StringUtils.ContainsAnyString(memberName, split);
                    if (!isValidForAllOptions)
                    {
                        break;
                    }
                }
                //Member Name Equal Option
                else if (option.StartsWith(NameEqualOption))
                {
                    reducedOption = option.Replace(NameEqualOption, string.Empty);
                    split = StringUtils.CheckedSplit(reducedOption, new[] {CommaSeparator}, StringSplitOptions.RemoveEmptyEntries);

                    isValidForAllOptions = string.IsNullOrEmpty(reducedOption) || StringUtils.EqualAnyString(memberName, split);
                    if (!isValidForAllOptions)
                    {
                        break;
                    }
                }

                //If it is not 'method' continue to search for other options
                if (!isMethod)
                {
                    continue;
                }

                //Method Parameters Count
                if (option.StartsWith(MethodParametersCountOption))
                {
                    reducedOption = option.Replace(MethodParametersCountOption, string.Empty);
                    split = StringUtils.CheckedSplit(reducedOption, new[] {CommaSeparator}, StringSplitOptions.RemoveEmptyEntries);
                    string last = split.LastOrDefault();
                    
                    isValidForAllOptions = (string.IsNullOrEmpty(reducedOption) || string.IsNullOrEmpty(last)) || (int.TryParse(last, out int targetParametersLength) && (targetParametersLength == -1 || targetParametersLength == parameters.Length));
                    if (!isValidForAllOptions)
                    {
                        break;
                    }
                }
                //Method Parameter Name Contains
                else if (option.StartsWith(MethodParameterNameContainsOption))
                {
                    reducedOption = option.Replace(MethodParameterNameContainsOption, string.Empty);
                    split = StringUtils.CheckedSplit(reducedOption, new[] {CommaSeparator}, StringSplitOptions.RemoveEmptyEntries);

                    isValidForAllOptions = string.IsNullOrEmpty(reducedOption) || (!ArrayUtils.IsNullOrEmpty(parameters) && parameters.Select(p => p.Name).Any(n => StringUtils.ContainsAnyString(n, split)));
                    if (!isValidForAllOptions)
                    {
                        break;
                    }
                }
                //Method Parameter Name Equal
                else if (option.StartsWith(MethodParameterNameEqualOption))
                {
                    reducedOption = option.Replace(MethodParameterNameEqualOption, string.Empty);
                    split = StringUtils.CheckedSplit(reducedOption, new[] {CommaSeparator}, StringSplitOptions.RemoveEmptyEntries);

                    isValidForAllOptions = string.IsNullOrEmpty(reducedOption) || (!ArrayUtils.IsNullOrEmpty(parameters) && parameters.Select(p => p.Name).Any(n => StringUtils.EqualAnyString(n, split)));
                    if (!isValidForAllOptions)
                    {
                        break;
                    }
                }
                //Method Parameter Type
                else if (option.StartsWith(MethodParameterTypeOption))
                {
                    reducedOption = option.Replace(MethodParameterTypeOption, string.Empty);
                    split = StringUtils.CheckedSplit(reducedOption, new[] {CommaSeparator}, StringSplitOptions.RemoveEmptyEntries);

                    isValidForAllOptions = string.IsNullOrEmpty(reducedOption) || (!ArrayUtils.IsNullOrEmpty(parameters) && parameters.SelectMany(ParametersTypeNamesSelector).Any(n => StringUtils.EqualAnyString(n, split)));
                    if (!isValidForAllOptions)
                    {
                        break;
                    }
                }
            }
            
            return isValidForAllOptions;
        }
        
        ICollection<string> ParametersTypeNamesSelector(ParameterInfo p)
        {
            Type parameterType = p.ParameterType;
            string parameterTypeName = parameterType.Name;;
            IsSpecialType(parameterType, out string otherParameterTypeName);
            List<string> names = new List<string> { parameterTypeName };
            if (!string.IsNullOrEmpty(otherParameterTypeName))
            {
                names.Add(otherParameterTypeName);
            }
            return names;
        }

        private bool IsSpecialType(Type type, out string otherName)
        {
            if (type == typeof(bool))
            {
                otherName = MemberInfoWriterParser.PrettyBooleanName;
                return true;
            }
            else if (type == typeof(int))
            {
                otherName = MemberInfoWriterParser.PrettyIntegerName;
                return true;
            }
            else if (type == typeof(float))
            {
                otherName = MemberInfoWriterParser.PrettySingleName;
                return true;
            }
            else if (type == typeof(string) || type == typeof(string[]) || type == typeof(void))
            {
                otherName =StringUtils.FirstCharToLower(type.Name);
                return true;
            }
            else if (type == typeof(bool[]))
            {
                otherName = MemberInfoWriterParser.PrettyBooleanArrayName;
                return true;
            }
            else if (type == typeof(int[]))
            {
                otherName = MemberInfoWriterParser.PrettyIntegerArrayName;
                return true;
            }
            else if (type == typeof(float[]))
            {
                otherName = MemberInfoWriterParser.PrettySingleArrayName;
                return true;
            }
            
            otherName = string.Empty;
            return false;
        }
        
        #endregion
    }
}