// Exercise 073 — useUserStore composable (reference solution).
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
  const user = ref<User | null>(null) as Ref<User | null>;
  const loading = ref(false);
  const error = ref<string | null>(null);

  async function loadUser(fetchUser: UserFetcher): Promise<void> {
    loading.value = true;
    error.value = null;
    try {
      const result = await fetchUser();
      user.value = result;
    } catch (err) {
      error.value = err instanceof Error ? err.message : String(err);
      user.value = null;
    } finally {
      loading.value = false;
    }
  }

  return { user, loading, error, loadUser };
}
