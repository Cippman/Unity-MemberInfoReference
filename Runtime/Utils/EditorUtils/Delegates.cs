#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CippSharp.Members
{
    /// <summary>
    /// For Iteration of an array of SerializedProperties
    /// </summary>
    /// <param name="element"></param>
    /// <param name="index"></param>
    public delegate void ForSerializedPropertyAction(SerializedProperty element, int index);
    
    /// <summary>
    /// Reference object callback.
    /// </summary>
    /// <param name="context"></param>
    public delegate void GenericRefAction(ref object context);

    /// <summary>
    /// Generic callback with serialized property
    /// </summary>
    /// <param name="property"></param>
    public delegate void SerializedPropertyAction(SerializedProperty property);
    
    
    /// <summary>
    /// Generic callback with serialized property
    /// </summary>
    
    
    /// <summary>
    /// Custom Delegate to draw a serialized property
    /// </summary>
    /// <param name="property"></param>
    public delegate void DrawSerializedPropertyDelegate(SerializedProperty property); 
    
    /// <summary>
    /// Custom Delegate to draw a property in <see cref="EditorGUI"/>
    /// </summary>
    /// <param name="rect">the rect used and edited to draw the property</param>
    /// <param name="property">the property to draw</param>
    public delegate void DrawSerializedPropertyDelegate1(ref Rect rect, SerializedProperty property);
    
    /// <summary>
    /// Custom Delegate to get property height
    /// </summary>
    /// <param name="property">the property to retrieve the height</param>
    public delegate float GetPropertyHeightDelegate(SerializedProperty property);
}
#endif
