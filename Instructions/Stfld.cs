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

		public override bool CompareInstruction(Instruction a, Instruction b) {
			if (!a.OpCode.Equals(b.OpCode)) return false;
			if (!a.Operand.GetType().Equals(b.Operand.GetType())) return false;

			if (a.Operand is FieldReference && b.Operand is FieldReference) {
				var _a = (FieldReference)a.Operand;
				var _b = (FieldReference)b.Operand;

				return _a.FullName == _b.FullName &&
					_a.DeclaringType.FullName == _b.DeclaringType.FullName &&
					_a.FieldType.FullName == _b.FieldType.FullName;
			}

			return false;
		}
	}
}
