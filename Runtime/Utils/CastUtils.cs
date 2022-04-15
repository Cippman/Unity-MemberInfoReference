//
// Author: Alessandro Salani (Cippo)
//

namespace CippSharp.Members
{
    internal static class CastUtils
    {
        /// <summary>
        /// Casts an object to type T. In case of failure returns T default value. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToOrDefault<T>(object target)
        {
            try
            {
                return (T) target;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Returns true if successfully casted to T.
        /// Casts an object to type T.
        /// In case of failure returns false and T default value.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>success</returns>
        public static bool To<T>(object target, out T result)
        {
            try
            {
                result = (T) target;
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }
    }
}
