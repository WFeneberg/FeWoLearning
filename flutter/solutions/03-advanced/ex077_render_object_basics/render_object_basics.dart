// Exercise 077 - custom RenderObject basics (reference solution).

import 'package:flutter/rendering.dart';
import 'package:flutter/widgets.dart';

class FixedSquare extends LeafRenderObjectWidget {
  const FixedSquare({super.key, required this.size});

  final double size;

  @override
  RenderObject createRenderObject(BuildContext context) {
    return RenderFixedSquare(size);
  }

  @override
  void updateRenderObject(
      BuildContext context, covariant RenderFixedSquare renderObject) {
    renderObject.squareSize = size;
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
    size = constraints.constrain(Size.square(_size));
  }
}
