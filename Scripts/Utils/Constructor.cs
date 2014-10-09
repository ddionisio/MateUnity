using System;
using System.Reflection;
using System.Reflection.Emit;

namespace M8 {
    public struct Constructor {
        public delegate object Instance();

        /// <summary>
        /// Create a constructor for instantiating given type name (Type.ToString())
        /// 
        /// Ensure that given type has an empty constructor available.
        /// 
        /// Example use:
        /// Constructor.Instance c = Constructor.Generate("Some.Thing");
        /// Some.Thing obj = c() as Some.Thing;
        /// </summary>
        public static Instance Generate(string type) {
            Type constructType = Type.GetType(type);

            if(constructType == null)
                return null;

            DynamicMethod dynMethod = new DynamicMethod("construct", constructType, null);
            ILGenerator ilGen = dynMethod.GetILGenerator();

            ilGen.Emit(OpCodes.Newobj, constructType.GetConstructor(Type.EmptyTypes));
            ilGen.Emit(OpCodes.Ret);

            return (Instance)dynMethod.CreateDelegate(typeof(Instance));
        }
    }
}