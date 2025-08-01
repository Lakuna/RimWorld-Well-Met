using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace Lakuna.WellMet.Utility {
	/// <summary>
	/// A static utility class that contains static utility methods for performing common patching operations.
	/// </summary>
	internal static class PatchUtility {
		/// <summary>
		/// `string.Empty`.
		/// </summary>
		private static readonly FieldInfo EmptyStringField = AccessTools.Field(typeof(string), nameof(string.Empty));

		/// <summary>
		/// `KnowledgeUtility.IsInformationKnownFor` given a `Pawn` as an argument.
		/// </summary>
		private static readonly MethodInfo IsInformationKnownForPawnMethod = AccessTools.Method(typeof(KnowledgeUtility), nameof(KnowledgeUtility.IsInformationKnownFor), new Type[] { typeof(InformationCategory), typeof(Pawn) });

		/// <summary>
		/// `KnowledgeUtility.IsInformationKnownFor` given a `Faction` as an argument.
		/// </summary>
		private static readonly MethodInfo IsInformationKnownForFactionMethod = AccessTools.Method(typeof(KnowledgeUtility), nameof(KnowledgeUtility.IsInformationKnownFor), new Type[] { typeof(InformationCategory), typeof(Faction) });

		/// <summary>
		/// AND the value on top of the stack with whether the given information category is known for the "given" pawn.
		/// </summary>
		/// <param name="informationCategory">The information category.</param>
		/// <param name="getPawnInstructions">The instructions to execute to load the pawn onto the stack.</param>
		/// <returns>The instructions that will perform the AND.</returns>
		internal static IEnumerable<CodeInstruction> AndPawnKnown(InformationCategory informationCategory, IEnumerable<CodeInstruction> getPawnInstructions) {
			foreach (CodeInstruction instruction in AndKnown(IsInformationKnownForPawnMethod, informationCategory, getPawnInstructions)) {
				yield return instruction;
			}
		}

		/// <summary>
		/// AND the value on top of the stack with whether the given information category is known for the "given" faction.
		/// </summary>
		/// <param name="informationCategory">The information category.</param>
		/// <param name="getFactionInstructions">The instructions to execute to load the faction onto the stack.</param>
		/// <returns>The instructions that will perform the AND.</returns>
		internal static IEnumerable<CodeInstruction> AndFactionKnown(InformationCategory informationCategory, IEnumerable<CodeInstruction> getFactionInstructions) {
			foreach (CodeInstruction instruction in AndKnown(IsInformationKnownForFactionMethod, informationCategory, getFactionInstructions)) {
				yield return instruction;
			}
		}

		/// <summary>
		/// AND the value on top of the stack with whether the given information category is known for the "given" pawn or faction.
		/// </summary>
		/// <param name="isInformationKnownForMethod">The method to call to check knowledge.</param>
		/// <param name="informationCategory">The information category.</param>
		/// <param name="getInstructions">The instructions to execute to load the pawn or faction onto the stack.</param>
		/// <returns>The instructions that will perform the AND.</returns>
		private static IEnumerable<CodeInstruction> AndKnown(MethodInfo isInformationKnownForMethod, InformationCategory informationCategory, IEnumerable<CodeInstruction> getInstructions) {
			// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
			yield return new CodeInstruction(OpCodes.Ldc_I4, (int)informationCategory);
			foreach (CodeInstruction instruction in getInstructions) {
				yield return new CodeInstruction(instruction);
			}

			// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
			yield return new CodeInstruction(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.

			// AND the return value with the value that was originally on top of the stack.
			yield return new CodeInstruction(OpCodes.And);
		}

		/// <summary>
		/// OR the value on top of the stack with whether the given information category is not known for the "given" pawn.
		/// </summary>
		/// <param name="informationCategory">The information category.</param>
		/// <param name="getPawnInstructions">The instructions to execute to load the pawn onto the stack.</param>
		/// <returns>The instructions that will perform the OR.</returns>
		internal static IEnumerable<CodeInstruction> OrPawnNotKnown(InformationCategory informationCategory, IEnumerable<CodeInstruction> getPawnInstructions) {
			foreach (CodeInstruction instruction in OrNotKnown(IsInformationKnownForPawnMethod, informationCategory, getPawnInstructions)) {
				yield return instruction;
			}
		}

		/// <summary>
		/// OR the value on top of the stack with whether the given information category is not known for the "given" faction.
		/// </summary>
		/// <param name="informationCategory">The information category.</param>
		/// <param name="getFactionInstructions">The instructions to execute to load the faction onto the stack.</param>
		/// <returns>The instructions that will perform the OR.</returns>
		internal static IEnumerable<CodeInstruction> OrFactionNotKnown(InformationCategory informationCategory, IEnumerable<CodeInstruction> getFactionInstructions) {
			foreach (CodeInstruction instruction in OrNotKnown(IsInformationKnownForFactionMethod, informationCategory, getFactionInstructions)) {
				yield return instruction;
			}
		}

		/// <summary>
		/// OR the value on top of the stack with whether the given information category is not known for the "given" pawn or faction.
		/// </summary>
		/// <param name="isInformationKnownForMethod">The method to call to check knowledge.</param>
		/// <param name="informationCategory">The information category.</param>
		/// <param name="getInstructions">The instructions to execute to load the pawn or faction onto the stack.</param>
		/// <returns>The instructions that will perform the OR.</returns>
		private static IEnumerable<CodeInstruction> OrNotKnown(MethodInfo isInformationKnownForMethod, InformationCategory informationCategory, IEnumerable<CodeInstruction> getInstructions) {
			// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
			yield return new CodeInstruction(OpCodes.Ldc_I4, (int)informationCategory);
			foreach (CodeInstruction instruction in getInstructions) {
				yield return new CodeInstruction(instruction);
			}

			// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
			yield return new CodeInstruction(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.

			// Load `0` (`false`) onto the stack, then check if it's equal to the return value (thus negating it).
			yield return new CodeInstruction(OpCodes.Ldc_I4_0);
			yield return new CodeInstruction(OpCodes.Ceq);

			// OR the negated return value with the value that was originally on top of the stack.
			yield return new CodeInstruction(OpCodes.Or);
		}

		/// <summary>
		/// Replace the value on top of the stack if the given information category isn't known for the "given" pawn.
		/// </summary>
		/// <param name="informationCategory">The information category that must be known to skip the replacement.</param>
		/// <param name="getPawnInstructions">The instructions to execute to load the pawn onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the top of the stack with if the information category isn't known for the "given" pawn.</param>
		/// <returns>The instructions that will perform the conditional replacement.</returns>
		internal static IEnumerable<CodeInstruction> ReplaceIfPawnNotKnown(InformationCategory informationCategory, IEnumerable<CodeInstruction> getPawnInstructions, ILGenerator generator, object value = null) {
			foreach (CodeInstruction instruction in ReplaceIfNotKnown(IsInformationKnownForPawnMethod, informationCategory, getPawnInstructions, generator, value)) {
				yield return instruction;
			}
		}

		/// <summary>
		/// Replace the value on top of the stack if the given information category isn't known for the "given" faction.
		/// </summary>
		/// <param name="informationCategory">The information category that must be known to skip the replacement.</param>
		/// <param name="getFactionInstructions">The instructions to execute to load the faction onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the top of the stack with if the information category isn't known for the "given" faction.</param>
		/// <returns>The instructions that will perform the conditional replacement.</returns>
		internal static IEnumerable<CodeInstruction> ReplaceIfFactionNotKnown(InformationCategory informationCategory, IEnumerable<CodeInstruction> getFactionInstructions, ILGenerator generator, object value = null) {
			foreach (CodeInstruction instruction in ReplaceIfNotKnown(IsInformationKnownForFactionMethod, informationCategory, getFactionInstructions, generator, value)) {
				yield return instruction;
			}
		}

		/// <summary>
		/// Replace the value on top of the stack if the given information category isn't known for the "given" pawn or faction.
		/// </summary>
		/// <param name="isInformationKnownForMethod">The method to call to check knowledge.</param>
		/// <param name="informationCategory">The information category that must be known to skip the replacement.</param>
		/// <param name="getInstructions">The instructions to execute to load the pawn or faction onto the stack.</param>
		/// <param name="generator">The code generator.</param>
		/// <param name="value">The value to replace the top of the stack with if the information category isn't known for the "given" pawn or faction.</param>
		/// <returns>The instructions that will perform the conditional replacement.</returns>
		private static IEnumerable<CodeInstruction> ReplaceIfNotKnown(MethodInfo isInformationKnownForMethod, InformationCategory informationCategory, IEnumerable<CodeInstruction> getInstructions, ILGenerator generator, object value = null) {
			// Load the arguments for `KnowledgeUtility.IsInformationKnownFor` onto the stack.
			yield return new CodeInstruction(OpCodes.Ldc_I4, (int)informationCategory);
			foreach (CodeInstruction instruction in getInstructions) {
				yield return new CodeInstruction(instruction);
			}

			// Call `KnowledgeUtility.IsInformationKnownFor`, leaving the return value on top of the stack.
			yield return new CodeInstruction(OpCodes.Call, isInformationKnownForMethod); // Remove the arguments from the stack and add the return value.

			// If the value on top of the stack is `true` (the given information is known), don't replace the value.
			Label dontReplaceLabel = generator.DefineLabel();
			yield return new CodeInstruction(OpCodes.Brtrue_S, dontReplaceLabel); // Remove the return value of `KnowledgeUtility.IsInformationKnownFor` from the stack, leaving the value that might be replaced on top.

			// This section is skipped unless the given information isn't known.
			yield return new CodeInstruction(OpCodes.Pop); // Remove the value that is being replaced from the stack.
			yield return LoadValue(value);

			// Jump here when the given information is known, skipping the code that replaces the original value (thus not modifying the stack).
			CodeInstruction dontReplaceTarget = new CodeInstruction(OpCodes.Nop);
			dontReplaceTarget.labels.Add(dontReplaceLabel);
			yield return dontReplaceTarget;
		}

		/// <summary>
		/// Load the given value onto the stack.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The instruction that will load the given value onto the stack.</returns>
		private static CodeInstruction LoadValue(object value) {
			if (value is int intValue) {
				switch (intValue) {
					case -1:
						return new CodeInstruction(OpCodes.Ldc_I4_M1);
					case 0:
						return new CodeInstruction(OpCodes.Ldc_I4_0);
					case 1:
						return new CodeInstruction(OpCodes.Ldc_I4_1);
					case 2:
						return new CodeInstruction(OpCodes.Ldc_I4_2);
					case 3:
						return new CodeInstruction(OpCodes.Ldc_I4_3);
					case 4:
						return new CodeInstruction(OpCodes.Ldc_I4_4);
					case 5:
						return new CodeInstruction(OpCodes.Ldc_I4_5);
					case 6:
						return new CodeInstruction(OpCodes.Ldc_I4_6);
					case 7:
						return new CodeInstruction(OpCodes.Ldc_I4_7);
					case 8:
						return new CodeInstruction(OpCodes.Ldc_I4_8);
					default:
						return new CodeInstruction(OpCodes.Ldc_I4, intValue);
				}
			}

			return value == null ? new CodeInstruction(OpCodes.Ldnull)
				: value is FieldInfo fieldInfo ? fieldInfo.IsStatic
					? new CodeInstruction(OpCodes.Ldsfld, fieldInfo)
					: throw new ArgumentException("A non-static field was passed to " + nameof(LoadValue) + ".")
				: value is ConstructorInfo constructorInfo ? new CodeInstruction(OpCodes.Newobj, constructorInfo)
				: value is long longValue ? new CodeInstruction(OpCodes.Ldc_I8, longValue)
				: value is byte byteValue ? new CodeInstruction(OpCodes.Ldc_I4_S, byteValue)
				: value is bool boolValue ? boolValue
					? new CodeInstruction(OpCodes.Ldc_I4_1)
					: new CodeInstruction(OpCodes.Ldc_I4_0)
				: value is double doubleValue ? new CodeInstruction(OpCodes.Ldc_R8, doubleValue)
				: value is float floatValue ? new CodeInstruction(OpCodes.Ldc_R4, floatValue)
				: value is string stringValue ? stringValue.NullOrEmpty()
					? new CodeInstruction(OpCodes.Ldsfld, EmptyStringField)
					: new CodeInstruction(OpCodes.Ldstr, stringValue)
				: new CodeInstruction(OpCodes.Ldc_I4, (int)value);
		}
	}
}
