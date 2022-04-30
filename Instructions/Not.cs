using Mono.Cecil;
using Mono.Cecil.Cil;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Computes the bitwise complement of the integer value on top of the stack and pushes the result onto the evaluation stack as the same type.
	/// </summary>
	public class Not : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			return processor.Create(OpCodes.Not);
		}
	}
}
