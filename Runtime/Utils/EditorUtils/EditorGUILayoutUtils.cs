#if UNITY_EDITOR
//
// Author: Alessandro Salani (Cippo)
//
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CippSharp.Members
{
	public static partial class EditorGUILayoutUtils
	{
		public const string inspectorModePropertyName = "inspectorMode";
		public const string instanceIdLabelValue = "Instance ID";
		public const string identfierLabelValue = "Local Identfier in File";
		public const string selfLabelValue = "Self";
//		public const string k_BackingField = SerializedPropertyUtils.k_BackingField;

		private const string PropertyIsNotArrayError = "Property isn't an array.";
		private const string PropertyIsNotValidArrayWarning = "Property isn't a valid array.";
		
		
//		/// <summary>
//		/// Foreach element (<see cref="SerializedProperty"/>) found in the <param name="serializedObject"></param> iterator,
//		/// this will invoke a callback where you can override the draw of each or of some properties.
//		/// </summary>
//		/// <param name="serializedObject"></param>
//		/// <param name="drawPropertyDelegate"></param>
//		/// <returns></returns>
//		public static bool DrawInspector(SerializedObject serializedObject, DrawSerializedPropertyDelegate drawPropertyDelegate)
//		{
//			EditorGUI.BeginChangeCheck();
//			serializedObject.UpdateIfRequiredOrScript();
//			SerializedProperty iterator = serializedObject.GetIterator();
//			for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
//			{
//				using (new EditorGUI.DisabledScope(Constants.ScriptSerializedPropertyName == iterator.propertyPath))
//				{
//					drawPropertyDelegate.Invoke(iterator.Copy());
//				}
//			}
//			serializedObject.ApplyModifiedProperties();
//			return EditorGUI.EndChangeCheck();
//		}
//		
		/// <summary>
		/// It draws the property only if its different from null.
		/// </summary>
		/// <param name="property"></param>
		public static void DrawProperty(SerializedProperty property)
		{
			if (property != null)
			{
				EditorGUILayout.PropertyField(property, property.isExpanded && property.hasChildren);
			}
		}
		
		/// <summary>
		/// It draws the property only if its different from null in a not-editable way.
		/// </summary>
		/// <param name="property"></param>
		public static void DrawNotEditableProperty(SerializedProperty property)
		{	
			bool guiEnabled;
			guiEnabled = GUI.enabled;
			GUI.enabled = false;
			if (property != null)
			{
				EditorGUILayout.PropertyField(property, property.isExpanded && property.hasChildren);
			}
			GUI.enabled = guiEnabled;
		}

	}
}
#endif
