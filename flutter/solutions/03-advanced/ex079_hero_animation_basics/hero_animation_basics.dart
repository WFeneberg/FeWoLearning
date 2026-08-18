// Exercise 079 - Hero shared element transitions (reference solution).

import 'package:flutter/material.dart';

const heroTag = 'logo';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: GestureDetector(
        onTap: () => Navigator.of(context).push(
          MaterialPageRoute(builder: (_) => const DetailPage()),
        ),
        child: const Hero(tag: heroTag, child: FlutterLogo(size: 50)),
      ),
    );
  }
}

class DetailPage extends StatelessWidget {
  const DetailPage({super.key});

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      body: Hero(tag: heroTag, child: FlutterLogo(size: 200)),
    );
  }
}

Widget buildHeroDemoApp() {
  return const MaterialApp(home: HomePage());
}
