using Mono.Cecil;
using Mono.Cecil.Cil;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Negates a value and pushes the result onto the evaluation stack.
	/// </summary>
	public class Neg : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			return processor.Create(OpCodes.Neg);
		}
	}
}
