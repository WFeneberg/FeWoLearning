<!--
Exercise 044 — SuspenseAsyncComponent (reference solution).
-->
<script setup lang="ts">
export interface Profile {
  name: string;
  role: string;
}

// Simulates an async data source (e.g. a network request) without any
// timers or real I/O — it simply resolves on a later microtask so a
// parent <Suspense> has a chance to show its fallback first.
function fetchProfile(): Promise<Profile> {
  return Promise.resolve().then(() =>
    Promise.resolve({ name: "Ada Lovelace", role: "Engineer" }),
  );
}

// A top-level `await` inside <script setup> makes this an async component:
// Vue suspends rendering until the promise settles, letting a wrapping
// <Suspense> show its fallback slot in the meantime.
const profile = await fetchProfile();
</script>

<template>
  <div>
    <p data-testid="name">{{ profile.name }}</p>
    <p data-testid="role">{{ profile.role }}</p>
  </div>
</template>
