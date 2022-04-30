using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.
	/// </summary>
	public class Newobj : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Type == null)
				return null;

			string typeName = patchInstruction.Type;

			AssemblyDefinition _assemblyDef = assemblyDef;
			if (patchInstruction.Assembly != null)
				_assemblyDef = Patcher.ResolveAssembly(patchInstruction.Assembly);

			if (_assemblyDef == null)
				return null;

			TypeDefinition typeDefinition = _assemblyDef.MainModule.GetType(typeName);

			var genericAssembly = patchInstruction.Parameters.First();
			AssemblyDefinition genericAssemblyDef = Patcher.ResolveAssembly(genericAssembly.Assembly);
			TypeReference genericTypeRef = assemblyDef.MainModule.ImportReference(genericAssemblyDef.MainModule.GetType(genericAssembly.Type));

			GenericInstanceType genericInstanceType = assemblyDef.MainModule.ImportReference(Type.GetType(patchInstruction.Type)).MakeGenericInstanceType(genericTypeRef);
			MethodReference genericInstanceCtor = Patcher.MakeHostInstanceGeneric(assemblyDef.MainModule.ImportReference(genericInstanceType.Resolve().Methods.First(_ => _.Name == patchInstruction.Method)), genericTypeRef);

			return processor.Create(OpCodes.Newobj, genericInstanceCtor);
		}
	}
}
