// Exercise 064 - widget keys & identity (reference solution).

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
  return Column(
    children: [
      for (final label in labels) CheckableItem(key: ValueKey(label), label: label),
    ],
  );
}
