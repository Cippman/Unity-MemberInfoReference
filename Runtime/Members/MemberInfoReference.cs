using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CippSharp.Members
{
    /// <summary>
    /// Consider this like an abstract class for MemberInfoReferences
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MemberInfoReference<T> : AMemberInfoReferenceBase 
        where T : UnityEngine.Object
    {
        private static readonly string LogName = StringUtils.LogName(typeof(MemberInfoReference<T>));
        
        public override Type Type => typeof(T);
    
        /// <summary>
        /// Target UnityObject from where members are taken
        /// </summary>
        [SerializeField] protected T target = null;
        public override object Target => target;

        /// <summary>
        /// Written Member of target to get or invoke.
        /// </summary>
        [SerializeField] protected string member = string.Empty;
        public override string WrittenMember => member;

        /// <summary>
        /// Binding flags stored as int
        /// </summary>
        [SerializeField, HideInInspector] private int flags = (int)(BindingFlags.Public | BindingFlags.Instance);
        public override BindingFlags Flags => (BindingFlags)flags;

        /// <summary>
        /// This is cached during <see cref="ISerializationCallbackReceiver"/>
        /// </summary>
        private MemberInfo cachedMember = null;
        
        protected MemberInfoReference()
        {
            
        }
       
        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            cachedMember = MemberInfoWriterParser.TryParseMemberInfo(this, out MemberInfo memberInfo) ? memberInfo : null;
        }
        
        public bool CanGetSetOrInvoke()
        {
            return target != null && cachedMember != null;
        }

        public override object GetOrInvoke(params object[] parameters)
        {
            if (!CanGetSetOrInvoke())
            {
                return null;
            }
            
            return GetSetOrInvokeInternal(Target, cachedMember, false, out object result, parameters) ? result : null;
        }

        public override object SetOrInvoke(params object[] parameters)
        {
            if (!CanGetSetOrInvoke())
            {
                return null;
            }
            
            return GetSetOrInvokeInternal(Target, cachedMember, true, out object result, parameters) ? result : null;
        }

        #region Get Set or Invoke Internal

        /// <summary>
        /// Handles the get and the set or the invoke of the member info.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="member"></param>
        /// <param name="isSet"></param>
        /// <param name="result"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static bool GetSetOrInvokeInternal(object target, MemberInfo member, bool isSet, out object result, params object[] parameters)
        {
            result = null;
            object value = null;
            if (!ArrayUtils.IsNullOrEmpty(parameters))
            {
                value = parameters[0];
            }
          
            if (ReflectionUtils.IsFieldInfo(member, out FieldInfo f))
            {
                #region Get/Set FieldInfo
                
                if (isSet)
                {
                    try
                    {
                        f.SetValue(target, value);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(LogName+$"Failed set field value. Cause: {e.Message}.");
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        result = f.GetValue(target);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(LogName+$"Failed get field value. Cause: {e.Message}.");  
                        return false;
                    }
                }
                
                #endregion
            }
            else if (ReflectionUtils.IsPropertyInfo(member, out PropertyInfo p))
            {
                #region Get/Set PropertyInfo
               
                if (isSet)
                {
                    try
                    {
                        p.SetValue(target, value);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(LogName+$"Failed set property value. Cause: {e.Message}.");
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        result = p.GetValue(target);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(LogName+$"Failed get property value. Cause: {e.Message}.");
                        return false;
                    }
                }
                
                #endregion
            }
            else if (ReflectionUtils.IsMethodInfo(member, out MethodInfo m))
            {
                #region MethodInfo Invoke
                
                try
                {
                    result = m.Invoke(target, parameters);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError(LogName+$"Failed to invoke method {m.Name}. Cause: {e.Message}.");
                    return false;
                }
                
                #endregion
            }

            return false;
        }
        
        #endregion
    }
}
