import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import cartService from '@/services/cartService'

export const useCartStore = defineStore('cart', () => {
  const items = ref([])
  const totalAmount = ref(0)
  const loading = ref(false)
  const error = ref(null)

  const itemCount = computed(() => items.value.reduce((sum, i) => sum + i.quantity, 0))

  function apply(cartDto) {
    items.value = cartDto.items
    totalAmount.value = cartDto.totalAmount
  }

  async function fetchCart() {
    loading.value = true
    error.value = null
    try {
      apply(await cartService.getCart())
    } catch {
      error.value = 'Could not load your cart.'
    } finally {
      loading.value = false
    }
  }

  async function addItem(productId, quantity = 1) {
    error.value = null
    try {
      apply(await cartService.addItem(productId, quantity))
    } catch (e) {
      error.value = e.response?.data?.message || 'Could not add item to cart.'
      throw e
    }
  }

  async function updateItem(productId, quantity) {
    error.value = null
    try {
      apply(await cartService.updateItem(productId, quantity))
    } catch (e) {
      error.value = e.response?.data?.message || 'Could not update quantity.'
      throw e
    }
  }

  async function removeItem(productId) {
    error.value = null
    try {
      apply(await cartService.removeItem(productId))
    } catch (e) {
      error.value = e.response?.data?.message || 'Could not remove item.'
      throw e
    }
  }

  return { items, totalAmount, loading, error, itemCount, fetchCart, addItem, updateItem, removeItem }
})
