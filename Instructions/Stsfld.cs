using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Replaces the value of a static field with a value from the evaluation stack.
	/// </summary>
	public class Stsfld : InstructionBase {
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
			TypeReference typeReference = _assemblyDef.MainModule.GetType(typeName);

			// Get Field
			FieldDefinition fieldDefinition = typeReference.Resolve().Fields.FirstOrDefault(f => f.Name == fieldName);


			return processor.Create(OpCodes.Stsfld, assemblyDef.MainModule.ImportReference(fieldDefinition));
		}
	}
}
