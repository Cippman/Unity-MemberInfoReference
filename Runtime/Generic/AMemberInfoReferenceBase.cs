using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CippSharp.Members
{
    [Serializable]
    public abstract class AMemberInfoReferenceBase : ISerializationCallbackReceiver, IMemberReferenceBase
    {
        /// <summary>
        /// Member Reference Type
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Current Target
        /// </summary>
        public abstract object Target { get; }
        
        /// <summary>
        /// Selected Member
        /// </summary>
        public abstract string WrittenMember { get; }
        
        /// <summary>
        /// Binding Flags
        /// </summary>
        public abstract BindingFlags Flags { get; }

        #region ISerializationCallbackReceiver Implementation
        
        public virtual void OnBeforeSerialize()
        {
            
        }

        public virtual void OnAfterDeserialize()
        {
            
        }
        
        #endregion

        /// <summary>
        /// Retrieve value or invokes member
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract object GetOrInvoke(params object[] parameters);

        /// <summary>
        /// Set value or invokes members
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract object SetOrInvoke(params object[] parameters);

        #region Custom Editor
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(AMemberInfoReferenceBase), true)]
        public class AMemberInfoReferenceDrawer : PropertyDrawer
        {
            protected const string targetPropertyName = "target";
            protected SerializedProperty targetProperty = null;
            protected const string memberPropertyName = "member";
            protected SerializedProperty memberProperty = null;

            protected const string hiddenFlagsPropertyName = "flags";
            protected SerializedProperty hiddenFlagsProperty = null;
            
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (property.isExpanded)
                {
                    return EditorGUIUtils.GetPropertyHeight(property, label) -  EditorGUIUtils.LineHeight;
                }
                else
                {
                    return EditorGUIUtils.GetPropertyHeight(property, label);
                }
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var c = GUI.color;
                
                EditorGUIUtils.DrawFoldout(ref position, property);
                if (property.isExpanded)
                {
                    targetProperty = property.FindPropertyRelative(targetPropertyName);
                    memberProperty = property.FindPropertyRelative(memberPropertyName);
                    hiddenFlagsProperty = property.FindPropertyRelative(hiddenFlagsPropertyName);
                    
                    List<GUIContent> displayOptions = GetGUIContentDisplayOptions();
                    
                    EditorGUI.indentLevel++;
                    
                    Rect[] rects = EditorGUIUtils.DivideSpaceHorizontal(position, 2);
                    rects[0].height = EditorGUIUtils.SingleLineHeight;
                    //Rect[0] is target property.
                    Rect[] rectsTargetProperty = EditorGUIUtils.DivideSpaceHorizontalTwoThirds(rects[0], 2);
                    
                    EditorGUI.LabelField(rectsTargetProperty[0], new GUIContent(targetProperty.displayName));
                    EditorGUI.PropertyField(rectsTargetProperty[1], targetProperty, GUIContent.none);
                    
                    //Rect[1] is member property.
                    EditorGUIUtils.DrawOptionsPopUpForStringProperty(rects[1], GUIContent.none, memberProperty, displayOptions);
                    
                    EditorGUI.indentLevel--;
                }

                GUI.color = c;
            }


            protected virtual List<GUIContent> GetGUIContentDisplayOptions()
            {
                List<GUIContent> displayOptions = new List<GUIContent>();
                displayOptions.Add(new GUIContent("Nothing"));
                Object target = targetProperty?.objectReferenceValue;
                if (target == null)
                {
                    return displayOptions;
                }

                Type type = ((object) target).GetType();
                MemberFilterAttribute memberFilterAttribute = fieldInfo.GetCustomAttributes<MemberFilterAttribute>()?.FirstOrDefault() ?? MemberFilterAttribute.Default;
                if (hiddenFlagsProperty != null)
                {
                    hiddenFlagsProperty.intValue = (int) memberFilterAttribute.Flags;
                }
                MemberInfo[] members = type.GetMembers(memberFilterAttribute.Flags);
                displayOptions.AddRange(MemberInfoWriterParser.FilterAndWriteMembersInfos(memberFilterAttribute, members));

                return displayOptions;
            }

        }
#endif
        #endregion
    }
}
