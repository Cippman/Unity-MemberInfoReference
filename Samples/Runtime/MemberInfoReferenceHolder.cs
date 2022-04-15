#if UNITY_EDITOR
using System;
using UnityEngine;

namespace CippSharp.Members.Samples
{
    public class MemberInfoReferenceHolder : MonoBehaviour
    {
        private static readonly string LogName = $"[{nameof(MemberInfoReferenceHolder)}]: ";
        
        [Serializable]
        public class NestedExample
        {
            /// <summary>
            /// This will filter all method with return type of int or bool
            /// with name that contains "al"
            /// </summary>
            [MemberFilter(new []{"r:int,bool", "nc:al"})]
            public ObjectMemberReference objectMemberReference = new ObjectMemberReference();
        }
        
        /// <summary>
        /// This will filter all method members with 0 parameters
        /// </summary>
        [MemberFilter(false, false, true, new []{"mpc:0"})]
        public TransformMemberReference transformMemberReference = new TransformMemberReference();
        
        /// <summary>
        /// This will filter all members with name that contains 's' char
        /// </summary>
        [MemberFilter(new []{"nc:s"})]
        public GameObjectMemberReference gameObjectMemberReference = new GameObjectMemberReference();
        
        /// <summary>
        /// This will filter all members with returning type of 'float'
        /// </summary>
        [MemberFilter(new []{"r:float"})]
        public ObjectMemberReference objectMemberReference = new ObjectMemberReference();
        
        //They are Working nested? 
        public NestedExample nestedExample = new NestedExample();

        private void Start()
        {
            Debug.Log(LogName+$"{nameof(transformMemberReference)} get = {transformMemberReference.GetOrInvoke(null)}.", this);
            Debug.Log(LogName+$"{nameof(objectMemberReference)} get = {objectMemberReference.GetOrInvoke(null)}.", this);
        }
    }
}
#endif