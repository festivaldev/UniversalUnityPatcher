using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace UniversalUnityPatcher.Instructions {
	public abstract class InstructionBase {
		public virtual Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			throw new NotImplementedException();
		}

		public virtual bool CompareInstruction(Instruction a, Instruction b) {
			return false;
		}
	}
}
