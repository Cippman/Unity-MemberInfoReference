
namespace CippSharp.Members
{
    public interface IMemberReferenceBase
    {
        /// <summary>
        /// Member Reference Type
        /// </summary>
        System.Type Type {get; }
    
        /// <summary>
        /// Current Target
        /// </summary>
        object Target { get; }
        
        /// <summary>
        /// Selected Member
        /// </summary>
        string WrittenMember { get; }
        
        /// <summary>
        /// Binding Flags
        /// </summary>
        System.Reflection.BindingFlags Flags { get; }
        
        /// <summary>
        /// Retrieve value or invokes member
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object GetOrInvoke(params object[] parameters);

        /// <summary>
        /// Set value or invokes members
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object SetOrInvoke(params object[] parameters);
    }
}
