window.showModal = function (modalId) {
    var modal = new bootstrap.Modal(document.getElementById(modalId));
    modal.show();
};

window.hideModal = function (modalId) {
    var modal = bootstrap.Modal.getInstance(document.getElementById(modalId));
    if (modal) modal.hide();
};