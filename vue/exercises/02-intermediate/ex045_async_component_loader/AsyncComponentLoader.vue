<!--
  Exercise 045 — AsyncComponentLoader component (intermediate).
  Goal:   load a component lazily with `defineAsyncComponent(() => Promise)` and
          render its content once the promise resolves.
  Drills: defineAsyncComponent, async component resolution, suspense-free loading
          states (loading / loaded).
-->
<script setup lang="ts">
import { defineAsyncComponent, h } from "vue";

// The "remote" component we are lazily loading. In a real app this would be
// a dynamic `import("./SomeComponent.vue")`; here we inline it so the
// exercise has no extra files, but it is still wrapped in a Promise to
// simulate an async chunk load.
function loadRemoteComponent() {
  return Promise.resolve({
    name: "RemoteGreeting",
    props: { name: { type: String, required: true } },
    setup(props: { name: string }) {
      return () => h("p", { class: "remote-greeting" }, `Hello, ${props.name}!`);
    },
  });
}

// TODO: build the async component with defineAsyncComponent, passing a
// loadingComponent + delay so a loading state briefly shows before the
// remote component resolves.
const AsyncGreeting: ReturnType<typeof defineAsyncComponent> = (() => {
  throw new Error("TODO: implement AsyncComponentLoader");
})();

defineProps<{ name: string }>();
</script>

<template>
  <AsyncGreeting :name="name" />
</template>
