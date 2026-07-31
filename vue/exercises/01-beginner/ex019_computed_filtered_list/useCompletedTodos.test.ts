import { describe, expect, it } from "vitest";
import { useCompletedTodos } from "./useCompletedTodos";

describe("useCompletedTodos", () => {
  it("returns only the completed todos initially", () => {
    const { completedTodos } = useCompletedTodos([
      { id: 1, text: "Buy milk", done: true },
      { id: 2, text: "Clean house", done: false },
      { id: 3, text: "Write code", done: true },
    ]);

    expect(completedTodos.value).toEqual([
      { id: 1, text: "Buy milk", done: true },
      { id: 3, text: "Write code", done: true },
    ]);
  });

  it("updates when the source array is modified", () => {
    const { todos, completedTodos } = useCompletedTodos([
      { id: 1, text: "Buy milk", done: false },
    ]);

    expect(completedTodos.value).toEqual([]);

    todos.value.push({ id: 2, text: "Clean house", done: true });
    expect(completedTodos.value).toEqual([
      { id: 2, text: "Clean house", done: true },
    ]);

    todos.value[0].done = true;
    expect(completedTodos.value).toEqual([
      { id: 1, text: "Buy milk", done: true },
      { id: 2, text: "Clean house", done: true },
    ]);
  });

  it("removes a todo from completedTodos when marked undone", () => {
    const { todos, completedTodos } = useCompletedTodos([
      { id: 1, text: "Buy milk", done: true },
    ]);

    todos.value[0].done = false;
    expect(completedTodos.value).toEqual([]);
  });
});
