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
            return Generate(Type.GetType(type));
        }

        public static Instance Generate(Type type) {
            if(type == null)
                return null;

            DynamicMethod dynMethod = new DynamicMethod("construct", type, null);
            ILGenerator ilGen = dynMethod.GetILGenerator();

            ilGen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            ilGen.Emit(OpCodes.Ret);

            return (Instance)dynMethod.CreateDelegate(typeof(Instance));
        }
    }
}