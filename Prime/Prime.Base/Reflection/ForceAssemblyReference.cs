using System;

namespace Prime.Core
{
    /// <summary>
    /// https://stackoverflow.com/a/46405537
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ForceAssemblyReference: Attribute
    {        
        public ForceAssemblyReference(Type forcedType)
        {
            //not sure if these two lines are required since 
            //the type is passed to constructor as parameter, 
            //thus effectively being used
            Action<Type> noop = _ => { };
            noop(forcedType);
        }
        public ForceAssemblyReference(Type forcedType, Type forcedType2)
        {
            //not sure if these two lines are required since 
            //the type is passed to constructor as parameter, 
            //thus effectively being used
            Action<Type> noop = _ => { };
            noop(forcedType);
            noop(forcedType2);
        }
        public ForceAssemblyReference(Type forcedType, Type forcedType2, Type forcedType3)
        {
            //not sure if these two lines are required since 
            //the type is passed to constructor as parameter, 
            //thus effectively being used
            Action<Type> noop = _ => { };
            noop(forcedType);
            noop(forcedType2);
            noop(forcedType3);
        }
        public ForceAssemblyReference(Type forcedType, Type forcedType2, Type forcedType3, Type forcedType4)
        {
            //not sure if these two lines are required since 
            //the type is passed to constructor as parameter, 
            //thus effectively being used
            Action<Type> noop = _ => { };
            noop(forcedType);
            noop(forcedType2);
            noop(forcedType3);
            noop(forcedType4);
        }
    }
}