using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace CippSharp.Members
{
    /// <summary>
    /// Hold static helpful methods for arrays.
    /// This part is dedicated to topmost generic arrays methods
    /// </summary>
    internal static class ArrayUtils
    {
        /// <summary>
        /// Retrieve if context object is an array.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsArray(object context)
        {
            return context.GetType().IsArray;
        }
        
         /// <summary>
        /// Try to get value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="element"></param>
        /// <returns>success</returns>
        public static bool TryGetValue(object[] array, int index, out object element)
        {
            try
            {
                element = array[index];
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                element = null;
                return false;
            }
        }

        /// <summary>
        /// Try to set value
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="element"></param>
        /// <returns>success</returns>
        public static bool TrySetValue(object[] array, int index, object element)
        {
            try
            {
                array[index] = element;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Try to cast an object to object array
        /// </summary>
        /// <param name="value"></param>
        /// <param name="array"></param>
        /// <returns>success</returns>
        public static bool TryCast(object value, out object[] array)
        {
            try
            {
                array = ((Array)value).Cast<object>().ToArray();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                array = null;
                return false;
            }
        }
        
        #region Contains / Find
        
        /// <summary>
        /// Similar to Any of <see> <cref>System.linq</cref> </see>
        /// it retrieve a valid index of the first element matching the predicate.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Any<T>(List<T> list, Predicate<T> predicate, out int index)
        {
            index = IndexOf(list, predicate);
            return index > -1;
        }
        
        /// <summary>
        /// Similar to Any of <see> <cref>System.linq</cref> </see>
        /// it retrieve a valid index of the first element matching the predicate.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="predicate"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Any<T>(T[] array, Predicate<T> predicate, out int index)
        {
            index = IndexOf(array, predicate);
            return index > -1;
        }

        #endregion
        
        #region Index Of

        /// <summary>
        /// Retrieve index if array contains an element with given predicate.
        /// Otherwise -1
        /// </summary>
        /// <param name="array"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>-1 if it fails</returns>
        public static int IndexOf<T>(T[] array, Predicate<T> predicate)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (predicate.Invoke(array[i]))
                {
                    return i;
                }
            }

            return -1;
        }
        
        /// <summary>
        /// Retrieve index if list contains an element with given predicate.
        /// Otherwise -1
        /// </summary>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>-1 if it fails</returns>
        public static int IndexOf<T>(List<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate.Invoke(list[i]))
                {
                    return i;
                }
            }

            return -1;
        }
        
        /// <summary>
        /// Retrieve index if enumerable contains an element with given predicate.
        /// Otherwise -1
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int IndexOf<T>(IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            T[] array = enumerable.ToArray();
            return IndexOf(array, predicate);
        }

        #endregion

        #region Is Null or Empty
        
        /// <summary>
        /// Returns true if the given array is null or empty
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(T[] array)
        {
            return array == null || array.Length < 1;
        }
        
        /// <summary>
        /// Returns true if the given list is null or empty
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(List<T> list)
        {
            return list == null || list.Count < 1;
        }

        #endregion

        #region Is Valid Index

        /// <summary>
        /// Returns true if the given index is the array range.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsValidIndex<T>(int index, T[] array)
        {
            return index >= 0 && index < array.Length;
        }
        
        /// <summary>
        /// Returns true if the given index is in the list range.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsValidIndex<T>(int index, List<T> list)
        {
            return index >= 0 && index < list.Count;
        }
        
        #endregion
        
        #region Select

        /// <summary>
        /// Select If util. Similar to System.linq Select but with a predicate to check.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="predicate"></param>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        public static IEnumerable<F> SelectIf<T, F>(IEnumerable<T> array, Predicate<T> predicate, Func<T, F> func)
        {
            return (from element in array where predicate.Invoke(element) select func.Invoke(element));
        }
        
        #endregion
    }
}
