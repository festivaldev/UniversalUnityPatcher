using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Pushes the value of a static field onto the evaluation stack.
	/// </summary>
	public class Ldsfld : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Type == null || patchInstruction.Field == null)
				return null;

			string typeName = patchInstruction.Type;
			string fieldName = patchInstruction.Field;

			AssemblyDefinition _assemblyDef = assemblyDef;
			if (patchInstruction.Assembly != null)
				_assemblyDef = Patcher.ResolveAssembly(patchInstruction.Assembly);

			if (_assemblyDef == null)
				return null;

			// Get Type
			TypeDefinition typeDefinition = _assemblyDef.MainModule.GetType(typeName);

			// Get Field
			FieldDefinition fieldDefinition = typeDefinition.Fields.FirstOrDefault(f => f.Name == fieldName);

			return processor.Create(OpCodes.Ldsfld, fieldDefinition);
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
