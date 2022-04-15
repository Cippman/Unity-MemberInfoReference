
using Serializable = System.SerializableAttribute;
using UnityEngine;

namespace CippSharp.Members
{
   [Serializable]
   public class ObjectMemberReference : MemberInfoReference<Object>
   {
      
   }
   
   [Serializable]
   public class GameObjectMemberReference : MemberInfoReference<GameObject>
   {
      
   }
   
   [Serializable]
   public class TransformMemberReference : MemberInfoReference<Transform>
   {
      
   }
}
