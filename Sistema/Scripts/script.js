
function modalBehavior() {
    const openModalBtns = document.querySelectorAll('[data-open-modal]');

    if (openModalBtns !== null) {

        function openModal(targetId) {
            return () => {
                document.body.style.overflow = 'hidden';
                document.querySelector(`#${targetId}`).classList.remove('hidden-animated');
            }
        }

        function closeModal(targetId) {
            return () => {
                document.body.style.overflow = 'visible';
                document.querySelector(`#${targetId}`).classList.add('hidden-animated');
            }
        }

        function outsideClick(e, modal) {
            if (e.target === modal) {
                modal.classList.add('hidden-animated');
                document.body.style.overflow = 'visible';
            }
        }

        const closeModalBtns = document.querySelectorAll('[data-close-modal]');
        const allModals = document.querySelectorAll('.modal-bg');

        openModalBtns.forEach((item) => {
            item.addEventListener('click', openModal(item.dataset.openModal));
        });

        closeModalBtns.forEach((item) => {
            item.addEventListener('click', closeModal(item.dataset.closeModal));
        });

        allModals.forEach((item) => {
            item.addEventListener('click', (e) => {
                outsideClick(e, item)
            });
        });
    }
}

let timer = null;

document.addEventListener('DOMContentLoaded', () => {
    window.onscroll = () => {
        clearTimeout(timer);
        timer = setTimeout(() => {
            if (window.innerWidth > 768) {
                if (window.scrollY > window.innerHeight * 0.2) {
                    document.querySelector('.back-to-top').classList.remove('hidden');
                } else {
                    document.querySelector('.back-to-top').classList.add('hidden');
                }
            }
        }, 500);
    };

    modalBehavior();
});

