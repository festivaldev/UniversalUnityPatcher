using Mono.Cecil;
using Mono.Cecil.Cil;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Compute the bitwise complement of the two integer values on top of the stack and pushes the result onto the evaluation stack.
	/// </summary>
	public class Or : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			return processor.Create(OpCodes.Or);
		}
	}
}
