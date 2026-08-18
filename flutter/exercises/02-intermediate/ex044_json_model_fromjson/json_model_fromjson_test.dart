import 'package:test/test.dart';

import 'json_model_fromjson.dart';

void main() {
  test('fromJson reads name and price', () {
    final product = Product.fromJson({'name': 'Widget', 'price': 9.99});
    expect(product.name, 'Widget');
    expect(product.price, 9.99);
  });

  test('fromJson coerces an integer price to double', () {
    final product = Product.fromJson({'name': 'Gadget', 'price': 5});
    expect(product.price, 5.0);
  });

  test('toJson round-trips through fromJson', () {
    final product = Product('Gizmo', 3.5);
    final restored = Product.fromJson(product.toJson());
    expect(restored.name, 'Gizmo');
    expect(restored.price, 3.5);
  });
}
