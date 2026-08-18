// Exercise 064 - widget keys & identity (intermediate).
//
// Goal:   Build the list of checkable items so each keeps its own checked
//         state tied to its label, even after the list is reordered.
// Drills: widget keys, ValueKey, element identity across rebuilds.
// Passes: when buildList attaches a ValueKey(label) to each CheckableItem,
//         so checking one item and then reordering the list keeps that
//         item's checked state attached to its label, not its position.

import 'package:flutter/material.dart';

class CheckableItem extends StatefulWidget {
  const CheckableItem({super.key, required this.label});

  final String label;

  @override
  State<CheckableItem> createState() => _CheckableItemState();
}

class _CheckableItemState extends State<CheckableItem> {
  bool _checked = false;

  @override
  Widget build(BuildContext context) {
    return CheckboxListTile(
      value: _checked,
      onChanged: (value) => setState(() => _checked = value ?? false),
      title: Text(widget.label),
    );
  }
}

Widget buildList(List<String> labels) {
  throw UnimplementedError('TODO');
}
