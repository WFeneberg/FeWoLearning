// Exercise 056 - widget finders (reference solution).

import 'package:flutter/material.dart';

class LikeButton extends StatefulWidget {
  const LikeButton({super.key});

  @override
  State<LikeButton> createState() => _LikeButtonState();
}

class _LikeButtonState extends State<LikeButton> {
  bool _liked = false;

  void _toggle() {
    setState(() {
      _liked = !_liked;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        IconButton(
          icon: Icon(_liked ? Icons.favorite : Icons.favorite_border),
          onPressed: _toggle,
        ),
        Text(_liked ? '1 like' : '0 likes'),
      ],
    );
  }
}
