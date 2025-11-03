// Thêm code này vào site.js

document.addEventListener("DOMContentLoaded", function () {

    // --- BẮT ĐẦU CODE HIỆU ỨNG CUỘN TRANG ---

    // 1. Lấy tất cả các phần tử có class 'fade-in-section'
    const sectionsToFade = document.querySelectorAll('.fade-in-section');

    if (sectionsToFade.length > 0) {
        // 2. Tùy chọn cho Intersection Observer
        // (Kích hoạt khi 15% của phần tử lọt vào màn hình)
        const observerOptions = {
            root: null,
            rootMargin: '0px',
            threshold: 0.15
        };

        // 3. Hàm callback sẽ chạy khi phần tử lọt vào/ra khỏi màn hình
        const observerCallback = (entries, observer) => {
            entries.forEach(entry => {
                // Nếu phần tử lọt vào màn hình
                if (entry.isIntersecting) {
                    // Thêm class 'is-visible' để kích hoạt CSS transition
                    entry.target.classList.add('is-visible');

                    // Ngừng theo dõi phần tử này (chỉ chạy hiệu ứng 1 lần)
                    observer.unobserve(entry.target);
                }
            });
        };

        // 4. Khởi tạo Observer
        const observer = new IntersectionObserver(observerCallback, observerOptions);

        // 5. Bắt đầu theo dõi tất cả các phần tử
        sectionsToFade.forEach(section => {
            observer.observe(section);
        });
    }

    // --- KẾT THÚC CODE HIỆU ỨNG CUỘN TRANG ---

});