//// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
//// for details on configuring this project to bundle and minify static web assets.

//// Write your JavaScript code.




//// site.js
//// Bu dosya, genel site JS kodları için kullanılır
//// Örnek: animasyonlar, fade-in vs.

//document.addEventListener("DOMContentLoaded", () => {
//    // Fade-in sınıfını tüm elemanlara uygula
//    document.querySelectorAll(".fade-in").forEach(el => {
//        el.style.opacity = 0;
//        setTimeout(() => {
//            el.style.transition = "opacity 0.5s, transform 0.5s";
//            el.style.opacity = 1;
//            el.style.transform = "translateY(0)";
//        }, 50);
//    });
//});




// site.js

document.addEventListener("DOMContentLoaded", () => {
    const deleteForms = document.querySelectorAll(".delete-form");

    deleteForms.forEach(form => {
        form.addEventListener("submit", function (event) {
            event.preventDefault();

            const popup = document.createElement("div");
            popup.classList.add("glass-popup");
            popup.innerHTML = `
                <div class="popup-content">
                    <h3>Silmek istediğine emin misin?</h3>
                    <p>Bu işlem geri alınamaz.</p>
                    <div class="popup-buttons">
                        <button class="btn-confirm">Evet, Sil</button>
                        <button class="btn-cancel">Vazgeç</button>
                    </div>
                </div>
            `;

            document.body.appendChild(popup);

            const confirmBtn = popup.querySelector(".btn-confirm");
            const cancelBtn = popup.querySelector(".btn-cancel");

            confirmBtn.addEventListener("click", () => form.submit());
            cancelBtn.addEventListener("click", () => popup.remove());
        });
    });
});
