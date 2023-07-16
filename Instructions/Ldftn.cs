using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Pushes an unmanaged pointer (type native int) to the native code implementing a specific method onto the evaluation stack.
	/// </summary>
	public class Ldftn : InstructionBase {
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

			TypeDefinition typeDefinition = _assemblyDef.MainModule.GetType(typeName);

			MethodDefinition methodDefinition = typeDefinition.Methods.FirstOrDefault(m => m.Name == methodName);
			MethodReference methodReference = assemblyDef.MainModule.ImportReference(methodDefinition);

			return processor.Create(OpCodes.Ldftn, methodReference);
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
