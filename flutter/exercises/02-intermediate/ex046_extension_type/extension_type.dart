// Exercise 046 - Dart 3 extension types (intermediate).
//
// Goal:   Wrap a raw int as a zero-cost UserId type with a formatted label.
// Drills: `extension type`, representation field, methods on wrapper types.
// Passes: when UserId(...).formatted() prefixes the wrapped value, and the
//         underlying `value` stays accessible.

extension type UserId(int value) {
  String formatted() {
    throw UnimplementedError('TODO');
  }
}
