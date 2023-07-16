using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Calls the method indicated by the passed method descriptor.
	/// </summary>
	public class Call : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Type == null || patchInstruction.Method == null)
				return null;

			string typeName = patchInstruction.Type;
			string methodName = patchInstruction.Method;

			AssemblyDefinition _assemblyDef = assemblyDef;
			if (patchInstruction.Assembly != null)
				_assemblyDef = Patcher.ResolveAssembly(patchInstruction.Assembly);

			if (_assemblyDef == null)
				return null;

			// Get Type
			TypeDefinition typeDefinition = _assemblyDef.MainModule.GetType(typeName);

			// Get Method
			MethodDefinition methodDefinition = typeDefinition.Methods.FirstOrDefault(m => m.Name == methodName);
			MethodReference methodReference = assemblyDef.MainModule.ImportReference(methodDefinition);

			//if (patchInstruction.Parameters.Count > 0) {
			//	List<TypeReference> genericParameters = new List<TypeReference>();

			//	foreach (PatchInstructionParameter genericAssembly in patchInstruction.Parameters) {
			//		AssemblyDefinition genericAssemblyDef = Patcher.ResolveAssembly(genericAssembly.Assembly);
			//		TypeReference genericTypeRef = assemblyDef.MainModule.ImportReference(genericAssemblyDef.MainModule.GetType(genericAssembly.Type));

			//		methodReference.GenericParameters.Add(new GenericParameter(genericTypeRef.Name, methodDefinition));
			//	}
			//}

			// Return Instruction
			return processor.Create(OpCodes.Call, methodReference);
		}

		public override bool CompareInstruction(Instruction a, Instruction b) {
			if (!a.OpCode.Equals(b.OpCode)) return false;
			if (!a.Operand.GetType().Equals(b.Operand.GetType())) return false;

			if (a.Operand is MethodReference && b.Operand is MethodReference) {
				var _a = (MethodReference)a.Operand;
				var _b = (MethodReference)b.Operand;

				return _a.FullName == _b.FullName &&
					_a.DeclaringType.FullName == _b.DeclaringType.FullName &&
					_a.ReturnType.FullName == _b.ReturnType.FullName &&
					_a.Parameters.All(aParam => _b.Parameters.Any(bParam => aParam.Name == bParam.Name && aParam.ParameterType.FullName == bParam.ParameterType.FullName)) &&
					_a.GenericParameters.All(aParam => _b.GenericParameters.Any(bParam => aParam.Name == bParam.Name));
			}

			return false;
		}
	}
}
