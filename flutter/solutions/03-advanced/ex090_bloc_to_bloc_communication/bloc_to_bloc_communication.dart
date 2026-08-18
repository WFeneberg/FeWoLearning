// Exercise 090 - Bloc-to-Bloc communication (reference solution).

import 'dart:async';

import 'package:flutter_bloc/flutter_bloc.dart';

class AuthCubit extends Cubit<bool> {
  AuthCubit() : super(true);

  void logOut() => emit(false);
  void logIn() => emit(true);
}

class CartCubit extends Cubit<List<String>> {
  CartCubit(this._authCubit) : super([]) {
    _authSubscription = _authCubit.stream.listen((loggedIn) {
      if (!loggedIn) emit([]);
    });
  }

  final AuthCubit _authCubit;
  StreamSubscription<bool>? _authSubscription;

  void addItem(String item) => emit([...state, item]);

  @override
  Future<void> close() async {
    await _authSubscription?.cancel();
    return super.close();
  }
}
