// Exercise 005 - switch expressions & records (reference solution).

String classify((int, int) point) => switch (point) {
      (0, 0) => 'origin',
      (var x, 0) when x != 0 => 'on-axis',
      (0, var y) when y != 0 => 'on-axis',
      _ => 'quadrant',
    };
