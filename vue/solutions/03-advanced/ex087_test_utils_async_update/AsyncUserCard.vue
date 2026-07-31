<!-- Exercise 087 — AsyncUserCard component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

interface User {
  id: number;
  name: string;
}

function fetchUser(id: number): Promise<User> {
  return new Promise((resolve) => {
    setTimeout(() => resolve({ id, name: "Ada Lovelace" }), 500);
  });
}

const status = ref<"idle" | "loading" | "loaded">("idle");
const user = ref<User | null>(null);

async function load(): Promise<void> {
  status.value = "loading";
  const result = await fetchUser(1);
  user.value = result;
  status.value = "loaded";
}
</script>

<template>
  <div>
    <button type="button" @click="load">Load user</button>
    <p v-if="status === 'idle'">No user loaded yet</p>
    <p v-else-if="status === 'loading'">Loading...</p>
    <p v-else>{{ user?.name }}</p>
  </div>
</template>
