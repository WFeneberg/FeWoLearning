// Exercise 044 - manual fromJson/toJson model mapping (reference solution).

class Product {
  final String name;
  final double price;

  Product(this.name, this.price);

  factory Product.fromJson(Map<String, Object?> json) => Product(
        json['name'] as String,
        (json['price'] as num).toDouble(),
      );

  Map<String, Object?> toJson() => {'name': name, 'price': price};
}
