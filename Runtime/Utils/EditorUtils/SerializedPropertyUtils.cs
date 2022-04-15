#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CippSharp.Members
{
    internal static class SerializedPropertyUtils
    {
        public const string k_BackingField = "k__BackingField";
        
        private const string PropertyIsNullError = "Property is null.";
        private const string PropertyIsNotArrayError = "Property isn't an array.";
        
        /// <summary>
        /// Retrieve a serialized property that is an array as an array of serialized properties.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static SerializedProperty[] ToArray(SerializedProperty property)
        {
            if (property == null)
            {
                Debug.LogError(PropertyIsNullError);
                return null;
            }
			
            if (!property.isArray)
            {
                Debug.LogError(PropertyIsNotArrayError);
                return null;
            }

            List<SerializedProperty> elements = new List<SerializedProperty>();
            for (int i = 0; i < property.arraySize; i++)
            {
                elements.Add(property.GetArrayElementAtIndex(i));
            }

            return elements.ToArray();
        }

         #region Generic Get / Set Value of Property
        
	    /// <summary>
	    /// Some years ago the gradient value wasn't exposed!
	    /// </summary>
        private static BindingFlags gradientValueFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
	    /// <summary>
	    /// Some years ago the gradient value wasn't exposed!
	    /// </summary>
        private static PropertyInfo gradientValuePropertyInfo = typeof(SerializedProperty).GetProperty("gradientValue", gradientValueFlags, null, typeof(Gradient), new Type[0], null);
       
	    #region Get Value

	    #region Get List
	    
	    /// <summary>
	    /// Retrieve an array of 'T' from serialized property array.
	    /// </summary>
	    /// <param name="property"></param>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    public static List<T> GetList<T>(SerializedProperty property)
	    {
		    if (property == null)
		    {
			    Debug.LogError(PropertyIsNullError);
			    return null;
		    }
			
		    if (!property.isArray)
		    {
			    Debug.LogError(PropertyIsNotArrayError);
			    return null;
		    }
			
		    List<T> objects = new List<T>();
		    for (int i = 0; i < property.arraySize; i++)
		    {
			    SerializedProperty element = property.GetArrayElementAtIndex(i);
			    try
			    {
				    objects.Add(GetValue<T>(element));
			    }
			    catch (Exception e)
			    {
				    Debug.LogError(e.Message);
			    }
		    }
		    return objects;
	    }
        
	    #endregion
	    
        /// <summary>
		/// Retrieve value from serialized property relative if possible.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="propertyRelative"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetValue<T>(SerializedProperty property, string propertyRelative)
		{
			SerializedProperty foundProperty = property.FindPropertyRelative(propertyRelative);
			return GetValue<T>(foundProperty);
		}

		/// <summary>
		/// Retrieve value from serialized property if possible.
		/// </summary>
		/// <param name="property"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public static T GetValue<T>(SerializedProperty property)
		{
			Type type = typeof(T);

			// First, do special Type checks
			if (type.IsEnum)
				return (T) (object) property.intValue;

			// Next, check for literal UnityEngine struct-types
			// @note: ->object->ValueT double-casts because C# is too dumb to realize that that the ValueT in each situation is the exact type needed.
			// 	e.g. `return thisSP.colorValue` spits _error CS0029: Cannot implicitly convert type `UnityEngine.Color' to `ValueT'_
			// 	and `return (ValueT)thisSP.colorValue;` spits _error CS0030: Cannot convert type `UnityEngine.Color' to `ValueT'_
			if (typeof(Color).IsAssignableFrom(type))
				return (T) (object) property.colorValue;
			else if (typeof(LayerMask).IsAssignableFrom(type))
				return (T) (object) property.intValue;
			else if (typeof(Vector2).IsAssignableFrom(type))
				return (T) (object) property.vector2Value;
			else if (typeof(Vector3).IsAssignableFrom(type))
				return (T) (object) property.vector3Value;
			else if (typeof(Vector4).IsAssignableFrom(type))
				return (T) (object) property.vector4Value;
			else if (typeof(Rect).IsAssignableFrom(type))
				return (T) (object) property.rectValue;
			else if (typeof(AnimationCurve).IsAssignableFrom(type))
				return (T) (object) property.animationCurveValue;
			else if (typeof(Bounds).IsAssignableFrom(type))
				return (T) (object) property.boundsValue;
			else if (typeof(Gradient).IsAssignableFrom(type))
				return (T) (object) SafeGetGradientValue(property);
			else if (typeof(Quaternion).IsAssignableFrom(type))
				return (T) (object) property.quaternionValue;

			// Next, check if derived from UnityEngine.Object base class
			if (typeof(UnityEngine.Object).IsAssignableFrom(type))
				return (T) (object) property.objectReferenceValue;

			// Finally, check for native type-families
			if (typeof(int).IsAssignableFrom(type))
				return (T) (object) property.intValue;
			else if (typeof(bool).IsAssignableFrom(type))
				return (T) (object) property.boolValue;
			else if (typeof(float).IsAssignableFrom(type))
				return (T) (object) property.floatValue;
			else if (typeof(string).IsAssignableFrom(type))
				return (T) (object) property.stringValue;
			else if (typeof(char).IsAssignableFrom(type))
				return (T) (object) property.intValue;
			
			// And if all fails, throw an exception.
			throw new NotImplementedException("Unimplemented propertyType " + property.propertyType + ".");
		}
        
	    /// <summary>
	    /// Access to SerializedProperty's internal gradientValue property getter,
	    /// in a manner that'll only soft break (returning null) if the property changes
	    /// or disappears in future Unity revs.
	    /// </summary>
	    /// <param name="property"></param>
	    /// <returns></returns>
	    private static Gradient SafeGetGradientValue(SerializedProperty property)
	    {
		    if (gradientValuePropertyInfo == null)
		    {
			    Debug.LogError("Gradient Property Info is null!");
			    return null;
		    }
		
		    Gradient gradientValue = gradientValuePropertyInfo.GetValue(property, null) as Gradient;
		    return gradientValue;
	    }
	    
        #endregion

        #region Set

	    #region Set Values
	    
        /// <summary>
        /// Overwrite array of T values.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="objects"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetValues<T>(SerializedProperty property, T[] objects)
        {
            if (property == null)
            {
                Debug.LogError(PropertyIsNullError);
                return;
            }
			
            if (!property.isArray)
            {
                Debug.LogError(PropertyIsNotArrayError);
                return;
            }

            int propertyArraySize = (property.arraySize < 0 ? 0 : property.arraySize);
            int delta = objects.Length - propertyArraySize;
			
            //Increase or decrease elements for the array property.
            if (delta > 0)
            {
                for (int i = 0; i < Mathf.Abs(delta); i++)
                {
                    property.InsertArrayElementAtIndex(0);
                }
            }
            else if (delta < 0)
            {
                for (int i = 0; i < Mathf.Abs(delta); i++)
                {
                    property.DeleteArrayElementAtIndex(0);
                }
            }
			
            //Property array size and objects length must match.
            property.arraySize = objects.Length;
			
            for (int i = 0; i < objects.Length; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);
                try
                {
                    SetValue(element, objects[i]);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
	    
	    #endregion
	    
        /// <summary>
		/// Set value to serialized property relative if possible.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="propertyRelative"></param>
		/// <param name="value"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static void SetValue<T>(SerializedProperty property, string propertyRelative, T value)
		{
			SerializedProperty foundProperty = property.FindPropertyRelative(propertyRelative);
			SetValue(foundProperty, value);
		}
		
		/// <summary>
		/// Set value to serialized property if possible.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <typeparam name="T"></typeparam>
		/// <exception cref="NotImplementedException"></exception>
		public static void SetValue<T>(SerializedProperty property, T value)
		{
			Type type = typeof(T);

			// First, do special Type checks
			if (type.IsEnum)
			{
				property.intValue = (int)(object)value;
				return;
			}

			// Next, check for literal UnityEngine struct-types
			// @note: ->object->ValueT double-casts because C# is too dumb to realize that that the ValueT in each situation is the exact type needed.
			// 	e.g. `return thisSP.colorValue` spits _error CS0029: Cannot implicitly convert type `UnityEngine.Color' to `ValueT'_
			// 	and `return (ValueT)thisSP.colorValue;` spits _error CS0030: Cannot convert type `UnityEngine.Color' to `ValueT'_
			if (typeof(Color).IsAssignableFrom(type))
			{
				property.colorValue = (Color)(object)value;
				return;
			}
			else if (typeof(LayerMask).IsAssignableFrom(type))
			{
				property.intValue = (int)(object)value;
				return;
			}
			else if (typeof(Vector2).IsAssignableFrom(type))
			{
				property.vector2Value = (Vector2)(object)value;
				return;
			}
			else if (typeof(Vector3).IsAssignableFrom(type))
			{
				property.vector3Value = (Vector3)(object)value;
				return;
			}
			else if (typeof(Vector4).IsAssignableFrom(type))
			{
				property.vector4Value = (Vector4)(object)value;
				return;
			}
			else if (typeof(Rect).IsAssignableFrom(type))
			{
				property.rectValue = (Rect)(object)value;
				return;
			}
			else if (typeof(AnimationCurve).IsAssignableFrom(type))
			{
				property.animationCurveValue = (AnimationCurve)(object)value;
				return;
			}
			else if (typeof(Bounds).IsAssignableFrom(type))
			{
				property.boundsValue = (Bounds)(object)value;
				return;
			}
			else if (typeof(Gradient).IsAssignableFrom(type))
			{
				SafeSetGradientValue(property, (Gradient)(object)value);
				return;
			}
			else if (typeof(Quaternion).IsAssignableFrom(type))
			{
				property.quaternionValue = (Quaternion)(object)value;
				return;
			}

			// Next, check if derived from UnityEngine.Object base class
			if (typeof(UnityEngine.Object).IsAssignableFrom(type))
			{
				property.objectReferenceValue = (UnityEngine.Object)(object)value;
				return;
			}

			// Finally, check for native type-families
			if (typeof(int).IsAssignableFrom(type))
			{
				property.intValue = (int)(object)(value);
				return;
			}
			else if (typeof(bool).IsAssignableFrom(type))
			{
				property.boolValue = (bool)(object)value;
				return;
			}
			else if (typeof(float).IsAssignableFrom(type))
			{
				property.floatValue = (float)(object)value;
				return;
			}
			else if (typeof(string).IsAssignableFrom(type))
			{
				property.stringValue = (string)(object)value;
				return;
			}
			else if (typeof(char).IsAssignableFrom(type))
			{
				property.intValue = (int)(object)(value);
				return;
			}
			
			// And if all fails, throw an exception.
			throw new NotImplementedException("Unimplemented propertyType " + property.propertyType + ".");
		}

	    /// <summary>
	    /// Access to SerializedProperty's internal gradientValue property getter,
	    /// in a manner that'll only soft break (returning null) if the property changes
	    /// or disappears in future Unity revs.
	    /// </summary>
	    /// <param name="property"></param>
	    /// <param name="newGradient"></param>
	    /// <returns></returns>
	    private static void SafeSetGradientValue(SerializedProperty property, Gradient newGradient)
	    {
		    if (gradientValuePropertyInfo == null)
		    {
			    Debug.LogError("Gradient Property Info is null!");
			    return;
		    }
		
		    gradientValuePropertyInfo.SetValue(property, newGradient, null);
	    }

        #endregion

        #endregion
        
        /// <summary>
        /// It retrieves all serialized properties from <param name="serializedObject"></param> iterator.
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static SerializedProperty[] GetAllProperties(SerializedObject serializedObject)
        {
            if (serializedObject == null)
            {
                Debug.LogError("Passed "+nameof(serializedObject)+" is null!");
                return null;
            }
		    
            List<SerializedProperty> properties = new List<SerializedProperty>();
            SerializedProperty iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope(Constants.ScriptSerializedPropertyName == iterator.propertyPath))
                {
                    properties.Add(iterator.Copy());
                }
            }
            return properties.ToArray();
        }
        
        
        /// <summary>
        /// Yes, even nested ones.
        /// 
        /// REMEMBER: if you want to save the reference to a property during the iteration you need to use <see cref="SerializedProperty.Copy"/>
        /// method. It's unity flow, follow it!
        /// </summary>
        public static void IterateAllChildren(SerializedProperty property, SerializedPropertyAction @delegate)
        {
            IEnumerator childrenEnumerator = property.GetEnumerator();
            while (childrenEnumerator.MoveNext())
            {
                SerializedProperty childProperty = childrenEnumerator.Current as SerializedProperty;
                if (childProperty == null)
                {
                    continue;
                }
                
                @delegate.Invoke(childProperty);
            }
        }
        
        #region Get Parent Level
        
        /// <summary>
        /// Retrieve last parent of Serialized Property as object.
        /// Use this for 'read-only' behaviour.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool TryGetParentLevel(SerializedProperty property, out object parent)
        {
            if (TryGetParentsLevels(property, out object[] parents))
            {
                parent = parents.Last();
                return true;
            }
            else
            {
                parent = null;
                return false;
            }
        }
        
        /// <summary>
        /// Retrieve a list of all parents Serialized Property as sorted objects array.
        /// Use this for 'read-only' behaviour.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="parents"></param>
        /// <returns></returns>
        public static bool TryGetParentsLevels(SerializedProperty property, out object[] parents)
        {
            if (property == null)
            {
                parents = null;
                return false;
            }

            string name = property.name;
            string path = property.propertyPath;
            string revisedPath = path.Replace(name, string.Empty);
            
            try
            {
                bool value = true;
                List<object> tmpParents = new List<object>();
                
                value = GetParentsLevelsInternal(property, ref tmpParents);

                parents = tmpParents.ToArray();
                return value;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                parents = null;
                return false;
            }
        }
        
        private static bool GetParentsLevelsInternal(SerializedProperty property, ref List<object> parents, bool debug = false)
        {
            if (parents == null)
            {
                return false;
            }
            
            string name = property.name;
            string path = property.propertyPath;
            string revisedPath = path.Replace(name, string.Empty);
       
            bool value = true;
                
            Object targetObject = property.serializedObject.targetObject;
            object contextObject = targetObject;
                
            string[] splitResults = revisedPath.Split(new []{'.'});
            if (debug)
            {
                Debug.Log($"Length {splitResults.Length.ToString()}");
            }

            int i = 0;

            #region Get Parents Levels

            while (i < splitResults.Length)
            {
                string fieldName = splitResults[i];
                if (string.IsNullOrEmpty(fieldName))
                {
                    if (debug)
                    {
                        Debug.Log($"{i.ToString()} --> {fieldName}");
                    }
                }
                else if (fieldName == Constants.Array && i + 1 < splitResults.Length && splitResults[i + 1].Contains("data"))
                {
                    #region Array Element Property
                        
                    string data = splitResults[i + 1];
                    if (int.TryParse(data.Replace("data[", string.Empty).Replace("]", string.Empty), out int w))
                    {
                        if (ArrayUtils.IsArray(contextObject) && ArrayUtils.TryCast(contextObject, out object[] array))
                        {
                            if (ArrayUtils.TryGetValue(array, w, out object element))
                            {
                                object previousContextObject = contextObject;
                                parents.Add(previousContextObject);
                                contextObject = element;

                                i += 2;
                                continue;
                            }
                            else
                            {
                                value = false;
                                break;
                            }
                        }
                        else
                        {
                            value = false;
                            break;
                        }
                    }
                    else
                    {
                        value = false;
                        break;
                    }
                        
                    #endregion
                }
                else if (ReflectionUtils.HasField(contextObject, fieldName, out FieldInfo field))
                {
                    if (debug)
                    {
                        Debug.Log($"{i.ToString()} --> {fieldName}");
                    }

                    object previousContextObject = contextObject;
                    parents.Add(previousContextObject);
                    contextObject = field.GetValue(previousContextObject);
                }
                else
                {
                    value = false;
                    break;
                }
                    
                i++;
            }
                
            #endregion
                
            parents.Add(contextObject);
            if (debug)
            {
                Debug.Log($"Parents Length {parents.Count.ToString()} / Split Results Length {splitResults.Length.ToString()}");
            }

            return value;
        }
        
        #endregion

        #region Edit Parent Level

        /// <summary>
        /// References the last parent of a property and regardless of method called it folds back 'exposed' values by reflection.
        /// Use this for get-set behaviour of inspector nest structure.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="callback"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool TryEditLastParentLevel(SerializedProperty property, GenericRefAction callback, bool debug = false)
        {
            if (property == null)
            {
                return false;
            }

            string name = property.name;
            string path = property.propertyPath;
            string revisedPath = path.Replace(name, string.Empty);
            
            try
            {
                bool value = true;
                List<object> tmpParents = new List<object>();
                
                Object targetObject = property.serializedObject.targetObject;
                object contextObject = targetObject;
                
                string[] splitResults = revisedPath.Split(new []{'.'}, StringSplitOptions.RemoveEmptyEntries);
                List<string> aggregatedSplitResults = new List<string>();
                if (debug)
                {
                    Debug.Log($"Length {splitResults.Length.ToString()}");
                }

                #region Unfold Parents
                
                int i = 0;
                
                while (i < splitResults.Length)
                {
                    string fieldName = splitResults[i];
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        if (debug)
                        {
                            Debug.Log($"{i.ToString()} --> {fieldName}");
                        }

                        //Do Nothing
                    }
                    else if (fieldName == Constants.Array && i + 1 < splitResults.Length && splitResults[i + 1].Contains("data"))
                    {
                        #region Array Element Property
                        
                        string data = splitResults[i + 1];
                        if (int.TryParse(data.Replace("data[", string.Empty).Replace("]", string.Empty), out int w))
                        {
                            if (ArrayUtils.IsArray(contextObject) && ArrayUtils.TryCast(contextObject, out object[] array))
                            {
                                if (ArrayUtils.TryGetValue(array, w, out object element))
                                {
                                    object previousContextObject = ((object)(Array)array);
                                    tmpParents.Add(previousContextObject);
                                    contextObject = element;

                                    if (debug)
                                    {
                                        Debug.Log($"{i.ToString()} --> {fieldName}");
                                        Debug.Log($"{(i + 1).ToString()} --> {data}");
                                    }

                                    aggregatedSplitResults.Add(fieldName + "." + data);

                                    i += 2;
                                    continue;
                                }
                                else
                                {
                                    value = false;
                                    break;
                                }
                            }
                            else
                            {
                                value = false;
                                break;
                            }
                        }
                        else
                        {
                            value = false;
                            break;
                        }
                        
                        #endregion
                    }
                    else if (ReflectionUtils.HasField(contextObject, fieldName, out FieldInfo field))
                    {
                        if (debug)
                        {
                            Debug.Log($"{i.ToString()} --> {fieldName}");
                        }

                        aggregatedSplitResults.Add(fieldName);
                        var previousContextObject = contextObject;
                        tmpParents.Add(previousContextObject);
                        contextObject = field.GetValue(previousContextObject);

                    }
                    else
                    {
                        value = false;
                        break;
                    }
                    
                    i++;
                }
                
                #endregion
                
                callback?.Invoke(ref contextObject);
                tmpParents.Add(contextObject);
	            
                if (debug)
                {
                    Debug.Log($"Parents Length {tmpParents.Count.ToString()} / Split Results Length {splitResults.Length.ToString()} / Aggregated Split Results Length {aggregatedSplitResults.Count.ToString()}");
                }

                splitResults = aggregatedSplitResults.ToArray();
                i = splitResults.Length - 1;
                object previousContext = (i >= 0) ? tmpParents[i] : targetObject;
	            
                #region Fold Parents

	            if (i >= 0)
	            {
		            while (i >= 0)
		            {
			            string fieldName = splitResults[i];
			            if (string.IsNullOrEmpty(fieldName))
			            {
				            if (debug)
				            {
					            Debug.Log($"Reverse loop {i.ToString()} --> {fieldName}");
				            }
			            }
			            else if (fieldName.Contains(Constants.Array) && fieldName.Contains("data"))
			            {
				            #region Array Element Property

				            string data = fieldName;
				            string parsingString = data.Replace(Constants.Array, string.Empty)
					            .Replace(".", string.Empty).Replace("data[", string.Empty).Replace("]", string.Empty);
				            if (int.TryParse(parsingString, out int w))
				            {
					            if (ArrayUtils.IsArray(previousContext) &&
					                ArrayUtils.TryCast(previousContext, out object[] array))
					            {
						            object element = tmpParents[i + 1];
						            if (ArrayUtils.TrySetValue(array, w, element))
						            {
							            Array destinationArray = Array.CreateInstance(element.GetType(), array.Length);
							            Array.Copy(array, destinationArray, array.Length);
							            tmpParents[i] = destinationArray;
							            if (debug)
							            {
								            Debug.Log($"Reverse loop {i.ToString()} --> {fieldName}");
							            }
						            }
						            else
						            {
							            value = false;
							            break;
						            }
					            }
					            else
					            {
						            value = false;
						            break;
					            }
				            }
				            else
				            {
					            value = false;
					            break;
				            }

				            #endregion
			            }
			            else if (ReflectionUtils.HasField(previousContext, fieldName, out FieldInfo field))
			            {
				            if (debug)
				            {
					            Debug.Log($"Reverse loop {i.ToString()} --> {fieldName}");
				            }

				            field.SetValue(previousContext, tmpParents[i + 1]);
			            }

			            i--;
			            if (i < 0)
			            {
				            break;
			            }

			            previousContext = tmpParents[i];
		            }
	            }
	            else
	            {
		            value = true;
		            
		            if (debug)
		            {
			            Debug.Log($"{i.ToString()} is less than 0");
		            }
	            }

	            #endregion

                return value;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        #endregion
    }
}
#endif

