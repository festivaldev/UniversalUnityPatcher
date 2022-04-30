using Mono.Cecil;
using Mono.Cecil.Cil;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Loads an argument (referenced by a specified index value) onto the stack.
	/// </summary>
	public class Ldarg : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Value == null)
				return null;

			return processor.Create(OpCodes.Ldarg, int.Parse(patchInstruction.Value));
		}
	}
}
