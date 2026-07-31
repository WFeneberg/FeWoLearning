<!-- Exercise 087 — AsyncUserCard component (advanced).
     Goal:   fetch a user from a simulated network call and render the result,
             moving through an "idle" -> "loading" -> "loaded" status so the
             DOM only shows the fetched user once the awaited promise settles.
     Drills: @vue/test-utils async DOM updates — a triggered async method does
             NOT update the DOM synchronously; the update only appears after
             the underlying promise resolves AND Vue flushes via
             `$nextTick()` (or an equivalent promise/timer flush). -->
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
  throw new Error("TODO: implement load");
}
</script>

<template>
  <div>
    <button type="button" @click="load">Load user</button>
    <p>TODO: render status/user</p>
  </div>
</template>
