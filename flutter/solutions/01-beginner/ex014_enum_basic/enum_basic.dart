// Exercise 014 - enums (reference solution).

enum Direction { north, east, south, west }

Direction opposite(Direction direction) => switch (direction) {
      Direction.north => Direction.south,
      Direction.south => Direction.north,
      Direction.east => Direction.west,
      Direction.west => Direction.east,
    };
