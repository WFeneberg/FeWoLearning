// Exercise 084 - accessibility semantics (advanced).
//
// Goal:   Give an icon-only button a screen-reader-friendly semantic label.
// Drills: Semantics, Icon.semanticLabel, merged semantics nodes.
// Passes: when the widget is discoverable via find.bySemanticsLabel and its
//         merged semantics node reports the label and a tap action.

import 'package:flutter/material.dart';

class LabeledIconButton extends StatelessWidget {
  const LabeledIconButton({
    super.key,
    required this.icon,
    required this.label,
    required this.onPressed,
  });

  final IconData icon;
  final String label;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
