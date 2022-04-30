using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
	/// </summary>
	public class Callvirt : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Type == null || patchInstruction.Method == null)
				return null;

			string typeName = patchInstruction.Type;
			string methodName = patchInstruction.Method;

			AssemblyDefinition _assemblyDef = assemblyDef;
			if (patchInstruction.Assembly != null)
				_assemblyDef = Patcher.ResolveAssembly(patchInstruction.Assembly);

			if (_assemblyDef == null)
				return null;

			// Get Type
			TypeDefinition typeDefinition = _assemblyDef.MainModule.GetType(typeName);

			// Get Method
			MethodDefinition methodDefinition = typeDefinition.Methods.FirstOrDefault(m => m.Name == methodName);
			MethodReference methodReference = assemblyDef.MainModule.ImportReference(methodDefinition);

			//if (patchInstruction.Parameters.Count > 0) {
			//	List<TypeReference> genericParameters = new List<TypeReference>();

			//	foreach (PatchInstructionParameter genericAssembly in patchInstruction.Parameters) {
			//		AssemblyDefinition genericAssemblyDef = Patcher.ResolveAssembly(genericAssembly.Assembly);
			//		TypeReference genericTypeRef = assemblyDef.MainModule.ImportReference(genericAssemblyDef.MainModule.GetType(genericAssembly.Type));

			//		methodReference.GenericParameters.Add(new GenericParameter(genericTypeRef.Name, methodDefinition));
			//	}
			//}

			// Return Instruction
			return processor.Create(OpCodes.Callvirt, methodReference);
		}
	}
}
