using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Unconditionally transfers control to a target instruction.
	/// </summary>
	public class Br : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.TargetIndex < 0)
				return null;

			Instruction targetInstruction = methodDef.Body.Instructions.ElementAt(patchInstruction.TargetIndex);

			if (targetInstruction == null)
				return null;

			return processor.Create(OpCodes.Br, targetInstruction);
		}
	}
}
