using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;
using System.Reflection;

namespace UniversalUnityPatcher.Instructions {
	/// <summary>
	/// Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.
	/// </summary>
	public class Newobj : InstructionBase {
		public override Instruction ParseInstruction(ILProcessor processor, AssemblyDefinition assemblyDef, TypeDefinition typeDef, MethodDefinition methodDef, PatchInstruction patchInstruction) {
			if (patchInstruction.Type == null)
				return null;

			string typeName = patchInstruction.Type;

			AssemblyDefinition _assemblyDef = assemblyDef;
			if (patchInstruction.Assembly != null)
				_assemblyDef = Patcher.ResolveAssembly(patchInstruction.Assembly);

			if (_assemblyDef == null)
				return null;

			TypeDefinition typeDefinition = _assemblyDef.MainModule.GetType(typeName);

			MethodDefinition methodDefinition = typeDefinition.GetConstructors().FirstOrDefault();
			MethodReference methodReference = assemblyDef.MainModule.ImportReference(methodDefinition);

			return processor.Create(OpCodes.Newobj, methodReference);
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
