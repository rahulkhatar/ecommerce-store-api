import api from './api'

export default {
  uploadProductImage(file) {
    const formData = new FormData()
    formData.append('file', file)
    return api.post('/api/uploads/product-image', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    }).then((r) => r.data)
  },
}
