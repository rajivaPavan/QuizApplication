<template>
  <form @submit.prevent="register" class="register-form">
    <h2>Register</h2>
    <label>
      Name:
      <input type="text" v-model="name" required>
    </label>
    <label>
      Email:
      <input type="email" v-model="email" required>
    </label>
    <label>
      Password:
      <input type="password" v-model="password" required>
    </label>
    <label>
      Confirm Password:
      <input type="password" v-model="confirmPassword" required>
    </label>
    <button type="submit">Register</button>
  </form>
</template>

<style scoped>

.register-form {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 20px;
  border: 2px solid #ddd;
  border-radius: 5px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  max-width: 400px;
  margin: 0 auto;
}

h2 {
  margin-top: 0;
  margin-bottom: 20px;
  text-align: center;
}

label {
  display: block;
  margin-bottom: 10px;
  font-weight: bold;
}

input {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  width: 100%;
  margin-bottom: 20px;
}

button[type="submit"] {
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 5px;
  padding: 10px;
  cursor: pointer;
  font-size: 16px;
  width: 100%;
}

button[type="submit"]:hover {
  background-color: var(--primary-color));
}
</style>

<script>
import axios from 'axios'
export default {
  name:"RegisterView",
  data() {
    return {
      name: '',
      email: '',
      password: '',
      confirmPassword: '',
      endpoint: '/api/register'
    }
  },
  methods: {
    async register() {
      // Send a request to register the user
      try {
        const response = await axios.post('/api/auth/Register', {
          name: this.name,
          email: this.email,
          password: this.password,
          confirm_password: this.confirmPassword
        })
        // Handle success
      } catch (error) {
        // Handle error
      }
    }
  }
}
</script>
