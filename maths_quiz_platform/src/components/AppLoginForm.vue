<!--
Usage of this component is implied by its name.
-->
<template>
  <v-form>
    <v-text-field
      v-model="username"
      label="Username"
      required
    ></v-text-field>
    <v-text-field
      v-model="password"
      label="Password"
      type="password"
      required
    ></v-text-field>
    <v-btn
      color="primary"
      @click="login"
    >Login</v-btn>
  </v-form>
</template>

<script>
export default {
name: "AppLoginForm"
}
</script>

<style scoped>

</style>