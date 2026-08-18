// Exercise 060 - FutureBuilder basics (reference solution).

import 'package:flutter/material.dart';

class UserNameLoader extends StatelessWidget {
  const UserNameLoader({super.key, required this.nameFuture});

  final Future<String> nameFuture;

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<String>(
      future: nameFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return const Text('Loading...');
        }
        if (snapshot.hasError) {
          return const Text('Error');
        }
        return Text(snapshot.data!);
      },
    );
  }
}
