// Exercise 061 - StreamBuilder basics (reference solution).

import 'package:flutter/material.dart';

class LiveScoreDisplay extends StatelessWidget {
  const LiveScoreDisplay({super.key, required this.scoreStream});

  final Stream<int> scoreStream;

  @override
  Widget build(BuildContext context) {
    return StreamBuilder<int>(
      stream: scoreStream,
      initialData: 0,
      builder: (context, snapshot) {
        return Text('Score: ${snapshot.data}');
      },
    );
  }
}
