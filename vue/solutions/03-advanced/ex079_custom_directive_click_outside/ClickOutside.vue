<!-- Exercise 079 — ClickOutside component (reference solution). -->
<script setup lang="ts">
import type { Directive } from "vue";

const emit = defineEmits<{ outside: [] }>();

interface ClickOutsideElement extends HTMLElement {
  __clickOutsideListener__?: (event: MouseEvent) => void;
}

const vClickOutside: Directive<HTMLElement, () => void> = {
  mounted(el: ClickOutsideElement, binding) {
    const listener = (event: MouseEvent) => {
      const target = event.target as Node | null;
      if (target && !el.contains(target)) {
        binding.value();
      }
    };
    el.__clickOutsideListener__ = listener;
    document.addEventListener("click", listener);
  },
  unmounted(el: ClickOutsideElement) {
    if (el.__clickOutsideListener__) {
      document.removeEventListener("click", el.__clickOutsideListener__);
      delete el.__clickOutsideListener__;
    }
  },
};
</script>

<template>
  <div>
    <div data-testid="box" v-click-outside="() => emit('outside')">Inside</div>
    <button type="button" data-testid="outside">Outside</button>
  </div>
</template>
