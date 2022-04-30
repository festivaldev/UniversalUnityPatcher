using Mono.Cecil;
using Mono.Cecil.Cil;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
	/// </summary>
	public class Ldc_I4 : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Value == null)
				return null;

			return processor.Create(OpCodes.Ldc_I4, int.Parse(patchInstruction.Value));
		}
	}
}
