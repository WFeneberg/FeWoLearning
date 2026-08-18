// Exercise 002 - string templates & multiline strings (reference solution).

String greet(String name, int age) => 'Hello, $name! You are $age years old.';

String receiptLine(String item, int quantity, double unitPrice) {
  final formattedPrice = unitPrice.toStringAsFixed(2);
  return '''
${quantity}x $item
@ $formattedPrice each''';
}
