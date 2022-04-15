//
// Author: Alessandro Salani (Cippo)
//

using System;
using System.Linq;
using System.Reflection;

namespace CippSharp.Members
{
    using Debug = UnityEngine.Debug;
    
    internal static class ReflectionUtils
    {
        /// <summary>
        /// Common binding flags for most public and non public methods.
        /// </summary>
        public const BindingFlags Common = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        public const string ErrorMessagePrefix = "Error ";
        
        /// <summary>
        /// Create an instance of a type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="flags"></param>
        /// <returns>success</returns>
        public static bool CreateInstance(Type type, out object instance, BindingFlags flags = Common)
        {
            var constructor = type.GetConstructors(flags).FirstOrDefault(c => c.GetParameters().Length == 0);
            if (constructor == null)
            {
                instance = null;
                return false;
            }

            instance = constructor.Invoke(null);
            return true;
        }

        #region Find Type
        
        /// <summary>
        /// Find type via string
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="foundType"></param>
        /// <returns></returns>
        public static bool FindType(string typeFullName, out Type foundType)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.FullName != typeFullName)
                    {
                        continue;
                    }
                    
                    foundType = type;
                    return true;
                }
            }

            foundType = null;
            return false;
        }
        
        #endregion
        
        #region Is
        
        /// <summary>
        /// Is field info
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsFieldInfo(MemberInfo member)
        {
            return member is FieldInfo;
        }

        /// <summary>
        /// Is field info
        /// </summary>
        /// <param name="member"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool IsFieldInfo(MemberInfo member, out FieldInfo field)
        {
            if (member is FieldInfo f)
            {
                field = f;
                return true;
            }
            else
            {
                field = null;
                return false;
            }
        }
        
        /// <summary>
        /// Is property info 
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsPropertyInfo(MemberInfo member)
        {
            return member is PropertyInfo;
        }

        /// <summary>
        /// Is property info 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsPropertyInfo(MemberInfo member, out PropertyInfo property)
        {
            if (member is PropertyInfo p)
            {
                property = p;
                return true;
            }
            else
            {
                property = null;
                return false;
            }
        }

        /// <summary>
        /// Is member info
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static bool IsMethodInfo(MemberInfo member)
        {
            return member is MethodInfo;
        }

        /// <summary>
        /// Is member info
        /// </summary>
        /// <param name="member"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsMethodInfo(MemberInfo member, out  MethodInfo method)
        {
            if (member is MethodInfo m)
            {
                method = m;
                return true;
            }
            else
            {
                method = null;
                return false;
            }
        }
        

        #endregion
        
        /// <summary>
        /// Returns true if the context object has the target field. It also throws out the interested field.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fieldName"></param>
        /// <param name="field"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool HasField(object context, string fieldName, out FieldInfo field, BindingFlags flags = Common)
        {
            try
            {
                field = context.GetType().GetField(fieldName, flags);
                return field != null;
            }
            catch (Exception e)
            {
                UnityEngine.Object obj = context as UnityEngine.Object;
                string logName = StringUtils.LogName(obj);
                Debug.LogError(logName+ErrorMessagePrefix+e.Message, obj);
                
                field = null;
                return false;
            }            
        }
        
        #region MemberInfo Methods

        /// <summary>
        /// Returns true if the context object has the target member.
        /// It also throws out the interested member.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="memberName"></param>
        /// <param name="member"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool HasMember(object context, string memberName, out MemberInfo member, BindingFlags flags = Common)
        {
            try
            {
                member = context.GetType().GetMember(memberName, flags).FirstOrDefault();
                return member != null;
            }
            catch (Exception e)
            {
                UnityEngine.Object obj = context as UnityEngine.Object;
                string logName = StringUtils.LogName(obj);
                Debug.LogError(logName+ErrorMessagePrefix+e.Message, obj);
                member = null;
                return false;
            }            
        }

        /// <summary>
        /// Returns the value of target member if it exists otherwise return T's default value.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="memberName"></param>
        /// <param name="result"></param>
        /// <param name="bindingFlags"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryGetMemberValue<T>(object context, string memberName, out T result, BindingFlags bindingFlags = Common)
        {
            try
            {
                MemberInfo member = context.GetType().GetMember(memberName, bindingFlags).FirstOrDefault();
                return TryGetMemberValue(context, member, out result);
            }
            catch (Exception e)
            {
                UnityEngine.Object obj = context as UnityEngine.Object;
                string logName = StringUtils.LogName(obj);
                Debug.LogError(logName+ErrorMessagePrefix+e.Message, obj);
            }
            
            result = default(T);
            return false;
        }
        
        /// <summary>
        ///  If you already have the member, it returns the value of target member
        /// if it exists otherwise return T's default value.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="member"></param>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryGetMemberValue<T>(object context, MemberInfo member, out T result)
        {
            try
            {
                if (member != null)
                {
                    if (IsFieldInfo(member, out FieldInfo f))
                    {
                        result = (T) f.GetValue(context);
                        return true;
                    }
                    else if (IsPropertyInfo(member, out PropertyInfo p))
                    {
                        result = (T) p.GetValue(context, null);
                        return true;
                    }
                    else if (IsMethodInfo(member, out MethodInfo m))
                    {
                        result = (T) m.Invoke(context, null);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Object obj = context as UnityEngine.Object;
                string logName = StringUtils.LogName(obj);
                Debug.LogError(logName+ErrorMessagePrefix+e.Message, obj);
            }
            
            result = default(T);
            return false;
        }

        #endregion
        
         #region MethodInfo Methods
        
        /// <summary>
        /// Find a method via string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="methodInfo"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool FindMethod(Type type, string methodName, out MethodInfo methodInfo, BindingFlags flags = Common)
        {
            methodInfo = type.GetMethod(methodName, flags);
            return methodInfo != null;
        }
        
        /// <summary>
        /// Returns true if the context object has the target method. It also throws out the interested method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="methodName"></param>
        /// <param name="method"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool HasMethod(object context, string methodName, out MethodInfo method, BindingFlags flags = Common)
        {
            try
            {
                method = context.GetType().GetMethod(methodName, flags);
                return method != null;
            }
            catch (Exception e)
            {
                UnityEngine.Object obj = context as UnityEngine.Object;
                Debug.LogError(e.Message, obj);
                
                method = null;
                return false;
            }
        }

        /// <summary>
        /// Check a condition on parameters of a method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool HasParametersCondition(MethodInfo method, Predicate<ParameterInfo[]> predicate)
        {
            try
            {
                ParameterInfo[] parameters = method.GetParameters();
                return predicate.Invoke(parameters);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }


        /// <summary>
        /// Call method if exists on target object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <param name="result"></param>
        /// <param name="bindingFlags"></param>
        public static bool TryCallMethod(object context, string methodName, out object result, object[] parameters = null, BindingFlags bindingFlags = Common)
        {
            try
            {
                MethodInfo methodInfo = context.GetType().GetMethod(methodName, bindingFlags);
                if (methodInfo != null)
                {
                    result = methodInfo.Invoke(context, parameters);
                    return true;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Object obj = context as UnityEngine.Object;
                Debug.LogError(e.Message, obj);
            }

            result = null;
            return false;
        }
        
        #endregion
    }
}
