// The DB constrains ShipmentStatus to these exact values (see
// CK_Shipments_Status in the schema) - that's what gets sent to/from the
// API. This map only changes what's *displayed*, so admins and customers
// see familiar courier-style language instead of the raw internal value.
export const SHIPMENT_STATUSES = ['Pending', 'Picked', 'Dispatched', 'InTransit', 'Delivered', 'Failed']

const LABELS = {
  Pending: 'Processing',
  Picked: 'Picked Up',
  Dispatched: 'Dispatched',
  InTransit: 'On the way',
  Delivered: 'Delivered',
  Failed: 'Failed',
}

export function shipmentStatusLabel(status) {
  return LABELS[status] ?? status
}
