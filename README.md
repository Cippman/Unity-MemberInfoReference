# Unity-MemberInfoReference
Create your custom serializable class that
holds the reference to an UnityEngine.Object and 
get one of  his members!

### Purpose: 
The main purpose of MemberInfoReference is to present 
a code friendly solution to reference a MemberInfo of a
UnityEngine.Object and to expose it in the inspector.


### Contents:
The repository comes with main class AMemberInfoReferenceBase, MemberInfoReference'T; 
support scripts and a MemberFilter attribute.

### How to Install:
- Option 1 (readonly) now it supports Unity Package Manager so you can download by copy/paste the git url in 'Package Manager Window + Install From Git'.
  As said this is a readonly solution so you cannot access all files this way.
- Option 2 (classic) download this repository as .zip; Extract the files; Drag 'n' drop the extracted folder in your unity project (where you prefer).
- Option 3 (alternative) add this as submodule / separate repo in your project
  

###  Notes:
Multiple Components: 
- For multiple components to select on same gameObject,
by default only the first is of correct type is retrieved. 
To assign another component that is not the first you should open [two inspectors](https://photos.app.goo.gl/Pw8Hq1o3qnCGoica6):
with one locked on the object with the interface
reference you can use the other to manually assign
the appointed component

Code:
* The process to create your custom reference
 is similar to create custom events with UnityEvents.
Just inherit from MemberInfoReference'T with your custom
serializable class where T is the UnityEngine.Object type. 
  * By code you can access his value by using GetOrInvoke and set value using SetOrInvoke  
- Be aware of what are you coding: only MemberInfo 
that are Fields or Properties supports Get and Set, Methods will be
invoked anyway.
* As for the [InterfaceReference](https://github.com/Cippman/Unity-InterfaceReference) this is intended 
to be used by inspector. If you need to use Reflection to access a specific member at runtime there are better solutions than this. 
    * For example by already caching the target MemberInfo of interested type as variable at Start or as static

So yes, this is more a small showoff of what Reflection and CustomEditors can do
...and I also presume the reason why the original may be _unmaintained_ :D


### History: 
Inspired by ([lazlo reflection](https://github.com/lazlo-bonin/ludiq-reflection)) with
his UnityMember _(that I also forked to see the magic of reflection)_ I decided to bring my solution.

I admit that this is a sort of _personal study_ to see if I was able to replicate
by myself something like that. Indeed this version is a little different as implementation and you have to
specify which UnityEngine.Object is affected to select his MemberInfo.

Thanks Lazlo
 

### Links:
- [original](https://github.com/lazlo-bonin/ludiq-reflection)
- [repo](https://github.com/Cippman/Unity-MemberInfoReference.git)

### Support:
- [tip jar](https://www.amazon.it/photos/share/Gbg3FN0k6pjG6F5Ln3dqQEmwO0u4nSkNIButm3EGtit)
