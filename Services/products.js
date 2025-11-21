const apiBase = location.protocol + '//' + location.hostname + ':7234';

async function getProducts(){
  const res = await fetchAuth(apiBase + '/api/products');
  if (!res.ok) throw new Error('Không thể lấy danh sách sản phẩm');
  return await res.json();
}

async function getProduct(id){
  const res = await fetchAuth(apiBase + '/api/products/' + id);
  if (!res.ok) throw new Error('Không thể lấy sản phẩm');
  return await res.json();
}

async function createProduct(dto){
  const res = await fetchAuth(apiBase + '/api/products', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto)
  });
  if (!res.ok) {
    const t = await res.text(); throw new Error(t || 'Tạo sản phẩm thất bại');
  }
  return await res.json();
}

async function updateProduct(id, dto){
  const res = await fetchAuth(apiBase + '/api/products/' + id, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto)
  });
  if (!res.ok) {
    const t = await res.text(); throw new Error(t || 'Cập nhật thất bại');
  }
}

async function deleteProduct(id){
  const res = await fetchAuth(apiBase + '/api/products/' + id, { method: 'DELETE' });
  if (!res.ok) {
    const t = await res.text(); throw new Error(t || 'Xoá thất bại');
  }
}

async function createOrder(items){
  const res = await fetchAuth(apiBase + '/api/orders', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ items })
  });
  if (!res.ok) {
    const t = await res.text(); throw new Error(t || 'Tạo đơn hàng thất bại');
  }
  return await res.json();
}

async function getMyOrders(){
  const res = await fetchAuth(apiBase + '/api/orders/myorders');
  if (!res.ok) {
    const t = await res.text(); throw new Error(t || 'Không thể lấy đơn hàng của bạn');
  }
  return await res.json();
}

async function getAllOrders(){
  const res = await fetchAuth(apiBase + '/api/orders');
  if (!res.ok) {
    const t = await res.text(); throw new Error(t || 'Không thể lấy tất cả đơn hàng');
  }
  return await res.json();
}

window.getProducts = getProducts;
window.getProduct = getProduct;
window.createProduct = createProduct;
window.updateProduct = updateProduct;
window.deleteProduct = deleteProduct;
window.createOrder = createOrder;
