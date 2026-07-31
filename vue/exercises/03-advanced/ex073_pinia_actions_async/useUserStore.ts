// Exercise 073 — useUserStore composable (advanced).
// Goal:   a Pinia-style store with an async action `loadUser()` that awaits
//         an injected fetcher, sets a `user` state field with the resolved
//         value, and tracks `loading`/`error` state around the call.
// Drills: async actions, injected dependencies, loading/error state machines.
import { ref, type Ref } from "vue";

export interface User {
  id: number;
  name: string;
  email: string;
}

export type UserFetcher = () => Promise<User>;

export interface UserStore {
  user: Ref<User | null>;
  loading: Ref<boolean>;
  error: Ref<string | null>;
  loadUser: (fetchUser: UserFetcher) => Promise<void>;
}

export function useUserStore(): UserStore {
  throw new Error("TODO: implement useUserStore");
}
