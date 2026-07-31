<!--
  Exercise 045 — AsyncComponentLoader component (reference solution).
-->
<script setup lang="ts">
import { defineAsyncComponent, h } from "vue";

function loadRemoteComponent() {
  return Promise.resolve({
    name: "RemoteGreeting",
    props: { name: { type: String, required: true } },
    setup(props: { name: string }) {
      return () => h("p", { class: "remote-greeting" }, `Hello, ${props.name}!`);
    },
  });
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
