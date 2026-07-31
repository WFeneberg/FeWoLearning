<!--
Exercise 044 — SuspenseAsyncComponent (intermediate).
Goal:   a `<script setup>` component that performs a real top-level `await`
        so it becomes an async component, meant to be mounted inside a
        parent `<Suspense>` boundary that shows a fallback while pending.
Drills: async `<script setup>`, top-level await, Suspense fallback timing.
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

// TODO: replace this with a real top-level `await fetchProfile()` call.
// A top-level `await` inside <script setup> is what turns this component
// into an async component that a parent <Suspense> can wait on.
throw new Error(
  "TODO: implement SuspenseAsyncComponent with a top-level await",
);

const profile = { name: "", role: "" } as Profile;
</script>

<template>
  <div>
    <p data-testid="name">{{ profile.name }}</p>
    <p data-testid="role">{{ profile.role }}</p>
  </div>
</template>
