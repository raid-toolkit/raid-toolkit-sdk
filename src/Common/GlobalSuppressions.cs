// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

#if NET5_0_OR_GREATER
[assembly:SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "Not supported in net472")]
#else
[assembly:SuppressMessage("Style", "IDE0057:Use range operator", Justification = "Not supported in net472")]
#endif
