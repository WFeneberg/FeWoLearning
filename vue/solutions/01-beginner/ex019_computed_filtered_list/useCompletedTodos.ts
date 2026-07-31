// Exercise 019 — useCompletedTodos composable (reference solution).
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
  const todos = ref<Todo[]>(initial) as Ref<Todo[]>;
  const completedTodos = computed(() => todos.value.filter((todo) => todo.done === true));

  return {
    todos,
    completedTodos,
  };
}
