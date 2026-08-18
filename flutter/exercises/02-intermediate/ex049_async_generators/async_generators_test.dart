import 'package:test/test.dart';

import 'async_generators.dart';

void main() {
  test('paginate splits items into pages of the given size', () async {
    final pages = await paginate([1, 2, 3, 4, 5], 2).toList();
    expect(pages, [
      [1, 2],
      [3, 4],
      [5],
    ]);
  });

  test('paginate on an empty list yields no pages', () async {
    final pages = await paginate(<int>[], 2).toList();
    expect(pages, <List<int>>[]);
  });

  test('paginate with a page size covering everything yields one page', () async {
    final pages = await paginate([1, 2, 3], 10).toList();
    expect(pages, [
      [1, 2, 3],
    ]);
  });
}
