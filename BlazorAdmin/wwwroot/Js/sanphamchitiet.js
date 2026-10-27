// Hiển thị modal
function showModal(id) {
    var modal = new bootstrap.Modal(document.getElementById(id));
    modal.show();
}

// Ẩn modal
function hideModal(id) {
    var modal = bootstrap.Modal.getInstance(document.getElementById(id));
    modal.hide();
}

// Hiển thị toast thông báo
function showToast(type, message) {
    // Triển khai toast notification của bạn
    console.log(`${type}: ${message}`);
}

// Xác nhận xóa
function confirmDelete(message) {
    return confirm(message || 'Bạn có chắc chắn muốn xóa?');
}