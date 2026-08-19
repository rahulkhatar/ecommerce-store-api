import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  { path: '/', name: 'home', component: () => import('@/pages/Products.vue') },
  { path: '/login', name: 'login', component: () => import('@/pages/Login.vue') },
  { path: '/register', name: 'register', component: () => import('@/pages/Register.vue') },
  { path: '/products/:id', name: 'product-detail', component: () => import('@/pages/ProductDetail.vue'), props: true },
  { path: '/cart', name: 'cart', component: () => import('@/pages/Cart.vue') },
  { path: '/checkout', name: 'checkout', component: () => import('@/pages/Checkout.vue'), meta: { requiresAuth: true } },
  { path: '/checkout/paypal-return', name: 'paypal-return', component: () => import('@/pages/PayPalReturn.vue'), meta: { requiresAuth: true } },
  { path: '/orders', name: 'orders', component: () => import('@/pages/Orders.vue'), meta: { requiresAuth: true } },
  { path: '/orders/:id', name: 'order-detail', component: () => import('@/pages/OrderDetail.vue'), props: true, meta: { requiresAuth: true } },
  { path: '/admin/products/new', name: 'admin-product-new', component: () => import('@/pages/AdminProductNew.vue'), meta: { requiresAuth: true, requiresAdmin: true } },
  { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/pages/NotFound.vue') },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }
  if (to.meta.requiresAdmin && !auth.isAdmin) {
    return { name: 'home' }
  }
})

export default router
