using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Pushes an unmanaged pointer (type native int) to the native code implementing a specific method onto the evaluation stack.
	/// </summary>
	public class Ldftn : InstructionBase {
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

			TypeDefinition typeDefinition = _assemblyDef.MainModule.GetType(typeName);

			MethodDefinition methodDefinition = typeDefinition.Methods.FirstOrDefault(m => m.Name == methodName);
			MethodReference methodReference = assemblyDef.MainModule.ImportReference(methodDefinition);

			return processor.Create(OpCodes.Ldftn, methodReference);
		}
	}
}
