// Exercise 077 - custom RenderObject basics (advanced).
//
// Goal:   Implement a leaf RenderObject that always sizes itself to a fixed
//         square, clamped to the incoming BoxConstraints.
// Drills: RenderBox, performLayout, BoxConstraints.constrain,
//         LeafRenderObjectWidget.
// Passes: when the rendered size is the requested square size whenever the
//         constraints allow it, and clamped down when they don't.

import 'package:flutter/rendering.dart';
import 'package:flutter/widgets.dart';

class FixedSquare extends LeafRenderObjectWidget {
  const FixedSquare({super.key, required this.size});

  final double size;

  @override
  RenderObject createRenderObject(BuildContext context) {
    throw UnimplementedError('TODO');
  }

  @override
  void updateRenderObject(
      BuildContext context, covariant RenderFixedSquare renderObject) {
    throw UnimplementedError('TODO');
  }
}

class RenderFixedSquare extends RenderBox {
  RenderFixedSquare(this._size);

  double _size;
  set squareSize(double value) {
    if (_size == value) return;
    _size = value;
    markNeedsLayout();
  }

  @override
  void performLayout() {
    throw UnimplementedError('TODO');
  }
}
