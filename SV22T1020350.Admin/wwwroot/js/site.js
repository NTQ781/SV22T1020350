// Gán hàm vào window để có thể gọi ở bất cứ đâu
window.paginationSearch = function (event, form, page) {
    if (event) event.preventDefault();
    if (!form) return;

    const url = form.action;
    const targetId = form.dataset.target || "searchResult";
    const formData = new FormData(form);
    formData.append("page", page);

    const targetEl = document.getElementById(targetId);
    if (targetEl) {
        targetEl.innerHTML = '<div class="text-center py-4"><div class="spinner-border text-primary"></div></div>';
    }

    // Chuyển FormData sang URL params cho phương thức GET
    let fetchUrl = url;
    const params = new URLSearchParams(formData).toString();
    fetchUrl = url + (url.includes("?") ? "&" : "?") + params;

    fetch(fetchUrl)
        .then(res => res.text())
        .then(html => {
            if (targetEl) targetEl.innerHTML = html;
        })
        .catch(err => {
            console.error(err);
            if (targetEl) targetEl.innerHTML = '<div class="text-danger">Lỗi tải dữ liệu!</div>';
        });
};

$(document).ready(function () {
    // Xử lý mở Modal
    $(document).on("click", ".open-modal", function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        if (!url || url === "#") return;

        $("#dialogModal .modal-content").load(url, function () {
            $("#dialogModal").modal("show");
        });
    });

    // Xử lý submit Form trong Modal bằng AJAX
    $(document).on("submit", "form.ajax-form", function (e) {
        e.preventDefault();
        var form = $(this);
        $.ajax({
            url: form.attr("action"),
            type: form.attr("method"),
            data: form.serialize(),
            success: function (res) {
                if (res.code === 1 || res.Code === 1) {
                    $("#dialogModal").modal("hide");
                    if (res.orderID || res.OrderID) {
                        window.location.reload();
                    } else {
                        if (typeof showShoppingCart === "function") showShoppingCart();
                    }
                } else {
                    alert(res.message || res.Message);
                }
            }
        });
    });
});