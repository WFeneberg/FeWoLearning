// Companion to Ex014_ViewLocatorContext.cs - the context-specific view variants for
// Ex014_ProbeViewModel. Measured convention: a model FeWoLearning.Caliburn.Exercises.Beginner
// .Ex014_ProbeViewModel resolved with context "Edit" or "Detail" is expected at
// FeWoLearning.Caliburn.Exercises.Beginner.Ex014_Probe.Edit / .Detail respectively - the
// "ViewModel" suffix is dropped from the model's own type name ("Ex014_ProbeViewModel" ->
// "Ex014_Probe"), and that becomes a NAMESPACE holding one type per context, named after the
// context string itself. This namespace exists only in solutions/ - it is the exercise's answer.

using System.Windows.Controls;

namespace FeWoLearning.Caliburn.Exercises.Beginner.Ex014_Probe;

public class Edit : UserControl;

public class Detail : UserControl;
