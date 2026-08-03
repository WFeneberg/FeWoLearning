<!--
  Exercise 045 — AsyncComponentLoader component (reference solution).
-->
<script setup lang="ts">
import { defineAsyncComponent, defineComponent, h } from "vue";

// defineComponent (rather than a bare object literal) is what lets TypeScript
// infer `props` inside setup from the `props` declaration.
const RemoteGreeting = defineComponent({
  name: "RemoteGreeting",
  props: { name: { type: String, required: true } },
  setup(props) {
    return () => h("p", { class: "remote-greeting" }, `Hello, ${props.name}!`);
  },
});

function loadRemoteComponent() {
  return Promise.resolve(RemoteGreeting);
}

const AsyncGreeting = defineAsyncComponent({
  loader: loadRemoteComponent,
  loadingComponent: {
    setup() {
      return () => h("p", { class: "loading" }, "Loading...");
    },
  },
  delay: 0,
});

defineProps<{ name: string }>();
</script>

<template>
  <AsyncGreeting :name="name" />
</template>
