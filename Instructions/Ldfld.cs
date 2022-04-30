using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Finds the value of a field in the object whose reference is currently on the evaluation stack.
	/// </summary>
	public class Ldfld : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Field == null)
				return null;

			string fieldName = patchInstruction.Field;

			FieldDefinition fieldDefinition = typeDef.Fields.FirstOrDefault(f => f.Name == fieldName);

			return processor.Create(OpCodes.Ldfld, fieldDefinition);
		}
	}
}
