import api from './api'

export default {
  // 204 (not yet shipped) resolves to null rather than throwing, since axios
  // has no response body to parse for a 204 and callers shouldn't have to
  // special-case that status themselves.
  getByOrder(orderId) {
    return api.get(`/api/shipments/order/${orderId}`).then((r) => (r.status === 204 ? null : r.data))
  },
  create(orderId, dto) {
    return api.post(`/api/shipments/order/${orderId}`, dto).then((r) => r.data)
  },
  updateStatus(shipmentId, shipmentStatus) {
    return api.put(`/api/shipments/${shipmentId}/status`, { shipmentStatus }).then((r) => r.data)
  },
}
