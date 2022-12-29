// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1854:Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method", Justification = "Throwing the exception is gonna be slower than searching twice.", Scope = "member", Target = "~M:RetroSpy.ViewWindow.#ctor(RetroSpy.SetupWindow,RetroSpy.Skin,RetroSpy.Background,RetroSpy.Readers.IControllerReader,System.Boolean)")]
