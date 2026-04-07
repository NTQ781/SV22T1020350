$(document).ready(function () {
    // 1. Cập nhật số lượng hiển thị trên icon giỏ hàng (Badge)
    window.updateCartBadge = function () {
        $.get("/Cart/GetCartCount", function (d) {
            $("#cartBadge").text(d.count);
        });
    };

    // 2. Hiển thị thông báo Toast
    window.showToast = function (msg) {
        $("#toastMsg").text(msg);
        var toastEl = document.getElementById('liveToast');
        if (toastEl) {
            var toast = new bootstrap.Toast(toastEl);
            toast.show();
        }
    };

    // 3. Hàm thêm vào giỏ hàng dùng chung
    window.addToCart = function (productId, quantity) {
        $.post("/Cart/AddToCart", { id: productId, quantity: quantity }, function (res) {
            if (res.success) {
                $("#cartBadge").text(res.count);
                showToast(res.msg);
            } else {
                alert(res.msg || "Có lỗi xảy ra");
            }
        });
    };

    // 4. Xử lý mở Modal cho tất cả link có class .open-modal
    $(document).on("click", ".open-modal", function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        if (!url || url === "#") return;

        var modalContent = $("#dialogModal .modal-content");
        // Hiển thị trạng thái đang tải
        modalContent.html('<div class="p-5 text-center"><div class="spinner-border text-primary"></div><br>Đang tải...</div>');
        $("#dialogModal").modal("show");

        $.get(url, function (data) {
            modalContent.html(data);
        }).fail(function () {
            modalContent.html('<div class="p-3 text-danger text-center">Không thể tải nội dung!</div>');
        });
    });

    // 5. Xử lý submit Form trong Modal bằng AJAX (dành cho class .ajax-form)
    $(document).on("submit", "#dialogModal form.ajax-form", function (e) {
        e.preventDefault();
        var form = $(this);
        $.ajax({
            url: form.attr("action"),
            type: form.attr("method"),
            data: form.serialize(),
            success: function (res) {
                if (res.code === 1 || res.Code === 1 || res.success) {
                    $("#dialogModal").modal("hide");
                    // Tự động load lại trang để cập nhật dữ liệu mới nhất
                    window.location.reload();
                } else {
                    alert(res.message || res.Message || "Thao tác thất bại");
                }
            },
            error: function () {
                alert("Lỗi kết nối máy chủ!");
            }
        });
    });

    // Chạy lần đầu khi load trang
    updateCartBadge();
});