// Exercise 019 — useCompletedTodos composable (beginner).
// Goal:   a computed that filters an array of todos by `done === true`.
// Drills: ref (array), computed, derived reactive state.
import { computed, ref, type ComputedRef, type Ref } from "vue";

export interface Todo {
  id: number;
  text: string;
  done: boolean;
}

export interface TodoList {
  todos: Ref<Todo[]>;
  completedTodos: ComputedRef<Todo[]>;
}

export function useCompletedTodos(initial: Todo[] = []): TodoList {
  throw new Error("TODO: implement useCompletedTodos");
}
