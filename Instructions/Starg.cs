using Mono.Cecil;
using Mono.Cecil.Cil;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Stores the value on top of the evaluation stack in the argument slot at a specified index.
	/// </summary>
	public class Starg : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Value == null)
				return null;

			return processor.Create(OpCodes.Starg, int.Parse(patchInstruction.Value));
		}
	}
}
