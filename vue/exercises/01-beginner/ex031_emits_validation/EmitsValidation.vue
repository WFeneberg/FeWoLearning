<!--
  Exercise 031 — EmitsValidation component (beginner).
  Goal:   declare a `change` emit with a validator that rejects non-numeric payloads.
  Drills: defineEmits object syntax, emits payload validation.
-->
<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "EmitsValidation",
  emits: {
    // TODO: return true only when `payload` is a number.
    change: (_payload: unknown): boolean => {
      throw new Error("TODO: implement the change validator");
    },
  },
  methods: {
    emitValid(): void {
      this.$emit("change", 42);
    },
    emitInvalid(): void {
      this.$emit("change", "not-a-number");
    },
  },
});
</script>

<template>
  <div>
    <button data-testid="valid" @click="emitValid">Emit valid</button>
    <button data-testid="invalid" @click="emitInvalid">Emit invalid</button>
  </div>
</template>
