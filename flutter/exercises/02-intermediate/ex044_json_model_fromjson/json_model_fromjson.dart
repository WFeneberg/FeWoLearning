// Exercise 044 - manual fromJson/toJson model mapping (intermediate).
//
// Goal:   Give Product manual JSON (de)serialization without a codegen tool.
// Drills: factory constructors, manual fromJson/toJson, numeric coercion.
// Passes: when Product.fromJson() reads name/price (coercing an int price to
//         double) and toJson() round-trips back through fromJson().

class Product {
  final String name;
  final double price;

  Product(this.name, this.price);

  factory Product.fromJson(Map<String, Object?> json) {
    throw UnimplementedError('TODO');
  }

  Map<String, Object?> toJson() {
    throw UnimplementedError('TODO');
  }
}
