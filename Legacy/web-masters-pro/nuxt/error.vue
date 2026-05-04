<template lang="pug">
  NuxtLayout
      section.error.pd-header.screen
          .container
              div(v-if="error.statusCode === 404")
                  h1 404 Not Found
              div(v-else-if="error.statusCode === 403")
                  h1 403 Forbidden
              div(v-else)
                  h1 Error
                  pre {{ error }}
</template>

<script setup>
const error = useError();
</script>