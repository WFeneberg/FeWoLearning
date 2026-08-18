// Exercise 060 - FutureBuilder basics (intermediate).
//
// Goal:   Build a widget that shows "Loading...", the resolved name, or an
//         error message, driven by an already-created Future.
// Drills: FutureBuilder, AsyncSnapshot, ConnectionState.
// Passes: when UserNameLoader shows "Loading..." while the future is
//         pending, the resolved name once it completes, and "Error" if it
//         completes with an error.

import 'package:flutter/material.dart';

class UserNameLoader extends StatelessWidget {
  const UserNameLoader({super.key, required this.nameFuture});

  final Future<String> nameFuture;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
