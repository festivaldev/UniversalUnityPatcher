﻿using Mono.Cecil;
using Mono.Cecil.Cil;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Pushes a new object reference to a string literal stored in the metadata.
	/// </summary>
	public class Ldstr : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Value == null)
				return null;

			return processor.Create(OpCodes.Ldstr, patchInstruction.Value);
		}

		public override bool CompareInstruction(Instruction a, Instruction b) {
			if (!a.OpCode.Equals(b.OpCode)) return false;
			if (!a.Operand.GetType().Equals(b.Operand.GetType())) return false;

			return true;
		}
	}
}
