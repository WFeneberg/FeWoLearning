// Exercise 049 - async* generators (reference solution).

Stream<List<int>> paginate(List<int> items, int pageSize) async* {
  for (var i = 0; i < items.length; i += pageSize) {
    final end = (i + pageSize > items.length) ? items.length : i + pageSize;
    yield items.sublist(i, end);
  }
}
