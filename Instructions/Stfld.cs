using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Replaces the value stored in the field of an object reference or pointer with a new value.
	/// </summary>
	public class Stfld : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Field == null)
				return null;

			string fieldName = patchInstruction.Field;

			FieldDefinition fieldDefinition = typeDef.Fields.FirstOrDefault(f => f.Name == fieldName);

			return processor.Create(OpCodes.Stfld, fieldDefinition);
		}
	}
}
