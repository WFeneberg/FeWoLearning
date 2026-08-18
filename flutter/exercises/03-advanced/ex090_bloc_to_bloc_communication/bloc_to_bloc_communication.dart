// Exercise 090 - Bloc-to-Bloc communication (advanced).
//
// Goal:   Make a CartCubit clear its items whenever an AuthCubit it depends
//         on transitions to logged-out.
// Drills: Cubit, cross-cubit subscriptions, StreamSubscription lifecycle.
// Passes: when logging out via AuthCubit empties CartCubit's items, and the
//         subscription is cancelled in CartCubit.close().

import 'dart:async';

import 'package:flutter_bloc/flutter_bloc.dart';

class AuthCubit extends Cubit<bool> {
  AuthCubit() : super(true);

  void logOut() => emit(false);
  void logIn() => emit(true);
}

class CartCubit extends Cubit<List<String>> {
  CartCubit(this._authCubit) : super([]) {
    throw UnimplementedError('TODO');
  }

  final AuthCubit _authCubit;
  StreamSubscription<bool>? _authSubscription;

  void addItem(String item) => emit([...state, item]);

  @override
  Future<void> close() {
    throw UnimplementedError('TODO');
  }
}
