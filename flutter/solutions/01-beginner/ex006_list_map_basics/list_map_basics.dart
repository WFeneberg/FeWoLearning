// Exercise 006 - List/Map literals & indexing (reference solution).

String secondItem(List<String> items) => items[1];

int priceFor(Map<String, int> prices, String item) => prices[item] ?? 0;
