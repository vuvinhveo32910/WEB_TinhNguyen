// HIỆU ỨNG HOA MAI RƠI (Chỉ dùng Javascript thuần, siêu mượt, không làm giật web)
(function () {
    const canvas = document.createElement('canvas');
    canvas.id = 'maiCanvas';
    canvas.style.position = 'fixed';
    canvas.style.top = '0';
    canvas.style.left = '0';
    canvas.style.width = '100vw';
    canvas.style.height = '100vh';
    canvas.style.pointerEvents = 'none'; // Để người dùng vẫn click xuyên qua được
    canvas.style.zIndex = '9999';
    document.body.appendChild(canvas);

    const ctx = canvas.getContext('2d');
    let width = canvas.width = window.innerWidth;
    let height = canvas.height = window.innerHeight;

    const petals = [];
    const numPetals = 40; // Số lượng cánh hoa

    // Khởi tạo cánh hoa
    for (let i = 0; i < numPetals; i++) {
        petals.push({
            x: Math.random() * width,
            y: Math.random() * height - height,
            size: Math.random() * 8 + 4, // Kích thước
            speedY: Math.random() * 1 + 0.5, // Tốc độ rơi
            speedX: Math.random() * 1 - 0.5, // Gió tạt
            angle: Math.random() * 360,
            spin: Math.random() * 2 - 1
        });
    }

    // Vẽ cánh hoa mai (màu vàng rực rỡ)
    function drawPetal(x, y, size, angle) {
        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(angle * Math.PI / 180);
        ctx.beginPath();
        // Hình cánh hoa
        ctx.ellipse(0, 0, size, size / 2, 0, 0, 2 * Math.PI);
        ctx.fillStyle = "rgba(255, 215, 0, 0.8)"; // Màu vàng gold của hoa mai
        ctx.fill();
        ctx.lineWidth = 0.5;
        ctx.strokeStyle = "rgba(255, 140, 0, 0.5)"; // Viền hơi cam
        ctx.stroke();
        ctx.restore();
    }

    function update() {
        ctx.clearRect(0, 0, width, height);

        for (let i = 0; i < numPetals; i++) {
            let p = petals[i];
            drawPetal(p.x, p.y, p.size, p.angle);

            p.y += p.speedY;
            p.x += p.speedX;
            p.angle += p.spin;

            // Nếu rơi quá màn hình thì quay lại trên cùng
            if (p.y > height) {
                p.y = -20;
                p.x = Math.random() * width;
            }
        }
        requestAnimationFrame(update);
    }

    // Cập nhật lại kích thước khi resize màn hình
    window.addEventListener('resize', () => {
        width = canvas.width = window.innerWidth;
        height = canvas.height = window.innerHeight;
    });

    update();
})();