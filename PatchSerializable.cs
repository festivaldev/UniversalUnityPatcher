using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace UniversalUnityPatcher {
	[XmlRoot]
	public class PatchDefinition {
		[XmlAttribute]
		public string Name;

		[XmlElement("Patch")]
		public List<Patch> Patches;
	}

	public class Patch {
		private bool enabled = true;

		[XmlAttribute]
		public string Name;

		[XmlAttribute]
		public string ShortDescription;

		[XmlElement]
		public string Description;

		[XmlAttribute("Enabled")]
		public bool IsEnabled {
			get { return enabled; }
			set {
				enabled = bool.Parse(value.ToString());
			}
		}

		[XmlArray]
		public List<FileHash> FileHashes;

		[XmlElement]
		public PatchAssemblyList Assemblies;

		[XmlElement("Type")]
		public List<PatchType> Types = new List<PatchType>();

		public override string ToString() {
			return $"[[Patch: {Name}] Enabled = {IsEnabled}; ShortDescription = {ShortDescription}; Description = {Description}]";
		}
	}

	public class FileHash {
		[XmlAttribute]
		public string Name;

		[XmlElement("string")]
		public List<string> ValidHashes;

		public override string ToString() {
			return $"Name: {Name}, ValidHashes({ValidHashes.Count}): [{string.Join(",", ValidHashes)}]";
		}
	}

	public class PatchAssemblyList {
		private string appliesTo;

		[XmlAttribute]
		public string AppliesTo {
			get {
				if (!appliesTo.EndsWith(".dll"))
					return appliesTo + ".dll";

				return appliesTo;
			}
			set { appliesTo = value; }
		}

		[XmlElement("Reference")]
		public List<PatchAssemblyReference> References = new List<PatchAssemblyReference>();
	}

	public class PatchAssemblyReference {
		[XmlAttribute]
		public string Name;

		[XmlAttribute]
		public string Version;

		public AssemblyNameReference AssemblyName {
			get {
				return new AssemblyNameReference(Name, new Version(Version ?? "0.0.0.0"));
			}
		}
	}

	public class PatchType {
		[XmlAttribute]
		public string Name;

		[XmlAttribute]
		public string Match;

		[XmlElement("Method")]
		public List<PatchMethod> Methods = new List<PatchMethod>();

		[XmlElement("Instruction", Type = typeof(PatchInstruction))]
		[XmlElement("Replace", Type = typeof(ReplaceOperation))]
		[XmlElement("ReplaceText", Type = typeof(TextReplaceOperation))]
		[XmlElement("Remove", Type = typeof(RemoveOperation))]
		public List<object> Instructions = new List<object>();
	}

	public class PatchMethod {
		[XmlAttribute]
		public string Name;

		[XmlAttribute]
		public string Match;

		[XmlAttribute]
		public int BeginAt = -1;

		[XmlAttribute]
		public int Offset;

		[XmlAttribute]
		public bool HasThis;

		[XmlElement("Instruction", Type = typeof(PatchInstruction))]
		[XmlElement("Replace", Type = typeof(ReplaceOperation))]
		[XmlElement("ReplaceText", Type = typeof(TextReplaceOperation))]
		[XmlElement("Remove", Type = typeof(RemoveOperation))]
		public List<object> Instructions = new List<object>();

		[XmlElement]
		public PatchExceptionHandler ExceptionHandler;
	}

	public class PatchInstruction {
		[XmlAttribute]
		public string OpCode;

		[XmlAttribute]
		public string Assembly;

		[XmlAttribute]
		public string Type;

		[XmlAttribute]
		public string Method;

		[XmlAttribute]
		public string Field;

		[XmlAttribute]
		public int Index = -1;

		[XmlAttribute]
		public int TargetIndex = -1;

		[XmlAttribute]
		public string Value;

		[XmlElement("Argument", Type = typeof(PatchInstructionParameter))]
		public List<PatchInstructionParameter> Parameters;

		public override string ToString() {
			return $"{OpCode} {Assembly} {Type} {Method} {Field} {Index} {TargetIndex} {Value}";
		}
	}

	public class PatchInstructionParameter {
		[XmlAttribute]
		public string Assembly;

		[XmlAttribute]
		public string Type;

		[XmlAttribute]
		public string Method;
	}

	public class ReplaceOperation {
		[XmlAttribute]
		public int Index = -1;

		[XmlElement("Instruction", Type = typeof(PatchInstruction))]
		public List<PatchInstruction> Replacements;
	}

	public class TextReplaceOperation {
		[XmlAttribute]
		public string TextToReplace;

		[XmlAttribute]
		public string ReplacementText;
	}

	public class RemoveOperation {
		[XmlAttribute]
		public int Index = -1;

		[XmlAttribute]
		public int Count = 0;
	}

	public class PatchExceptionHandler {
		[XmlAttribute]
		public int TryStart;

		[XmlAttribute]
		public int TryEnd;

		[XmlAttribute]
		public int HandlerStart;

		[XmlAttribute]
		public int HandlerEnd;
	}
}
