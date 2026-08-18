// Exercise 080 - CustomScrollView & slivers (reference solution).

import 'package:flutter/material.dart';

Widget buildItemListApp(List<String> items) {
  return MaterialApp(
    home: Scaffold(
      body: CustomScrollView(
        slivers: [
          const SliverAppBar(title: Text('Items'), pinned: true),
          SliverList(
            delegate: SliverChildBuilderDelegate(
              (context, index) => ListTile(title: Text(items[index])),
              childCount: items.length,
            ),
          ),
        ],
      ),
    ),
  );
}
