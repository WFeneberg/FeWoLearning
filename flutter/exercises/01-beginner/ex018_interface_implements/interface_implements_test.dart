import 'package:test/test.dart';

import 'interface_implements.dart';

void main() {
  test('Greeter greets casually', () {
    expect(Greeter().greet('Ada'), 'Hello, Ada!');
  });

  test("FormalGreeter greets formally, ignoring Greeter's body", () {
    expect(FormalGreeter().greet('Ada'), 'Good day, Ada.');
  });

  test('FormalGreeter is usable wherever a Greeter is expected', () {
    Greeter greeter = FormalGreeter();
    expect(greeter.greet('Bo'), 'Good day, Bo.');
  });
}
