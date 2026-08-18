// Exercise 056 - widget finders (intermediate).
//
// Goal:   Build a like button that toggles between an outlined and a filled
//         heart icon, and between "0 likes" and "1 like".
// Drills: find.text/find.byType/find.byIcon, tester.tap.
// Passes: when tapping the icon button toggles both the icon and the label
//         text, and tapping again reverts it.

import 'package:flutter/material.dart';

class LikeButton extends StatefulWidget {
  const LikeButton({super.key});

  @override
  State<LikeButton> createState() => _LikeButtonState();
}

class _LikeButtonState extends State<LikeButton> {
  bool _liked = false;

  void _toggle() {
    throw UnimplementedError('TODO');
  }

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
