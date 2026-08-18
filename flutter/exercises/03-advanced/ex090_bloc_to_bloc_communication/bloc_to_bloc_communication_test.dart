import 'package:test/test.dart';

import 'bloc_to_bloc_communication.dart';

void main() {
  test('logging out clears the cart', () async {
    final auth = AuthCubit();
    final cart = CartCubit(auth);

    cart.addItem('apple');
    await Future.delayed(Duration.zero);
    expect(cart.state, ['apple']);

    auth.logOut();
    await Future.delayed(Duration.zero);

    expect(cart.state, isEmpty);

    await cart.close();
    await auth.close();
  });

  test('close cancels the auth subscription cleanly', () async {
    final auth = AuthCubit();
    final cart = CartCubit(auth);

    await cart.close();
    expect(cart.isClosed, isTrue);

    // Should not throw even though the cubit is closed — the subscription
    // must have been cancelled inside close().
    auth.logOut();
    await Future.delayed(Duration.zero);

    await auth.close();
  });
}
